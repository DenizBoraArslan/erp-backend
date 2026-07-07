using Identity.Application.Features.Auth.ActivateUser;
using Identity.Application.Features.Auth.ChangePassword;
using Identity.Application.Features.Auth.DeactivateUser;
using Identity.Application.Features.Auth.GetUsers;
using Identity.Application.Features.Auth.Login;
using Identity.Application.Features.Auth.Register;
using Identity.Application.Features.Auth.ResetPassword;
using Identity.Application.Features.Auth.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // The JWT's "sub" claim is the current user's id (see TokenService).
        // ASP.NET Core's default inbound claim mapping renames it to
        // ClaimTypes.NameIdentifier, but we fall back to the raw "sub" name
        // just in case that mapping is ever disabled.
        private Guid CurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.Parse(raw!);
        }

        // Public self-registration has been removed. Creating a user is now
        // an admin-only action (used by the "Kullanıcı Yönetimi" screen on
        // web/mobile to create accounts with a specific role).
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return Unauthorized(new { error = result.Error });

            return Ok(result.Data);
        }

        // Admin-only user list for the "Kullanıcı Yönetimi" screen.
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUsersQuery(), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var command = new UpdateUserCommand(id, request.FirstName, request.LastName, request.Role);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { id = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeactivateUserCommand(id, CurrentUserId()), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new ActivateUserCommand(id), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // Admin force-resets someone else's password (e.g. they're locked out).
        [Authorize(Roles = "Admin")]
        [HttpPatch("users/{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new AdminResetPasswordCommand(id, request.NewPassword), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }

        // Self-service: any logged-in user (any role) changes their own password.
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            var command = new ChangePasswordCommand(CurrentUserId(), request.CurrentPassword, request.NewPassword);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok();
        }
    }

    public record UpdateUserRequest(string FirstName, string LastName, string Role);
    public record ResetPasswordRequest(string NewPassword);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
