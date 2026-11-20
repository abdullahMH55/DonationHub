using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Admin.Commands.CreateStaff;

public record CreateStaffCommand(CreateStaffRequest Request) : IRequest<Result<Guid>>;
