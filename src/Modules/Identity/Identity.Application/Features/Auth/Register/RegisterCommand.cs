namespace Identity.Application.Features.Auth.Register;

using Identity.Application.Common;
using Identity.Application.DTOs;
using MediatR;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "User"
) : IRequest<Result<AuthResponse>>;