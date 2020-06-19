﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.QuestionTypesManagement.Dto
{
    [AutoMap(typeof(QuestionTypes))]
    public class QuestionTypesOutputDto : EntityDto
    {
        /// <summary>
        /// Question Type Code(用於顯示)
        /// </summary>
        public string QuestionTypeCode { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// No. of Questions
        /// </summary>
        public int NumberOfQuestions { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        public string Status { get; set; }
    }
}
