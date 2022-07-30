using HR.LeaveManagement.MVC.Contracts;

namespace HR.LeaveManagement.MVC.Services.Base
{
    public class BaseHttpService
    {
        protected readonly ILocalStorageService _localStorage;
        protected IClient _client;
        public BaseHttpService(IClient client, ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }
        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException ex)
        {
            if(ex.StatusCode == 400)
            {
                return new Response<Guid>() { Message = "Validation errors have ocurred.", ValidationErrors = ex.Response, Success = false };
            }
            if (ex.StatusCode == 404)
            {
                return new Response<Guid>() { Message = "The requested item could not be found.", Success = false };
            }
            else
            {
                return new Response<Guid>() { Message = "Something went wrong, please try again.", Success = false };
            }
        }
        protected void AddBearerToken()
        {
            if (_localStorage.Exists("token"))
            {
                _client.HttpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _localStorage.GetStorageValue<string>("token"));
            }
        }
    }
}
