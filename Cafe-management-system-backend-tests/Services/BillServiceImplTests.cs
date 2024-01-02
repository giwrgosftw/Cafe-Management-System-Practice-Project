using Moq;
using AutoFixture;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Services;
using System.Data;

namespace Cafe_management_system_backend_tests.Services
{
    [TestClass]
    public class BillServiceImplTests
    {
        private Mock<BillRepository> mockBillRepository;
        private BillServiceImpl billService;
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            mockBillRepository = new Mock<BillRepository>();
            billService = new BillServiceImpl(mockBillRepository.Object);
            fixture = new Fixture();
            /* Fixed: AutoFixture.ObjectCreationExceptionWithPath: AutoFixture was unable to create an instance
            // of type AutoFixture.Kernel.SeededRequest because the traversed object graph contains a circular reference.
            // Information about the circular path follows below. This is the correct behavior when
            // a Fixture is equipped with a ThrowingRecursionBehavior, which is the default.
            // This ensures that you are being made aware of circular references in your code,
            // you can replace this default behavior with a different behavior: on the Fixture instance,
            // remove the ThrowingRecursionBehavior from Fixture.Behaviors, and instead add an instance of OmitOnRecursionBehavior:
            */
            // Handle circular refences loop
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [TestMethod]
        public void FindAllBills_ShouldReturnListOfBills()
        {
            // Arrange
            var expectedBills = fixture.CreateMany<Bill>(5).ToList();
            mockBillRepository.Setup(repo => repo.FindAll()).Returns(expectedBills);

            // Act
            var result = billService.FindAllBills();

            // Assert
            CollectionAssert.AreEqual(expectedBills, result);
        }

        [TestMethod]
        public void FindAllBillsByCreatedBy_ShouldReturnListOfBills()
        {
            // Arrange
            string emailCreatedBy = "test@example.com";
            var expectedBills = fixture.CreateMany<Bill>(3).ToList();
            mockBillRepository.Setup(repo => repo.FindAllByCreatedBy(emailCreatedBy)).Returns(expectedBills);

            // Act
            var result = billService.FindAllBillsByCreatedBy(emailCreatedBy);

            // Assert
            CollectionAssert.AreEqual(expectedBills, result);
        }

        [TestMethod]
        public void FindBillByUUID_ShouldReturnBill_WhenBillExists()
        {
            // Arrange
            var bill = fixture.Create<Bill>();
            mockBillRepository.Setup(repo => repo.FindByUUID(bill.uuid)).Returns(bill);

            // Act
            var result = billService.FindBillByUUID(bill.uuid);

            // Assert
            Assert.AreEqual(bill, result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindBillByUUID_ShouldThrowException_WhenBillDoesNotExist()
        {
            // Arrange
            string billUUID = "nonexistent-uuid";
            mockBillRepository.Setup(repo => repo.FindByUUID(billUUID)).Returns((Bill)null);

            // Act
            billService.FindBillByUUID(billUUID);
        }

        [TestMethod]
        public void AddBill_ShouldAddNewBill_WhenBillDoesNotExist()
        {
            // Arrange
            var bill = fixture.Create<Bill>();
            mockBillRepository.Setup(repo => repo.FindByUUID(bill.uuid)).Returns((Bill)null);

            // Act
            billService.AddBill(bill);

            // Assert
            mockBillRepository.Verify(repo => repo.Add(bill), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateNameException))]
        public void AddBill_ShouldThrowException_WhenBillAlreadyExists()
        {
            // Arrange
            var bill = fixture.Create<Bill>();
            mockBillRepository.Setup(repo => repo.FindByUUID(bill.uuid)).Returns(bill);

            // Act
            billService.AddBill(bill);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddBill_ShouldThrowException_WhenBillCreatorIsNull()
        {
            // Arrange
            var bill = fixture.Create<Bill>();
            bill.createdBy = null;

            // Act
            billService.AddBill(bill);
        }

        [TestMethod]
        public void DeleteBill_ShouldDeleteBill_WhenBillExists()
        {
            // Arrange
            var bill = fixture.Create<Bill>();
            mockBillRepository.Setup(repo => repo.FindByUUID(bill.uuid)).Returns(bill);

            // Act
            billService.DeleteBill(bill.uuid);

            // Assert
            mockBillRepository.Verify(repo => repo.Delete(bill), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteBill_ShouldThrowException_WhenBillDoesNotExist()
        {
            // Arrange
            string billUUID = "nonexistent-uuid";
            mockBillRepository.Setup(repo => repo.FindByUUID(billUUID)).Returns((Bill)null);

            // Act
            billService.DeleteBill(billUUID);
        }
    }
}
