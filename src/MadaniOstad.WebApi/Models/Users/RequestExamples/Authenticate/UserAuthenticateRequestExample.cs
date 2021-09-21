using Swashbuckle.AspNetCore.Filters;

namespace MadaniOstad.WebApi.Models.Users.RequestExamples.Authenticate
{
    public class UserAuthenticateRequestExample : IExamplesProvider<AuthenticationModel.AuthenticationModel>
    {
        public AuthenticationModel.AuthenticationModel GetExamples() =>
            new AuthenticationModel.AuthenticationModel
            {
                Grant_Type = "Password",
                Username = "r.kargar.2014@gmail.com",
                Password = "abc123"
            };
    }
}
