using System.ComponentModel.DataAnnotations;

namespace WebApi.Models.ViewModels
{
    public class EditUserVM
    {
        [StringLength(55)]
        public string Name { get; set; }

        [StringLength(55)]
        public string UserName { get; set; }

        [StringLength(55, MinimumLength = 8)]
        public string OldPassword { get; set; }

        [StringLength(55, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
