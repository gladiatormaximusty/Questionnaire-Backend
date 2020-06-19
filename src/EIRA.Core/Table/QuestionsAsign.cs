using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("QuestionsAsign")]
    public class QuestionsAsign : FullAuditedEntity
    {
        public int Questionnaire_Id { get; set; }

        /// <summary>
        /// 問卷資料
        /// </summary>
        [ForeignKey("Questionnaire_Id")]
        public Questionnaires Questionnaire { get; set; }

        public int Question_Id { get; set; }

        /// <summary>
        /// Question 資料
        /// </summary>
        [ForeignKey("Question_Id")]
        public Questions Question { get; set; }

        /// <summary>
        /// Single Answer
        /// </summary>
        public bool SingleAnswer { get; set; }
    }
}
