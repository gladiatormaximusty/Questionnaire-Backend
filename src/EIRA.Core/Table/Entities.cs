using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("Entities")]
    public class Entities : FullAuditedEntity
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }
    }
}
