using Moq;
using SOA.DomainEvents;
using SOA.Interfaces;


namespace SOA.Commands.Tests
{
    [TestFixture]
    public class DeleteUserCommandHandlerTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IUnitOfWork> _unitOfWorkMock = null!;
        private Mock<IEventPublisher> _eventPublisherMock = null!;
        private DeleteUserCommandHandler _handler = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            _handler = new DeleteUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _eventPublisherMock.Object);
        }

        [Test]
        public async Task Handle_ShouldDeleteUserSuccessfully()
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result, Is.True);
            _userRepositoryMock.Verify(r => r.DeleteAsync(command.UserId), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishAsync(
                It.Is<UserDeleted>(evt => evt.UserId == command.UserId),
                "user.deleted"), Times.Once);
        }
    }
}
