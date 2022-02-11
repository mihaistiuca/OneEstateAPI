using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Resources.Base.Exception;
using Resources.Base.Responses;
using System.Collections.Generic;
using System.Linq;

namespace OneEstateAPI.Middlewares
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = new List<PropertyError>();
                foreach (var modelState in context.ModelState)
                {
                    var propertyError = new PropertyError
                    {
                        PropertyName = modelState.Key,
                        Errors = modelState.Value.Errors.Select(a => a.ErrorMessage).ToList()
                    };

                    errors.Add(propertyError);
                }

                throw new HttpStatusCodeException(422, errors);
            }
        }
    }
}
