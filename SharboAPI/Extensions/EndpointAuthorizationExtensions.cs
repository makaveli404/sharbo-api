namespace SharboAPI.Extensions;

public static class EndpointAuthorizationExtensions
{
	public static RouteGroupBuilder WithAuthorization(this RouteGroupBuilder group) => group.RequireAuthorization();

	public static RouteGroupBuilder WithAuthorization(this RouteGroupBuilder group,
		string policy) => group.RequireAuthorization(policy);
}
