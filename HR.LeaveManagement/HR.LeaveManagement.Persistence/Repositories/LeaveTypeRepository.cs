using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Persistence.Repositories
{
    public class LeaveTypeRepository : GenericRepository<LeaveType>, ILeaveTypeRepository
    {
        private readonly HrLeaveManagementDbContext _context;
        public LeaveTypeRepository(HrLeaveManagementDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
