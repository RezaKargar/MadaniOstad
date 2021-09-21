using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.ResponseExamples.GetList
{
    public class UserGetListOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                Data = new[]
                 {
                    new UsersOutputDto
                    {
                        Id = 1,
                        Name = "Reza Kargar",
                        Email = "r.kargar.2014@gmail.com",
                        Username = "r.kargar.2014@gmail.com",
                        StudentId = "9718",
                        CreatedAt = "9/13/2020 7:03:57 PM"
                    },
                    new UsersOutputDto
                    {
                        Id = 2,
                        Name = "Amir Kabiri",
                        Email = "akabiridev@gmail.com",
                        Username = "akabiridev@gmail.com",
                        StudentId = "9718",
                        CreatedAt = "9/15/2020 03:00:00 PM",
                        LastModifiedAt = "10/15/2020 05:30:00 PM"
                    }
                }
            };
        }
    }
}
