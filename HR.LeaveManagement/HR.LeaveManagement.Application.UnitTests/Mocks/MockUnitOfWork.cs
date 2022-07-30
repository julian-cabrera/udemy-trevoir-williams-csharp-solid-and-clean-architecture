using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.UnitTests.Mock;
using Moq;

namespace HR.LeaveManagement.Application.UnitTests.Mocks
{
    public class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockLeaveTypeRepo = MockLeaveTypeRepository.GetLeaveTypeRepository();            
            mockUow.Setup(x => x.LeaveTypeRepository).Returns(mockLeaveTypeRepo.Object);
            return mockUow;
        }
    }
}
