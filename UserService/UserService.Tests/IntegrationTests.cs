
namespace UserService.UserService.Tests
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using controller;
    using System.Net;
    using Microsoft.AspNetCore.Mvc.Testing;

    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        internal IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnCreatedStatus_WhenUserIsValid()
        {
            var userRegistrationDto = new UserRegistrationDto
            {
                UserName = "testuser",
                Password = "password123"
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(userRegistrationDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/user/register", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnOkStatus_WhenCredentialsAreValid()
        {
            var userAuthenticationDto = new UserAuthenticationDto
            {
                UserName = "testuser",
                Password = "password123"
            };

            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(userAuthenticationDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/user/authenticate", content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            var userId = "123";
            var url = $"/api/user/{userId}";

            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var userId = "999";
            var url = $"/api/user/{userId}";

            var response = await _client.GetAsync(url);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

}
