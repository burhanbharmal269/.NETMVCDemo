using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVCCrudAjax
{
    public class RequireSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Your session management logic here
            var emailID = context.HttpContext.Session.GetString("EmailID");

            if (string.IsNullOrEmpty(emailID))
            {
                // User is not authenticated, handle the scenario accordingly
                context.Result = new RedirectToActionResult("Home", "Privacy", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
