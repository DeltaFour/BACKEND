using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeltaFour.CrossCutting.Middleware
{
    ///<summary>
    ///Function for extract user from context
    ///</summary>
    public static class ContextUser
    {
        public static T GetUserAuthenticated<T>(this HttpContext context)
        {
            var value = context.User.FindFirst("user")!.Value;
            return JsonConvert.DeserializeObject<T>(value)!;
        }
    }
}
