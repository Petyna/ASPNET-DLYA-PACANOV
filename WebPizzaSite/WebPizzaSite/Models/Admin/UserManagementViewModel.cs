using System.ComponentModel.DataAnnotations;

namespace WebPizzaSite.Models.Admin
{
    public class UserManagementViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        public bool IsLockedOut { get; set; }
    }
}
