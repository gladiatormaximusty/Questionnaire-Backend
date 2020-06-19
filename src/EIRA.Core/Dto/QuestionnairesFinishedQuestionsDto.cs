using Abp.Application.Services.Dto;
using EIRA.Table;
using System.Collections.Generic;
using Abp.AutoMapper;

namespace EIRA.Dto
{
    [AutoMap(typeof(Questions))]
    public class QuestionnairesFinishedQuestionsDto : EntityDto
    {
        public int QuestionnairesAsignId { get; set; }

        public int? QuestionType_Id { get; set; }

        /// <summary>
        /// Question Type Code(用於顯示)
        /// </summary>
        public string QuestionTypeCode { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

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
        /// Has Supporting Document
        /// </summary>
        public bool HasSupportingDocument { get; set; }

        /// <summary>
        /// Is Supporting Document Mandatory
        /// </summary>
        public bool IsSupportingDocumentMandatory { get; set; }

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
        /// Questions Answers
        /// </summary>
        public List<QuestionnairesFinishedQuestionsAnswer> QuestionsAnswers { get; set; }

        /// <summary>
        /// SupportingDocument
        /// </summary>
        public List<SupportingDocumentOutputDto> SupportingDocument { get; set; }
    }
}
