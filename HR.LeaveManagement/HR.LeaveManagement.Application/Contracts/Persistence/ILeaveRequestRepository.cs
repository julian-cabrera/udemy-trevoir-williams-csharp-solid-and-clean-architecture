using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Application.Contracts.Persistence
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<LeaveRequest> GetRequestWithDetails(int id);
        Task<List<LeaveRequest>> GetRequestsWithDetails();
        Task<List<LeaveRequest>> GetLeaveRequestsWithDetails(string userId);
        Task ChangeApprovalStatus(LeaveRequest leaveRequest, bool? approved);
    }
}
