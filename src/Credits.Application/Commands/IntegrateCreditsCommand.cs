using Credits.Application.DTOs.Requests;

namespace Credits.Application.Commands;
public sealed record IntegrateCreditsCommand(IReadOnlyCollection<IntegrateCreditRequest> Credits);
