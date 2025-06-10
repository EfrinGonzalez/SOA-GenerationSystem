using Moq;
using SOA.Entities;
using SOA.Interfaces;


namespace SOA.Queries.Tests
{
    [TestFixture]
    public class GetUserByIdQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private GetUserByIdQueryHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnUserDto_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            var query = new GetUserByIdQuery { Id = userId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(user.Id));
            Assert.That(result?.Email, Is.EqualTo(user.Email));
            Assert.That(result?.IsActive, Is.EqualTo(user.IsActive));
        }

        [Test]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            var query = new GetUserByIdQuery { Id = userId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
