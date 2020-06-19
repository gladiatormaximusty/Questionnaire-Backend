using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.Users.Dto
{
    public class ProfileInfoOutput
    {
        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Last Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// 郵箱
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 頭像
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Latest Update Time
        /// </summary>
        public string LastModificationTime { get; set; }

        /// <summary>
        /// Create Time
        /// </summary>
        public string CreationTime { get; set; }

        /// <summary>
        /// Latest Login Time
        /// </summary>
        public string LastLogin { get; set; }
    }
}
