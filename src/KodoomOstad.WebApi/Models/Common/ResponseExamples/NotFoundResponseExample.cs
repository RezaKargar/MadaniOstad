using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Common.ResponseExamples
{
    public class NotFoundResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => null;
    }
}
