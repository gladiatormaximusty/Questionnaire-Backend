using System.Collections.Generic;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    public class MyQuestionnaireEditDto
    {
        /// <summary>
        /// Questionnaires Asign Id
        /// </summary>
        public int QuestionnairesAsignId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Question Code（頁面顯示的Question Id）
        /// </summary>
        public string QuestionCode { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Answers
        /// </summary>
        public List<MyQuestionnaireQuestionAnswerDto> Answers { get; set; }

        /// <summary>
        /// Selected Answer
        /// </summary>
        public string SelectedAnswer { get; set; }

        /// <summary>
        /// Selected Answer Id
        /// </summary>
        public int SelectedAnswerId { get; set; }

        /// <summary>
        /// Free Text Placeholder
        /// </summary>
        public string FreeTextPlaceholder { get; set; }

        /// <summary>
        /// Free Text
        /// </summary>
        public string FreeText { get; set; }

        /// <summary>
        /// SupportingDocument
        /// </summary>
        public List<MyQuestionnaireSupportingDocumentDto> SupportingDocument { get; set; }

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
        /// 是否已作答
        /// </summary>
        public bool HasBeenAnswered { get; set; }
    }
}
