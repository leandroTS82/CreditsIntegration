using Asp.Versioning;
using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;
using Credits.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Credits.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/creditos")]
public class CreditsController(IIntegrateCreditService integrateCreditService) : ControllerBase
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
}

