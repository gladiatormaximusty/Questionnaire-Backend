using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;
using System;

namespace EIRA.QuestionnaireManagement.Dto
{
    [AutoMap(typeof(Questionnaires))]
    public class QuestionnaireOutputDto : EntityDto
    {
        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 問卷名稱
        /// </summary>
        public string QuestionnaireName { get; set; }

        /// <summary>
        /// Submission Deadline
        /// </summary>
        public DateTime SubmissionDeadline { get; set; }

        /// <summary>
        /// Progress
        /// </summary>
        public decimal Progress { get; set; }

        /// <summary>
        /// 風險類型
        /// </summary>
        public string RiskType { get; set; }

        /// <summary>
        /// 是否允許編輯
        /// </summary>
        public bool IsEdit { get; set; }
    }
}
