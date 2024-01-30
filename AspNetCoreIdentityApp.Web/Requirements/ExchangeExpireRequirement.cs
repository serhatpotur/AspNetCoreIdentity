using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreIdentityApp.Web.Requirements
{
    public class ExchangeExpireRequirement : IAuthorizationRequirement
    {
    }

    public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
        {
            var hasExchangeExpireClaim = context.User.HasClaim(x => x.Type == "ExchangeExpireDate");

            if (!hasExchangeExpireClaim)
            {
                context.Fail();
                return Task.CompletedTask;

            }

            var exchangeExprieDate = context.User.FindFirst("ExchangeExpireDate");

            if (DateTime.Now > Convert.ToDateTime(exchangeExprieDate.Value))
            {
                context.Fail();
                return Task.CompletedTask;

            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
