using AutoMapper;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Domain;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class UpdateLeaveRequestCommandHandler : IRequestHandler<UpdateLeaveRequestCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(UpdateLeaveRequestCommand command, CancellationToken cancellationToken)
        {
            var leaveRequest = await _unitOfWork.LeaveRequestRepository.Get(command.Id);
            if(command.UpdateLeaveRequestDTO != null)
            {
                var validator = new UpdateLeaveRequestDtoValidator(_unitOfWork.LeaveTypeRepository);
                var validationResult = await validator.ValidateAsync(command.UpdateLeaveRequestDTO);

                if (validationResult.IsValid == false)
                    throw new ValidationException(validationResult);

                _mapper.Map(command.UpdateLeaveRequestDTO, leaveRequest);
                await _unitOfWork.LeaveRequestRepository.Update(leaveRequest);
                await _unitOfWork.Save();
            }
            else if(command.ChangeLeaveRequestApprovalDTO != null)
            {
                await _unitOfWork.LeaveRequestRepository.ChangeApprovalStatus(leaveRequest, command.ChangeLeaveRequestApprovalDTO.Approved);
                if (command.ChangeLeaveRequestApprovalDTO.Approved)
                {
                    var allocation = await _unitOfWork.LeaveAllocationRepository.GetUserAllocations(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                    int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                    allocation.NumberOfDays -= daysRequested;
                    await _unitOfWork.LeaveAllocationRepository.Update(allocation);
                }
                await _unitOfWork.Save();
            }
            return Unit.Value;
        }
    }
}
