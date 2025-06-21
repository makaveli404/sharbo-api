using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SharboAPI.Application.Abstractions.Services;

namespace SharboAPI.Application.Services;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		=> _httpContextAccessor = httpContextAccessor;

	public Guid? UserId
	{
		get
		{
			var sub = _httpContextAccessor.HttpContext?.User
				.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			return Guid.TryParse(sub, out var id) ? id : null;
		}
	}
}
