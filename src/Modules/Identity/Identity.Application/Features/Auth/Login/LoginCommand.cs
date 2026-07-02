namespace Identity.Application.Features.Auth.Login;

using Identity.Application.Common;
using Identity.Application.DTOs;
using MediatR;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;