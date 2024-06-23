using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ITokenProvider tokenProvider;
        public BaseService(IHttpClientFactory httpClient,ITokenProvider tokenProvider)
        {
            _httpClient = httpClient;
            this.tokenProvider = tokenProvider;
        }

        public async Task<ResponseDTO?> SendAsync(RequestDTO dto,bool isBearer = true)
        {
            try
            {
                var client = _httpClient.CreateClient();
                HttpRequestMessage message = new();
                message.Headers.Add("Accept", "application/json");
                if(isBearer)
                {
                    var token = tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }
                message.RequestUri = new Uri(dto.Url);

                if (dto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(dto.Data), Encoding.UTF8, "application/json");
                }

                switch (dto.ApiType)
                {
                    case Utility.SD.ApiType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case Utility.SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case Utility.SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case Utility.SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;

                }


                HttpResponseMessage response = await client.SendAsync(message);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new ResponseDTO() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new ResponseDTO() { IsSuccess = false, Message = "Unauthorized" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new ResponseDTO() { IsSuccess = false, Message = "Forbidden" };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new ResponseDTO() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<ResponseDTO>(content);
                        return result;

                }
            }
            catch(Exception ex)
            {
                return new ResponseDTO() { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
