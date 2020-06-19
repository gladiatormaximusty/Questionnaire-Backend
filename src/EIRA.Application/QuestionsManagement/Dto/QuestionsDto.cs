using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;
using System.Collections.Generic;

namespace EIRA.QuestionsManagement.Dto
{
    [AutoMap(typeof(Questions))]
    public class QuestionsDto : EntityDto
    {
        /// <summary>
        /// Question Type Id
        /// </summary>
        public int QuestionTypeId { get; set; }

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
        /// Aswer資料
        /// </summary>
        public List<QuestionsAnswerDto> QuestionsAnswers { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }
    }
}
