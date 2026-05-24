using Credits.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credits.Infrastructure.Persistence.Configurations;

public sealed class CreditEntityTypeConfiguration : IEntityTypeConfiguration<Credit>
{
    public void Configure(EntityTypeBuilder<Credit> builder)
    {
        builder.ToTable("credito");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
               .HasColumnName("id")
               .UseIdentityByDefaultColumn();

        builder.Property(c => c.CreditNumber)
               .HasColumnName("numero_credito")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(c => c.NfseNumber)
               .HasColumnName("numero_nfse")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(c => c.ConstitutionDate)
               .HasColumnName("data_constituicao")
               .IsRequired();

        builder.Property(c => c.IssqnAmount)
               .HasColumnName("valor_issqn")
               .HasColumnType("decimal(15,2)")
               .IsRequired();

        builder.Property(c => c.CreditType)
               .HasColumnName("tipo_credito")
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(c => c.IsSimpleNational)
               .HasColumnName("simples_nacional")
               .IsRequired();

        builder.Property(c => c.TaxRate)
               .HasColumnName("aliquota")
               .HasColumnType("decimal(5,2)")
               .IsRequired();

        builder.Property(c => c.BilledAmount)
               .HasColumnName("valor_faturado")
               .HasColumnType("decimal(15,2)")
               .IsRequired();

        builder.Property(c => c.DeductionAmount)
               .HasColumnName("valor_deducao")
               .HasColumnType("decimal(15,2)")
               .IsRequired();

        builder.Property(c => c.CalculationBase)
               .HasColumnName("base_calculo")
               .HasColumnType("decimal(15,2)")
               .IsRequired();

        // Índices para as consultas dos endpoints GET
        builder.HasIndex(c => c.NfseNumber)
               .HasDatabaseName("ix_credito_numero_nfse");

        builder.HasIndex(c => c.CreditNumber)
               .HasDatabaseName("ix_credito_numero_credito");
    }
}
