using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveRequest.Validators;
using HR.LeaveManagement.Application.Features.LeaveRequests.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Domain;
using MediatR;
using HR.LeaveManagement.Application.Contracts.Infrastructure;
using HR.LeaveManagement.Application.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using HR.LeaveManagement.Application.Constants;

namespace HR.LeaveManagement.Application.Features.LeaveRequests.Handlers.Commands
{
    public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CreateLeaveRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<BaseCommandResponse> Handle(CreateLeaveRequestCommand commmand, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateLeaveRequestDtoValidator(_unitOfWork.LeaveTypeRepository);
            var validationResult = await validator.ValidateAsync(commmand.LeaveRequestDTO);
            var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == CustomClaimTypes.Uid)?.Value;

            var allocation = await _unitOfWork.LeaveAllocationRepository.GetUserAllocations(userId, commmand.LeaveRequestDTO.LeaveTypeId);
            if(allocation is null)
            {
                validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                    nameof(commmand.LeaveRequestDTO.LeaveTypeId), "You do not have any allocations for this leave type."));
            }
            else
            {
                int daysRequested = (int)(commmand.LeaveRequestDTO.EndDate - commmand.LeaveRequestDTO.StartDate).TotalDays;
                if(daysRequested > allocation.NumberOfDays)
                {
                    validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure(
                        nameof(commmand.LeaveRequestDTO.EndDate), "You do not have enough days for this request."));
                }
            }

            if(validationResult.IsValid == false)
            {
                response.Success = false;
                response.Message = "Creation failed";
                response.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                var leaveRequest = _mapper.Map<LeaveRequest>(commmand.LeaveRequestDTO);
                leaveRequest.RequestingEmployeeId = userId;
                leaveRequest = await _unitOfWork.LeaveRequestRepository.Add(leaveRequest);
                await _unitOfWork.Save();

                response.Success = true;
                response.Message = "Creation successful";
                response.Id = leaveRequest.Id;

                var emailAdress = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
                //var emailAdress = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                var email = new Email
                {
                    To = emailAdress,
                    Body = $"Your leave request for {commmand.LeaveRequestDTO.StartDate:D} to {commmand.LeaveRequestDTO.EndDate:D} has been submitted successfully.",
                    Subject = "Leave request submitted"
                };
                try
                {
                    await _emailSender.SendEmail(email);
                }
                catch (Exception ex)
                {
                    response.Errors = new List<string>() { ex.Message };
                }
            }
            return response;
        }
    }
}
