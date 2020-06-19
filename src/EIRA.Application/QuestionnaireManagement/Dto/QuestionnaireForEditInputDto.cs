using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionnaireForEditInputDto
    {
        /// <summary>
        /// Questionnaire Info
        /// </summary>
        public QuestionnaireDto Questionnaire { get; set; }

        public List<AsignQuestionTypeDto> AsignQuestionType { get; set; }
    }
}
