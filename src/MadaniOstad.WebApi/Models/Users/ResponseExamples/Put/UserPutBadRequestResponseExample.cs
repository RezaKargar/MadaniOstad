using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.ResponseExamples.Put
{
    public class UserPutBadRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Errors = new[]
            {
                "The Email field is required.",
                "User name 'r.kargar.2014@gmail.com' is already taken.",
                "Email 'r.kargar.2014@gmail.com' is already taken.",
                "The Email field is not a valid e-mail address."
            }
        };
    }
}
