using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class UserDatabaseDbContext : IdentityDbContext<User>
    {
        public UserDatabaseDbContext(DbContextOptions options) : base(options)
        {     
        }
    }
}
