using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Get
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
