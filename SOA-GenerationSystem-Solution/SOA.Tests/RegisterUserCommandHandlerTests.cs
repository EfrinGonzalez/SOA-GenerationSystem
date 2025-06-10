using Moq;
using SOA.Entities;
using SOA.Interfaces;
using SOA.DomainEvents;

namespace SOA.Commands.Tests
{
    [TestFixture]
    public class RegisterUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IPasswordHasher> _passwordHasherMock = null!;
        private Mock<IEventPublisher> _eventPublisherMock = null!;
        private Mock<IUserFactory> _userFactoryMock = null!;
        private RegisterUserCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _userFactoryMock = new Mock<IUserFactory>();

            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _passwordHasherMock.Object,
                _eventPublisherMock.Object,
                _userFactoryMock.Object);
        }

        [Test]
        public async Task Handle_ShouldRegisterUserSuccessfully()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                TenantId = Guid.NewGuid(),
                Email = "test@example.com",
                Password = "securepassword"
            };

            var hashedPassword = "hashed_securepassword";
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                TenantId = command.TenantId,
                Email = command.Email,
                PasswordHash = hashedPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(h => h.Hash(command.Password)).Returns(hashedPassword);
            _userFactoryMock.Setup(f => f.Create(command.TenantId, command.Email, command.Password)).Returns(newUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo(command.Email));
            _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == command.Email)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishAsync(
                It.Is<UserSelfRegistered>(evt => evt.UserId == newUser.Id && evt.Email == newUser.Email),
                "user.registered"), Times.Once);
        }

        [Test]
        public void Handle_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                TenantId = Guid.NewGuid(),
                Email = "duplicate@example.com",
                Password = "password"
            };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync(new User());

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.That(ex?.Message, Is.EqualTo("Email already registered"));

            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _eventPublisherMock.Verify(e => e.PublishAsync(It.IsAny<UserSelfRegistered>(), It.IsAny<string>()), Times.Never);
        }
    }
}
