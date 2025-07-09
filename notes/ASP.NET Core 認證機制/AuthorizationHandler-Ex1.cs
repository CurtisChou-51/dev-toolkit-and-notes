public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            return Task.CompletedTask;

        var dateOfBirth = Convert.ToDateTime(context.User.FindFirst(ClaimTypes.DateOfBirth).Value);
        var age = DateTime.Today.Year - dateOfBirth.Year;

        if (dateOfBirth > DateTime.Today.AddYears(-age))
            age--;

        if (age >= requirement.MinimumAge)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

options.AddPolicy("AtLeast18", policy =>
{
    policy.Requirements.Add(new MinimumAgeRequirement(18));
});

builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();