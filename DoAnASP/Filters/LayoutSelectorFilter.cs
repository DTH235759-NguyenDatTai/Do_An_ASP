using Microsoft.AspNetCore.Mvc.Filters;

namespace DoAnASP.Filters
{
    public class LayoutSelectorFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            // Only apply layout selection for ViewResults
            if (result.Result is Microsoft.AspNetCore.Mvc.ViewResult viewResult)
            {
                var user = context.HttpContext.User;

                // Determine layout based on user role
                string layout = "~/Views/Shared/_LayoutCustomer.cshtml";

                if (user.Identity?.IsAuthenticated == true)
                {
                    var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                    if (role == "admin")
                    {
                        layout = "~/Views/Shared/_LayoutAdmin.cshtml";
                    }
                }

                viewResult.ViewData["Layout"] = layout;
            }
        }
    }
}