using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Common.ResponseExamples
{
    public class NotFoundResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => null;
    }
}
