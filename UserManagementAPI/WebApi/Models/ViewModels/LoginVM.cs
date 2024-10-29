using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.ViewModels
{
    public class LoginVM
    {
        [StringLength(55)]
        public string UserName { get; set; }

        [StringLength(55, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
