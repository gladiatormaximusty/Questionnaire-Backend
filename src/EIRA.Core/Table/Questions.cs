using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("Questions")]
    public class Questions : FullAuditedEntity
    {
        public int? QuestionType_Id { get; set; }

        /// <summary>
        /// Question Type
        /// </summary>
        [ForeignKey("QuestionType_Id")] 
        public QuestionTypes QuestionType { get; set; }

        /// <summary>
        /// Question Code（頁面顯示的Question Id）
        /// </summary>
        public string QuestionCode { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Highest Rating
        /// </summary>
        public string HighestRating { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Scoring Method
        /// </summary>
        public string ScoringMethod { get; set; }

        /// <summary>
        /// Has Answer
        /// </summary>
        public bool HasAnswer { get; set; }

        /// <summary>
        /// Is Answer Mandatory
        /// </summary>
        public bool IsAnswerMandatory { get; set; }

        /// <summary>
        /// Has Free Text
        /// </summary>
        public bool HasFreeText { get; set; }

        /// <summary>
        /// Free Text Placeholder
        /// </summary>
        public string FreeTextPlaceholder { get; set; }

        /// <summary>
        /// Is Free Text Mandatory
        /// </summary>
        public bool IsFreeTextMandatory { get; set; }

        /// <summary>
        /// Is Free Text Numeric
        /// </summary>
        public bool IsFreeTextNumeric { get; set; }

        /// <summary>
        /// Has Supporting Document
        /// </summary>
        public bool HasSupportingDocument { get; set; }

        /// <summary>
        /// Is Supporting Document Mandatory
        /// </summary>
        public bool IsSupportingDocumentMandatory { get; set; }

        /// <summary>
        /// Questions Answers
        /// </summary>
        public List<QuestionsAnswer> QuestionsAnswers { get; set; }
    }
}
