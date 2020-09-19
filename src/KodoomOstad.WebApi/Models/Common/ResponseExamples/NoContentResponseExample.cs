using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Common.ResponseExamples
{
    public class NoContentResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => null;
    }
}
