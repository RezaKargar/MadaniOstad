using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Common.ResponseExamples
{
    public class NoContentResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => null;
    }
}
