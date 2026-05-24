using Asp.Versioning;
using Credits.Application.Abstractions;
using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Credits.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/creditos")]
public class CreditsController(IIntegrateCreditService integrateCreditService,
        IQueryCreditService queryCreditService) : ControllerBase
{
    [HttpPost("integrar-credito-constituido")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Integrate([FromBody] IReadOnlyCollection<IntegrateCreditRequest> request, CancellationToken ct)
    {
        var command = new IntegrateCreditsCommand(request);

        await integrateCreditService.IntegrateAsync(command, ct);

        return StatusCode(StatusCodes.Status202Accepted, new { success = true });
    }

    [HttpGet("{numeroNfse}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNfse(
    [FromRoute, StringLength(50, MinimumLength = 1)] string numeroNfse,
    CancellationToken ct)
    {
        var result = await queryCreditService.GetByNfseNumberAsync(numeroNfse, ct);
        return result.Count == 0 ? NotFound() : Ok(result);
    }

    [HttpGet("credito/{numeroCredito}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCredit(
        [FromRoute, StringLength(50, MinimumLength = 1)] string numeroCredito,
        CancellationToken ct)
    {
        var result = await queryCreditService.GetByCreditNumberAsync(numeroCredito, ct);
        return result is null ? NotFound() : Ok(result);
    }
}

