using HR.LeaveManagement.MVC.Models.VMs;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Contracts
{
    public interface ILeaveRequestService
    {
        Task<AdminLeaveRequestVM> GetAdminLeaveRequestList();
        Task<EmployeeLeaveRequestVM> GetUserLeaveRequests();
        Task<Response<int>> CreateLeaveRequest(CreateLeaveRequestVM leaveRequest);
        Task<LeaveRequestVM> GetLeaveRequest(int id);
        Task ApproveLeaveRequest(int id, bool approved);
        Task DeleteLeaveRequest(int id);
    }
}
