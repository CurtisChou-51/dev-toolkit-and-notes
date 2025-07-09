public class RoleFuncAuthRequirement : IAuthorizationRequirement
{

}

public class RoleFuncAuthHandler : AuthorizationHandler<RoleFuncAuthRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRoleFuncService _roleFuncService;

    public RoleFuncAuthHandler(IHttpContextAccessor httpContextAccessor, IRoleFuncService roleFuncService)
    {
        _httpContextAccessor = httpContextAccessor;
        _roleFuncService = roleFuncService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleFuncAuthRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var routeData = httpContext?.GetRouteData();
        var controller = routeData?.Values["controller"]?.ToString();

        IEnumerable<string> userRoles = context.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        IEnumerable<string> allowRoles = _roleFuncService.GetRoleFuncs()
            .Where(x => x.Controller == controller)
            .SelectMany(x => x.Roles);

        if (allowRoles.Intersect(userRoles).Any())
            context.Succeed(requirement);
        return Task.CompletedTask;
    }
}