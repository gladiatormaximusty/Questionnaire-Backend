using EIRA.EntitiesManagement.Dto;
using EIRA.Table;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class AsignDto
    {
        /// <summary>
        /// Entities info
        /// </summary>
        public List<EntitiesDto> Entities { get; set; }

        /// <summary>
        /// Asign info
        /// </summary>
        public List<AsignQuestionTypeDto> AsignQuestionType { get; set; }
    }
}
