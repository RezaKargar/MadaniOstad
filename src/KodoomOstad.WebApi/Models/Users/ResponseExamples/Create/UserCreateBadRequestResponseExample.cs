using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Create
{
    public class UserCreateBadRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Errors = new[]
                {
                    "The Email field is required.",
                    "The Password field is required.",
                    "User name 'r.kargar.2014@gmail.com' is already taken.",
                    "Email 'r.kargar.2014@gmail.com' is already taken.",
                    "The Email field is not a valid e-mail address.",
                    "Password must be at least 6 characters.",
                    "Passwords must have at least one digit ('0'-'9')."
                }
        };
    }
}
