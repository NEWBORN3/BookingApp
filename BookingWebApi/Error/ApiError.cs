using System.Runtime.Serialization.Json;
using System.Text.Json;

namespace WebApi.Error
{
    public class ApiError
    {
        public ApiError(int errorCode, string errorMessage, string errorDetial = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.ErrorDetial = errorDetial;

        }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string ErrorDetial { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}