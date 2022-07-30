using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        private readonly HrLeaveManagementDbContext _context;
        public LeaveRequestRepository(HrLeaveManagementDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task ChangeApprovalStatus(LeaveRequest leaveRequest, bool? approved)
        {
            leaveRequest.Approved = approved;
            _context.Entry(leaveRequest).State = EntityState.Modified;
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsWithDetails(string userId)
        {
            var leaveRequests = await _context.LeaveRequests.Where(q => q.RequestingEmployeeId == userId)
                .Include(q => q.LeaveType)
                .ToListAsync();
            return leaveRequests;
        }

        public async Task<List<LeaveRequest>> GetRequestsWithDetails()
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(x => x.LeaveType)
                .ToListAsync();
            return leaveRequest;
        }

        public async Task<LeaveRequest> GetRequestWithDetails(int id)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == id);
            return leaveRequest;
        }
    }
}
