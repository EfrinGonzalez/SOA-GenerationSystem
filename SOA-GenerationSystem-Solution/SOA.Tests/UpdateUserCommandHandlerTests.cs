using Moq;
using SOA.DomainEvents;
using SOA.Entities;
using SOA.Interfaces;

namespace SOA.Commands.Tests
{
    [TestFixture]
    public class UpdateUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IEventPublisher> _eventPublisherMock = null!;
        private UpdateUserCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            _handler = new UpdateUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _eventPublisherMock.Object);
        }

        [Test]
        public async Task Handle_ShouldUpdateUserSuccessfully()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                UserId = Guid.NewGuid(),
                Email = "updated@example.com",
                IsActive = false
            };

            var existingUser = new User
            {
                Id = command.UserId,
                Email = "old@example.com",
                TenantId = Guid.NewGuid(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId)).ReturnsAsync(existingUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(existingUser.Email, Is.EqualTo(command.Email));
            Assert.That(existingUser.IsActive, Is.EqualTo(command.IsActive));

            _userRepositoryMock.Verify(r => r.UpdateAsync(existingUser), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishAsync(
                It.Is<UserUpdated>(evt => evt.UserId == command.UserId && evt.Email == command.Email && evt.IsActive == command.IsActive),
                "user.updated"), Times.Once);
        }

        [Test]
        public async Task Handle_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                UserId = Guid.NewGuid(),
                Email = "nonexistent@example.com",
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId)).ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.False);
            _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _eventPublisherMock.Verify(e => e.PublishAsync(It.IsAny<UserUpdated>(), It.IsAny<string>()), Times.Never);
        }
    }
}
