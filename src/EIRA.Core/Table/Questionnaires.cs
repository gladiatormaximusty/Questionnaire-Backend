using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("Questionnaires")]
    public class Questionnaires : FullAuditedEntity
    {
        /// <summary>
        /// 問卷編號（頁面顯示的Questionnaire Id）
        /// </summary>
        public string QuestionnaireCode { get; set; }

        /// <summary>
        /// 問卷名稱
        /// </summary>
        public string QuestionnaireName { get; set; }

        /// <summary>
        /// 風險類型
        /// </summary>
        public string RiskType { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Submission Deadline
        /// </summary>
        public DateTime SubmissionDeadline { get; set; }

        /// <summary>
        /// 選中的問題類型
        /// </summary>
        public string QuestionTypeIds { get; set; }
    }
}
