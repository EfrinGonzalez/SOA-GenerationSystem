using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;

namespace SOA.IntegrationTests
{
    public class UserRegistrationTests : TestFixture
    {
        [Test]
        public async Task Should_Register_User_Successfully()
        {
            var client = Factory.CreateClient();

            var payload = new
            {
                tenantId = Guid.NewGuid(),
                email = "integration@example.com",
                password = "MySecure123!"
            };

            var response = await client.PostAsJsonAsync("/users/register", payload);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain(payload.email);
        }
    }
}

