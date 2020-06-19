using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionnaireAsignInputDto
    {
        /// <summary>
        /// Question Id
        /// </summary>
        public int QuestionnaireId { get; set; }

        /// <summary>
        /// Selected Question Type Id
        /// </summary>
        public List<int> SelectedQuestionTypeId { get; set; }
    }
}
