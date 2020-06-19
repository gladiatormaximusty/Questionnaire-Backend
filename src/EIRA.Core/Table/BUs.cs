using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("BUs")]
    public class BUs : FullAuditedEntity
    {
        /// <summary>
        /// BU Name
        /// </summary>
        public string BUName { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }
    }
}
