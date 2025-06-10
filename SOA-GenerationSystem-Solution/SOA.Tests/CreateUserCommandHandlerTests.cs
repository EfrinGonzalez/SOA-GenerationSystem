using Moq;
using SOA.DomainEvents;
using SOA.Entities;
using SOA.Interfaces;

namespace SOA.Commands.Tests
{
    [TestFixture]
    public class CreateUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IPasswordHasher> _passwordHasherMock = null!;
        private Mock<IEventPublisher> _eventPublisherMock = null!;
        private Mock<IUserFactory> _userFactoryMock = null!;
        private CreateUserCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _userFactoryMock = new Mock<IUserFactory>();

            _handler = new CreateUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object,
                _eventPublisherMock.Object,
                _userFactoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TenantId = Guid.NewGuid(),
                Email = "admin@example.com",
                Password = "AdminPass123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = command.TenantId,
                Email = command.Email,
                PasswordHash = "hashed",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);
            _userFactoryMock.Setup(f => f.Create(command.TenantId, command.Email, command.Password)).Returns(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(user.Id));
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == user.Email)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _eventPublisherMock.Verify(p => p.PublishAsync(
                It.Is<UserCreatedByAdmin>(e => e.UserId == user.Id && e.Email == user.Email),
                "user.created"), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TenantId = Guid.NewGuid(),
                Email = "duplicate@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync(new User());

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.That(ex!.Message, Is.EqualTo("Email is already in use."));

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _eventPublisherMock.Verify(e => e.PublishAsync(It.IsAny<UserCreatedByAdmin>(), It.IsAny<string>()), Times.Never);
        }
    }
}
