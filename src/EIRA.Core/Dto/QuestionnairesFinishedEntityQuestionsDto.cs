using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace EIRA.Dto
{
    public class QuestionnairesFinishedEntityQuestionsDto : EntityDto
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Questions
        /// </summary>
        public List<QuestionnairesFinishedQuestionsDto> Questions { get; set; }
    }
}
