using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("QuestionnairesAsign")]
    public class QuestionnairesAsign : FullAuditedEntity
    {
        public int? Questionnaire_Id { get; set; }

        /// <summary>
        /// 問卷資料
        /// </summary>
        [ForeignKey("Questionnaire_Id")] 
        public Questionnaires Questionnaire { get; set; }

        public int? Question_Id { get; set; }

        /// <summary>
        /// Question 資料
        /// </summary>
        [ForeignKey("Question_Id")]
        public Questions Question { get; set; }

        public int? BU_Id { get; set; }

        /// <summary>
        /// BU 資料
        /// </summary>
        [ForeignKey("BU_Id")] 
        public BUs BU { get; set; }

        public int? Entity_Id { get; set; }

        /// <summary>
        /// Entity 資料
        /// </summary>
        [ForeignKey("Entity_Id")]
        public Entities Entity { get; set; }

        /// <summary>
        /// Single Answer
        /// </summary>
        public bool SingleAnswer { get; set; }

        /// <summary>
        /// Selected Answer
        /// </summary>
        public string SelectedAnswer { get; set; }

        /// <summary>
        /// Free Text
        /// </summary>
        public string FreeText { get; set; }

        /// <summary>
        /// 問卷回答時間
        /// </summary>
        public DateTime? AnswerTime { get; set; }

        /// <summary>
        /// 問卷回答人Id
        /// </summary>
        public long? AnswerUserId { get; set; }
    }
}
