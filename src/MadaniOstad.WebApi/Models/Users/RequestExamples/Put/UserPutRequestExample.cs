using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.RequestExamples.Put
{
    public class UserPutRequestExample : IExamplesProvider<UsersUpdateInputDto>
    {
        public UsersUpdateInputDto GetExamples() => new UsersUpdateInputDto
        {
            Name = "Reza Kargar Kheljani",
            Email = "r.kargar.2014@gmail.com",
            StudentId = "9718",
            Username = "r.kargar.2014@gmail.com"
        };
    }
}
