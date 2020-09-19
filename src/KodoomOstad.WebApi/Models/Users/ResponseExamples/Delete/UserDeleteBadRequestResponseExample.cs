using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Delete
{
    public class UserDeleteBadRequestResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Errors = new[]
            {
                "Some errors."
            }
        };
    }
}