using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.QuestionnaireManagement.Dto
{
    [AutoMap(typeof(QuestionTypes))]
    public class QuestionnaireQuestionTypeDto : EntityDto
    {
        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }
    }
}
