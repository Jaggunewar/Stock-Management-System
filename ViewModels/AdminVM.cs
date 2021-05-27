using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.ViewModels
{
    public class AdminVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class ChangePasswordVM
    {
        public string Id { get; set; }
        [RegularExpression("/(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8}/g")]
       [Required]
        public string NewPassword { get; set; }
        [RegularExpression("/(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8}/g")]
        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

    }
}
