using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace DeltaFour.CrossCutting.Middleware
{
    public static class ContextUser
    {
        public static void SetObject(this HttpContext context, string key, object value)
        {
            List<Claim> claim = new List<Claim>
                {
                    new(key, JsonConvert.SerializeObject(value)) 
                };
            var identity = new ClaimsIdentity(claim, "Jwt");
            context.User = new ClaimsPrincipal(identity);
        }

        public static T GetObject<T>(this HttpContext context)
        {
            var value = context.User.FindFirst("user")!.Value;
            return JsonConvert.DeserializeObject<T>(value)!;
        }
    }
}
