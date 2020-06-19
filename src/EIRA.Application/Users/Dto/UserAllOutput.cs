using EIRA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.Users.Dto
{
    public class UserAllOutput
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// User Id
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Included BU
        /// </summary>
        public string BU { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
    }
}
