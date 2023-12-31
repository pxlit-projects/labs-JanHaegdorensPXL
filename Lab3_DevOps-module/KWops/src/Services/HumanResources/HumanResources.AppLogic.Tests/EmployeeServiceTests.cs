using HumanResources.Domain;
using Moq;
using Test;

namespace HumanResources.AppLogic.Tests
{
    public class EmployeeServiceTests : TestBase
    {
        private Mock<IEmployeeFactory> _employeeFactoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private EmployeeService _service;

        [SetUp]
        public void Setup()
        {
            _employeeFactoryMock = new Mock<IEmployeeFactory>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();

            _service = new EmployeeService(_employeeFactoryMock.Object, _employeeRepositoryMock.Object);
        }

        [Test]
        public void HireNewAsync_Should_RetrieveNumberOfStarters_CreateTheEmployee_AndSaveIt()
        {
            //Arrange
            string lastName = Random.NextString();
            string firstName = Random.NextString();
            DateTime startDate = DateTime.Now;

            int numberOfStartersOnStartDate = Random.Next(1, 1000);
            _employeeRepositoryMock.Setup(repo => repo.GetNumberOfStartersOnAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(numberOfStartersOnStartDate);

            IEmployee createdEmployee = new Mock<IEmployee>().Object;
            _employeeFactoryMock
                .Setup(factory =>
                    factory.CreateNew(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(createdEmployee);

            //Act
            IEmployee result = _service.HireNewAsync(lastName, firstName, startDate).Result;

            //Assert
            _employeeRepositoryMock.Verify(repo => repo.GetNumberOfStartersOnAsync(startDate), Times.Once);
            int expectedSequence = numberOfStartersOnStartDate + 1;
            _employeeFactoryMock.Verify(
                factory => factory.CreateNew(lastName, firstName, startDate, expectedSequence), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.AddAsync(createdEmployee), Times.Once);
            Assert.That(result, Is.SameAs(createdEmployee));
        }

        [Test]
        public void DismissAsync_Should_RetrieveEmployeeFromRepository_DismissTheEmployee_AndSaveTheChanges()
        {
            //Arrange
            EmployeeNumber employeeNumber = new EmployeeNumber(DateTime.Now, 1);

            Mock<IEmployee> employeeToDismissMock = new Mock<IEmployee>();
            IEmployee employeeToDismiss = employeeToDismissMock.Object;

            _employeeRepositoryMock.Setup(repo => repo.GetByNumberAsync(It.IsAny<EmployeeNumber>()))
                .ReturnsAsync(employeeToDismiss);

            //Act
            _service.DismissAsync(employeeNumber, true).Wait();

            //Assert
            _employeeRepositoryMock.Verify(repo => repo.GetByNumberAsync(employeeNumber), Times.Once);
            employeeToDismissMock.Verify(e => e.Dismiss(true), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.CommitTrackedChangesAsync(), Times.Once);
        }
    }
}