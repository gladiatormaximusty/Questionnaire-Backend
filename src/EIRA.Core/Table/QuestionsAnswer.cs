using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("QuestionsAnswer")]
    public class QuestionsAnswer : FullAuditedEntity
    {
        public int? Question_Id { get; set; }

        /// <summary>
        /// Question 資料
        /// </summary>
        [ForeignKey("Question_Id")]
        public Questions Question { get; set; }

        /// <summary>
        /// 選項內容
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 推薦評分
        /// </summary>
        public string RecommendedScore { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }
    }
}
