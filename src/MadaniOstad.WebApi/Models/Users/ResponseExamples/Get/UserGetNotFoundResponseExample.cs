using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.ResponseExamples.Get
{
    public class UserGetNotFoundResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Errors = new[]
            {
                "Not found."
            }
        };
    }
}
