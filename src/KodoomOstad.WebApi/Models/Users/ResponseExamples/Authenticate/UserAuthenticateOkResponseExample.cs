using Swashbuckle.AspNetCore.Filters;

namespace KodoomOstad.WebApi.Models.Users.ResponseExamples.Authenticate
{
    public class UserAuthenticateOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples() => new
        {
            access_token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIyIiwidW5pcXVlX25hbWUiOiJSZXphQFJlemEuY29tIiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiJOWlhOUE5YSkVSRUhJQVhVSlY0M1M0N1hWTlpZVFVDSiIsInJvbGUiOiJBZG1pbiIsIm5iZiI6MTYwMDAwOTMzMCwiZXhwIjoxNjAwMDExMTMwLCJpYXQiOjE2MDAwMDkzMzAsImlzcyI6IktvZG9vbU9zdGFkLkFwaSIsImF1ZCI6IktvZG9vbU9zdGFkLkNsaWVudCJ9.c4I8gdWMsMcvp4LMHcF7U7QExyjN23YbldvZtc0Ir4M",
            refresh_token = "",
            token_type = "Bearer",
            expires_in = 1799
        };
    }
}
