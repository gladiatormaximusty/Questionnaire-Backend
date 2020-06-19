using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionTypeInputDto
    {
        public int QuestionTypeId { get; set; }

        public List<AsignQuestionsInputDto> Questions { get; set; }
    }
}
