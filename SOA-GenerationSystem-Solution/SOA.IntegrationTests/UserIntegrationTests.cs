using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SOA.DTOs;
using SOA.Entities;
using SOA.Persistence;

namespace SOA.IntegrationTests
{
    [TestFixture]
    public class UserIntegrationTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                        );
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("SOA_Users_DB");
                        });
                    });
                });

            _client = _factory.CreateClient();
        }

        [Test]
        public async Task RegisterUser_ShouldCreateUser()
        {
            var dto = new RegisterUserDto
            (
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "testuser@example.com",
                "MyPass123!"
            );

            var response = await _client.PostAsJsonAsync("/users/register", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            result.Should().NotBeNull();
            result!.Email.Should().Be(dto.Email);
        }

        [Test]
        public async Task CreateUser_AsAdmin_ShouldCreateUser()
        {
            var dto = new CreateUserDto
            (
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "adminuser@example.com",
                "AdminPass123!"
            );

            var response = await _client.PostAsJsonAsync("/admin/users/create", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            result.Should().NotBeNull();
            result!.Email.Should().Be(dto.Email);
        }

        [Test]
        public async Task GetUserById_ShouldReturnUser()
        {
            var register = new RegisterUserDto
            (
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "lookupuser@example.com",
                "MyPass123!"
            );

            var createResponse = await _client.PostAsJsonAsync("/users/register", register);
            var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();

            var response = await _client.GetAsync($"/admin/users/get/{created!.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<UserDto>();
            result.Should().NotBeNull();
            result!.Email.Should().Be(register.Email);
        }

        [Test]
        public async Task UpdateUser_ShouldSucceed()
        {
            var dto = new RegisterUserDto
            (
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "updatable@example.com",
                "MyPass123!"
            );

            var createResponse = await _client.PostAsJsonAsync("/users/register", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();

            var updateDto = new
            {
                userId = created!.Id,
                email = "updated@example.com",
                isActive = true
            };

            var response = await _client.PutAsJsonAsync($"/admin/users/update/{created.Id}", updateDto);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task DeleteUser_ShouldSucceed()
        {
            var dto = new RegisterUserDto
            (
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "deletable@example.com",
                "MyPass123!"
            );

            var createResponse = await _client.PostAsJsonAsync("/users/register", dto);
            var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();

            var deleteResponse = await _client.DeleteAsync($"/admin/users/delete/{created!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}

