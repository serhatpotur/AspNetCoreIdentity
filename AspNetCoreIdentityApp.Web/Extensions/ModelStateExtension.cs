using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class ModelStateExtension
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState,List<string> errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(string.Empty, error);


            }
        }


        public static void AddModelErrorList(this ModelStateDictionary modelState, IEnumerable<IdentityError> ıdentityErrors)
        {
            foreach (var error in ıdentityErrors.ToList())
            {
                modelState.AddModelError(string.Empty, error.Description);


            }
        }
    }
}
