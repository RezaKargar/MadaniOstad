using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.RequestExamples.Create
{
    public class UserCreateRequestExample : IExamplesProvider<UsersCreateInputDto>
    {
        public UsersCreateInputDto GetExamples() => new UsersCreateInputDto
        {
            Name = "Reza Kargar",
            Email = "r.kargar.2014@gmail.com",
            Password = "abc123",
            StudentId = "9718"
        };
    }
}
