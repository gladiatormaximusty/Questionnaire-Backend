using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.Users.Dto
{
    public class UserAllInput : PagedResultRequestDto, ISortedResultRequest
    {
        public string Keyword { get; set; }

        public string Sorting { get; set; }
    }
}
