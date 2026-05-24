# CreditsIntegration

API RESTful e Background Service para integração e consulta de créditos constituídos (ISSQN/NFS-e), construídos com .NET 8, Azure Service Bus e PostgreSQL.

---

## Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                         Docker Compose                          │
│                                                                 │
│  ┌──────────┐    POST     ┌─────────────────────────────────┐  │
│  │  Client  │ ──────────► │           Credits.Api           │  │
│  │          │             │  CreditsController               │  │
│  │          │    GET      │  /api/v1/creditos/...            │  │
│  │          │ ──────────► │  /self  /ready                   │  │
│  └──────────┘             └────────────┬────────────────────┘  │
│                                        │                        │
│                           ┌────────────▼────────────────────┐  │
│                           │      Credits.Application        │  │
│                           │  IntegrateCreditService         │  │
│                           │  QueryCreditService             │  │
│                           │  IngestCreditService            │  │
│                           └──────┬──────────────┬──────────┘  │
│                                  │ publish       │ notify       │
│                                  ▼              ▼              │
│                           ┌──────────┐  ┌──────────────────┐  │
│                           │  Azure   │  │  Azure Service   │  │
│                           │ Service  │  │  Bus (Audit)     │  │
│                           │   Bus    │  │  notification-   │  │
│                           │ integrar-│  │  audit-entry     │  │
│                           │ credito- │  └────────┬─────────┘  │
│                           │ entry    │           │             │
│                           └────┬─────┘           │             │
│                                │                 │             │
│                    ┌───────────▼─────────────────▼──────────┐  │
│                    │           Credits.Worker                │  │
│                    │  ServiceBusConsumerService (500ms)      │  │
│                    │  AuditConsumerService      (500ms)      │  │
│                    └───────────────────┬────────────────────┘  │
│                                        │                        │
│                           ┌────────────▼────────────────────┐  │
│                           │     Credits.Infrastructure      │  │
│                           │  CreditRepository               │  │
│                           │  AppDbContext (EF Core)         │  │
│                           └────────────┬────────────────────┘  │
│                                        │                        │
│                           ┌────────────▼────────────────────┐  │
│                           │    PostgreSQL 16 (Docker)       │  │
│                           │    volume: postgres_data        │  │
│                           └─────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Fluxos principais

### POST — Integrar créditos

```
Client
  │
  ▼
POST /api/v1/creditos/integrar-credito-constituido
  │
  ▼
IntegrateCreditService
  ├─ Valida command (FluentValidation)
  ├─ Valida regras de negócio (duplicatas na requisição)
  └─ forEach crédito
       └─ PublishAsync → tópico: integrar-credito-constituido-entry
  │
  ▼
HTTP 202 { "success": true }
```

### Worker — Consumir e persistir

```
ServiceBusConsumerService (BackgroundService)
  │
  ├─ Polling a cada 500ms
  ├─ ReceiveMessageAsync (PeekLock)
  │
  ▼
CreditMessageProcessor
  ├─ Deserializa mensagem
  ├─ IngestCreditService
  │    ├─ Verifica se crédito já existe (idempotência)
  │    ├─ Credit.Create(...)
  │    ├─ AddAsync (inserção individual, não bulk)
  │    └─ SaveChangesAsync
  └─ CompleteMessage / DeadLetter (erro)
```

### GET — Consultar com auditoria

```
Client
  │
  ▼
GET /api/v1/creditos/{numeroNfse}
GET /api/v1/creditos/credito/{numeroCredito}
  │
  ▼
QueryCreditService
  ├─ Repository → PostgreSQL
  └─ NotifyAsync → tópico: notification-audit-entry
       └─ AuditConsumerService consome e loga o evento
  │
  ▼
HTTP 200 [ CreditResponse ] / 404
```

---

## Estrutura do projeto

```
CreditsIntegration/
├── src/
│   ├── Credits.Api             # Controllers, Middlewares, Program.cs
│   ├── Credits.Application     # Services, Commands, DTOs, Validators, Messaging
│   ├── Credits.Domain          # Entities, Repositories (interfaces), Exceptions
│   ├── Credits.Infrastructure  # EF Core, Migrations, ServiceBusPublisher
│   └── Credits.Worker          # BackgroundServices, Processors, Deserializers
├── tests/
│   ├── Credits.Api.IntegrationTests    # Testcontainers + PostgreSQL real
│   └── Credits.Application.Tests      # Unit tests com Moq + xUnit
├── config/
│   └── Config.json             # Configuração do Service Bus Emulator
└── docker-compose.yml
```

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) e Docker Compose

---

## Como rodar

### 1. Clonar o repositório

```bash
git clone https://github.com/leandrots82/CreditsIntegration.git
cd CreditsIntegration
```

### 2. Subir todos os serviços
Com Docker instalado e configurado execute o comando abaixo na raiz do projeto.
```bash
docker compose up --build
```

Isso sobe em ordem:
- `sqledge` — SQL Edge (dependência interna do emulador)
- `servicebus-emulator` — Azure Service Bus local
- `postgres` — PostgreSQL 16 com volume persistente
- `api` — Credits.Api na porta `5000`
- `worker` — Credits.Worker (Background Service)

> As migrations do banco são aplicadas automaticamente na inicialização do Worker.

### 3. Acessar a API

- Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- Health check: [http://localhost:5000/self](http://localhost:5000/self)
- Health check: [http://localhost:5000/ready](http://localhost:5000/ready)

---

## Endpoints

### POST `/api/v1/creditos/integrar-credito-constituido`

Integra uma lista de créditos, publicando uma mensagem distinta por crédito no Service Bus.

**Request:**
```json
[
  {
    "numeroCredito": "123456",
    "numeroNfse": "7891011",
    "dataConstituicao": "2024-02-25",
    "valorIssqn": 1500.75,
    "tipoCredito": "ISSQN",
    "simplesNacional": "Sim",
    "aliquota": 5.0,
    "valorFaturado": 30000.00,
    "valorDeducao": 5000.00,
    "baseCalculo": 25000.00
  }
]
```

**Response:** `202 Accepted`
```json
{ "success": true }
```

---

### GET `/api/v1/creditos/{numeroNfse}`

Retorna todos os créditos associados a uma NFS-e.

**Response:** `200 OK` — lista de créditos | `404 Not Found`

---

### GET `/api/v1/creditos/credito/{numeroCredito}`

Retorna um crédito específico pelo número do crédito constituído.

**Response:** `200 OK` — crédito | `404 Not Found`

---

## Testes

### Unitários
Execute pelo gerenciador de testes do Visual studio ou com o comando abaixo.
```bash
dotnet test tests/Credits.Application.Tests
```

Cobertura: `IntegrateCreditService`, `QueryCreditService`, `IngestCreditService`, `CreditsController`, `IntegrateCreditCommandValidator`.

### Integração
Execute pelo gerenciador de testes do Visual studio ou com o comando abaixo.
```bash
dotnet test tests/Credits.Api.IntegrationTests
```

Usa [Testcontainers](https://dotnet.testcontainers.org/) com PostgreSQL real. Cobre os três endpoints, health checks e o fluxo de ingestão.

---

## Decisões técnicas

### Por que mensagem por crédito e não batch?

O documento especifica inserção distinta no banco. Publicar uma mensagem por crédito no Service Bus garante que cada item seja processado, retentado e rastreado de forma independente — um crédito com erro não bloqueia os demais.


Este tipo de tratamento é mencionado como oportunidade de evolução deste projeto.

### Idempotência no Worker

Antes de inserir, o `IngestCreditService` verifica se o `numeroCredito` já existe via `ExistsByCreditNumberAsync`. Reprocessamentos (retry, redelivery) não geram duplicatas.

### ServiceBusClient como Singleton

Seguindo a [recomendação oficial da Microsoft](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-performance-improvements), o `ServiceBusClient` é registrado como Singleton. thread-safe por design — criar uma instância por requisição seria custoso e desnecessário.

### Separação API / Worker

A API é responsável apenas por receber e publicar. O Worker é responsável por consumir e persistir. Isso permite escalar os dois serviços de forma independente e mantém as responsabilidades isoladas.

### Arquitetura em camadas

```
Credits.Api / Credits.Worker
        ↓
Credits.Application   ← regras de negócio, serviços, validações
        ↓
Credits.Domain        ← entidades, interfaces de repositório
        ↓
Credits.Infrastructure ← EF Core, PostgreSQL, ServiceBus
```

As camadas internas (Domain, Application) não conhecem as externas — dependem apenas de abstrações.

### Pronto para CQRS

A separação já existente entre `IntegrateCreditService` (comando) e `QueryCreditService` (consulta) deixa o projeto estruturalmente preparado para evoluir para CQRS com MediatR ou Dispatchers (usar a lib ou implementar o padrão manualmente) sem quebrar contratos.

---

## Oportunidades de evolução

| Melhoria | Descrição |
|---|---|
| **Retry Policy** | Polly para retentar publicações no Service Bus em caso de falha transiente |
| **Circuit Breaker** | Evitar sobrecarga em cascata entre API e Service Bus |
| **Dead Letter Queue** | Monitoramento e reprocessamento de mensagens que falharam após N tentativas |
| **Outbox Pattern** | Garantir consistência entre persistência e publicação de mensagem na mesma transação |
| **Inbox Pattern** | Idempotência garantida por tabela de mensagens processadas, não só por chave de negócio |
| **Health checks detalhados** | Verificação ativa de conectividade com PostgreSQL e Service Bus nos endpoints `/self` e `/ready` |

---

## Variáveis de ambiente

| Variável | Descrição |
|---|---|
| `ConnectionStrings__DefaultConnection` | Connection string do PostgreSQL |
| `ServiceBus__ConnectionString` | Connection string do Azure Service Bus |
| `ServiceBus__Subscription` | Nome da subscription de créditos |
| `ServiceBus__AuditSubscription` | Nome da subscription de auditoria |
| `ServiceBus__Topics__IntegrateCreditConstituted` | Nome do tópico de integração |
| `ServiceBus__Topics__NotificationAudit` | Nome do tópico de auditoria |
| `ServiceBus__Consumer__PollingIntervalMs` | Intervalo de polling em ms (padrão: 500) |

## 7. Rascunho inicial / plano de ação

<img width="4624" height="2075" alt="RascunhoInicial_QuadroBranco" src="https://github.com/user-attachments/assets/74573d43-b0f4-4d86-8fdd-da8468361af3" />




