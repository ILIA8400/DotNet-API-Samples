using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
