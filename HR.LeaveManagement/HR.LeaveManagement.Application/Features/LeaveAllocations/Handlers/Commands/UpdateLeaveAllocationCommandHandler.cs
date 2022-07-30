using AutoMapper;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Exceptions;
using HR.LeaveManagement.Domain;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation.Validators;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Commands
{
    public class UpdateLeaveAllocationCommandHandler : IRequestHandler<UpdateLeaveAllocationCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateLeaveAllocationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(UpdateLeaveAllocationCommand command, CancellationToken cancellationToken)
        {
            var validator = new UpdateLeaveAllocationDtoValidator(_unitOfWork.LeaveAllocationRepository);
            var validationResult = await validator.ValidateAsync(command.LeaveAllocationDTO);

            if (validationResult.IsValid == false)
                throw new ValidationException(validationResult);

            var leaveAllocation = await _unitOfWork.LeaveAllocationRepository.Get(command.LeaveAllocationDTO.Id);
            
            if (leaveAllocation == null)
                throw new NotFoundException(nameof(LeaveAllocation), command.LeaveAllocationDTO.Id);

            _mapper.Map(command.LeaveAllocationDTO, leaveAllocation);
            await _unitOfWork.LeaveAllocationRepository.Update(leaveAllocation);
            await _unitOfWork.Save();
            return Unit.Value;
        }
    }
}
