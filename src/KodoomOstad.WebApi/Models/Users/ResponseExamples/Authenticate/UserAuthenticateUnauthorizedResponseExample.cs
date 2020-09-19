using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Authenticate
{
    public class UserAuthenticateUnauthorizedResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Errors = new[]
                {
                    "Email or password is incorrect.",
                    "User attempting to sign-in is lock out for a few minutes."
                }
        };
    }
}
