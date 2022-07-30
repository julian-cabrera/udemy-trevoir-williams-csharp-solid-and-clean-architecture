﻿using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models.VMs;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services
{
    public class LeaveRequestService : BaseHttpService, ILeaveRequestService
    {
        //private readonly ILeaveRequestService _leaveRequestService;
        private readonly IMapper _mapper;
        public LeaveRequestService(IClient client, 
            ILocalStorageService localStorage,
            IMapper mapper)
            //ILeaveRequestService leaveRequestService)
            : base(client, localStorage)
        {
            //_leaveRequestService = leaveRequestService;
            _mapper = mapper;
        }

        public async Task ApproveLeaveRequest(int id, bool approved)
        {
            AddBearerToken();
            try
            {
                var request = new ChangeLeaveRequestApprovalDto { Approved = approved, Id = id };
                await _client.ChangeapprovalAsync(id, request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Response<int>> CreateLeaveRequest(CreateLeaveRequestVM leaveRequest)
        {
            try
            {
                var response = new Response<int>();
                CreateLeaveRequestDto createLeaveRequestDto = _mapper.Map<CreateLeaveRequestDto>(leaveRequest);
                AddBearerToken();
                var apiResponse = await _client.LeaveRequestsPOSTAsync(createLeaveRequestDto);
                if (apiResponse.Success)
                {
                    response.Data = apiResponse.Id;
                    response.Success = true;
                }
                else
                {
                    foreach (var error in apiResponse.Errors)
                    {
                        response.ValidationErrors += error + Environment.NewLine;
                    }
                }
                return response;
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public Task DeleteLeaveRequest(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<AdminLeaveRequestVM> GetAdminLeaveRequestList()
        {
            AddBearerToken();
            var leaveRequests = await _client.LeaveRequestsAllAsync(isLoggedInUser: false);
            var model = new AdminLeaveRequestVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(x => x.Approved == true),
                PendingRequests = leaveRequests.Count(x => x.Approved == null),
                RejectedRequests = leaveRequests.Count(x => x.Approved == false),
                LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
            };
            return model;
        }

        public async Task<LeaveRequestVM> GetLeaveRequest(int id)
        {
            AddBearerToken();
            var leaveRequest = await _client.LeaveRequestsGETAsync(id);
            return _mapper.Map<LeaveRequestVM>(leaveRequest);
        }

        public async Task<EmployeeLeaveRequestVM> GetUserLeaveRequests()
        {
            AddBearerToken();
            var leaveRequests = await _client.LeaveRequestsAllAsync(isLoggedInUser: true);
            var allocations = await _client.LeaveAllocationsAllAsync(isLoggedInUser: true);
            var model = new EmployeeLeaveRequestVM
            {
                LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(allocations),
                LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests)
            };
            return model;
        }
    }
}
