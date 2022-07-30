using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace HR.LeaveManagement.Persistence.Repositories
{
    public class LeaveAllocationRepository : GenericRepository<LeaveAllocation>, ILeaveAllocationRepository
    {
        private readonly HrLeaveManagementDbContext _context;
        public LeaveAllocationRepository(HrLeaveManagementDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task AddAllocations(List<LeaveAllocation> allocations)
        {
            await _context.LeaveAllocations.AddRangeAsync(allocations);
        }

        public async Task<bool> AllocationExists(string userId, int leaveTypeId, int period)
        {
            return await _context.LeaveAllocations.AnyAsync(x => x.EmployeeId == userId
                                                                 && x.LeaveTypeId == leaveTypeId
                                                                 && x.Period == period);
        }

        public async Task<List<LeaveAllocation>> GetAllocationsWithDetails()
        {
            var leaveAllocation = await _context.LeaveAllocations
                .Include(x => x.LeaveType)
                .ToListAsync();
            return leaveAllocation;
        }

        public async Task<LeaveAllocation> GetAllocationWithDetails(int id)
        {
            var leaveAllocation = await _context.LeaveAllocations
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == id);
            return leaveAllocation;
        }

        public async Task<List<LeaveAllocation>> GetLeaveAllocationsWithDetails(string userId)
        {
            var leaveAllocations = await _context.LeaveAllocations.Where(x => x.EmployeeId == userId)
                .Include(q => q.LeaveType)
                .ToListAsync();
            return leaveAllocations;
        }

        public async Task<LeaveAllocation> GetUserAllocations(string userId, int leaveTypeId)
        {
            return await _context.LeaveAllocations.FirstOrDefaultAsync(x => x.EmployeeId == userId && x.LeaveTypeId == leaveTypeId);
        }
    }
}
