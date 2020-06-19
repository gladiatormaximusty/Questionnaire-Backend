using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.Authorization.Accounts.Dto
{
    public class ResetPPasswordInput
    {
        [Required]
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ForgetPasswordInput
    {
        [Required]
        [EmailAddress]
        public string EmailAddr { get; set; }
    }
}
