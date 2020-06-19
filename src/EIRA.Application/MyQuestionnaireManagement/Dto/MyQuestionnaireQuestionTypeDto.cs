using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    [AutoMap(typeof(QuestionTypes))]
    public class MyQuestionnaireQuestionTypeDto : EntityDto
    {
        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }
    }
}
