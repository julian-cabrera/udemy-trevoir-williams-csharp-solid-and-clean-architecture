using AutoMapper;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;
using HR.LeaveManagement.Application.DTOs.LeaveType.Validators;
using HR.LeaveManagement.Application.Exceptions;

namespace HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands
{
    public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateLeaveTypeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(UpdateLeaveTypeCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateLeaveTypeDtoValidator();
            var validationResult = await validator.ValidateAsync(command.LeaveTypeDTO);

            if (validationResult.IsValid == false)
                throw new ValidationException(validationResult);

            var leaveType = await _unitOfWork.LeaveTypeRepository.Get(command.LeaveTypeDTO.Id);

            if (leaveType is null)
                throw new NotFoundException(nameof(leaveType), command.LeaveTypeDTO.Id);

            _mapper.Map(command.LeaveTypeDTO, leaveType);

            await _unitOfWork.LeaveTypeRepository.Update(leaveType);
            await _unitOfWork.Save();
            return Unit.Value;
        }
    }
}
