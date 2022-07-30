using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Moq;
using System.Collections.Generic;

namespace HR.LeaveManagement.Application.UnitTests.Mock
{
    public static class MockLeaveTypeRepository
    {
        public static Mock<ILeaveTypeRepository> GetLeaveTypeRepository()
        {
            var leaveTypes = new List<LeaveType>
            {
                new LeaveType
                {
                    Id = 1,
                    DefaultDays = 10,
                    Name = "Test Vacation"
                },
                new LeaveType
                {
                    Id = 2,
                    DefaultDays = 15,
                    Name = "Test Sick"
                }
            };
            var mockRepo = new Mock<ILeaveTypeRepository>();
            mockRepo.Setup(x => x.GetAll()).ReturnsAsync(leaveTypes);
            mockRepo.Setup(x => x.Add(It.IsAny<LeaveType>())).ReturnsAsync((LeaveType leaveType) =>
            {
                leaveTypes.Add(leaveType);
                return leaveType;
            });

            return mockRepo;
        }
    }
}
