using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.QuestionTypesManagement.Dto;
using EIRA.Table;
using System;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    [AutoMap(typeof(Questionnaires))]
    public class QuestionnaireDto : EntityDto
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
        public string SubmissionDeadline { get; set; }

        /// <summary>
        /// 問題類型
        /// </summary>
        public List<QuestionTypeAllOutputDto> QuestionType { get; set; }
    }
}