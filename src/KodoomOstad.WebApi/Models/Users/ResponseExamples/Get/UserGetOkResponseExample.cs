using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Get
{
    public class UserGetOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            Data = new UsersOutputDto
            {
                Id = 1,
                Name = "Reza Kargar",
                Email = "r.kargar.2014@gmail.com",
                Username = "r.kargar.2014@gmail.com",
                StudentId = "9718",
                CreatedAt = "9/13/2020 7:03:57 PM"
            }
        };
    }
}
