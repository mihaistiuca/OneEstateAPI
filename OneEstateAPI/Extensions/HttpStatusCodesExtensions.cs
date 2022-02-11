using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resources.Base.Responses;

namespace OneEstateAPI.Extensions
{
    public static class HttpStatusCodesExtensions
    {
        private static readonly int OkStatus = 200;
        private static readonly int ValidationErrorStatus = 422;
        private static readonly int UnauthorizedStatus = 401;
        private static readonly int ServerErrorStatus = 500;

        public static IActionResult Ok(this HttpResponse response, BaseResponse data)
        {
            data.Status = OkStatus;
            return new JsonResult(data);
        }

        public static IActionResult ServerError(this HttpResponse response, BaseResponse data)
        {
            data.Status = ServerErrorStatus;
            return new JsonResult(data);
        }

        public static IActionResult ValidationError(this HttpResponse response, BaseResponse data)
        {
            data.Status = ValidationErrorStatus;
            return new JsonResult(data);
        }

        public static IActionResult Unauthorized(this HttpResponse response, BaseResponse data)
        {
            data.Status = UnauthorizedStatus;
            return new JsonResult(data);
        }
    }
}
