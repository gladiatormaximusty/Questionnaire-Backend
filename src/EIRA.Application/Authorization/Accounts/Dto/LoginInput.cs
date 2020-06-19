using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.Authorization.Accounts.Dto
{
    public class LoginInput
    {
        public string Account { get; set; }

        public string Password { get; set; }
    }
}
