using Moq;
using SOA.Entities;
using SOA.Interfaces;

namespace SOA.Queries.Tests
{
    [TestFixture]
    public class GetAllUsersQueryHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private GetAllUsersQueryHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldReturnAllUserDtos()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@example.com", IsActive = true, CreatedAt = DateTime.UtcNow },
                new User { Id = Guid.NewGuid(), Email = "user2@example.com", IsActive = false, CreatedAt = DateTime.UtcNow }
            };

            _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var query = new GetAllUsersQuery();

            // Act
            var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

            // Assert
            Assert.That(result.Count, Is.EqualTo(users.Count));
            Assert.That(result[0].Email, Is.EqualTo(users[0].Email));
            Assert.That(result[1].IsActive, Is.EqualTo(users[1].IsActive));
        }

        [Test]
        public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());

            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
