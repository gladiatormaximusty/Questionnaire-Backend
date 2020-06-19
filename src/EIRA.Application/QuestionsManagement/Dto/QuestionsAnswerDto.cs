using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.QuestionsManagement.Dto
{
    [AutoMap(typeof(QuestionsAnswer))]
    public class QuestionsAnswerDto : EntityDto
    {
        /// <summary>
        /// 選項內容
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 推薦評分
        /// </summary>
        public string RecommendedScore { get; set; }
    }
}
