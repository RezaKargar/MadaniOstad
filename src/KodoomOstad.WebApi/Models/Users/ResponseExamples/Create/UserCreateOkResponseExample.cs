using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Create
{
    public class UserCreateOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            data = new UsersOutputDto
            {
                Id = 1,
                Email = "r.kargar.2014@gmail.com",
                Username = "r.kargar.2014@gmail.com",
                Name = "Reza Kargar",
                StudentId = "9718",
                CreatedAt = "9/13/2020 7:03:57 PM",
            },
            errors = new string[] {}
        };
    }
}