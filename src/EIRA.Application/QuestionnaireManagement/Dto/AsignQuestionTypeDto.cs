using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class AsignQuestionTypeDto
    {
        /// <summary>
        /// Question Type Id
        /// </summary>
        public int QuestionTypeId { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// Questions
        /// </summary>
        public List<AsignQuestionDto> Questions { get; set; }
    }
}
