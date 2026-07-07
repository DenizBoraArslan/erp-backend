namespace Identity.Application.Features.Auth.GetUsers;

using Identity.Application.Common;
using Identity.Application.DTOs;
using MediatR;

public record GetUsersQuery() : IRequest<Result<List<UserSummaryDto>>>;
