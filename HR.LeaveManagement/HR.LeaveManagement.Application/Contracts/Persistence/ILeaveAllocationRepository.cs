using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Application.Contracts.Persistence
{
    public interface ILeaveAllocationRepository : IGenericRepository<LeaveAllocation>
    {
        Task<LeaveAllocation> GetAllocationWithDetails(int id);
        Task<List<LeaveAllocation>> GetAllocationsWithDetails();
        Task<bool> AllocationExists(string userId, int leaveTypeId, int period);
        Task AddAllocations(List<LeaveAllocation> allocations);
        Task<LeaveAllocation> GetUserAllocations(string userId, int leaveTypeId);
        Task<List<LeaveAllocation>> GetLeaveAllocationsWithDetails(string userId);
    }
}
