using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Services.Base;

namespace HR.LeaveManagement.MVC.Services;

public class LeaveAllocationService : BaseHttpService, ILeaveAllocationService
{
    private readonly IClient _client;
    private readonly ILocalStorageService _localStorage;
    public LeaveAllocationService(IClient client, ILocalStorageService localStorage)
        : base(client, localStorage)
    {
        _client = client;
        _localStorage = localStorage;
    }
    public async Task<Response<int>> CreateLeaveAllocations(int leaveTypeId)
    {
        try
        {
            var response = new Response<int>();
            CreateLeaveAllocationDto createLeaveAllocationDto = new CreateLeaveAllocationDto() { LeaveTypeId = leaveTypeId };
            AddBearerToken();
            var apiResponse = await _client.LeaveAllocationsPOSTAsync(createLeaveAllocationDto);
            if (apiResponse.Success)
            {
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
}
