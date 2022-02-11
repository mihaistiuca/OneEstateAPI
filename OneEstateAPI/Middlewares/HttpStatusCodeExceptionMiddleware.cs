using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Resources.Base.Exception;
using Resources.Base.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneEstateAPI.Middlewares
{
    public class HttpStatusCodeExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpStatusCodeExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                if (ex.StatusCode == 500) // server error 
                {
                    var result = JsonConvert.SerializeObject(new BaseResponse(ex.PropertyErrors, ex.GeneralErrors, 500),
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync(result);
                }
                else if (ex.StatusCode == 422) // validation error 
                {
                    var result = JsonConvert.SerializeObject(new BaseResponse(ex.PropertyErrors, ex.GeneralErrors, 422),
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync(result);
                }
                else if (ex.StatusCode == 401) // unauthorized 
                {
                    var result = JsonConvert.SerializeObject(new BaseResponse(ex.PropertyErrors, ex.GeneralErrors, 401),
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;

                    await context.Response.WriteAsync(result);
                }
            }
            catch (Exception ex)
            {
                var result = JsonConvert.SerializeObject(new BaseResponse(new List<string> { "Ups. Ceva nu a mers bine." }, 500),
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;

                await context.Response.WriteAsync(result);
            }
        }
    }
}
