using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    [AutoMap(typeof(QuestionsAnswer))]
    public class MyQuestionnaireQuestionAnswerDto : EntityDto
    {
        /// <summary>
        /// 選項內容
        /// </summary>
        public string AnswerContent { get; set; }
    }
}
