using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("QuestionTypes")]
    public class QuestionTypes : FullAuditedEntity
    {
        /// <summary>
        /// Question Type Code(用於顯示)
        /// </summary>
        public string QuestionTypeCode { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Questions
        /// </summary>
        public List<Questions> Questions { get; set; }
    }
}
