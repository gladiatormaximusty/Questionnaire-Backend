using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class AsignQuestionsInputDto
    {
        public int QuestionId { get; set; }

        /// <summary>
        /// Single Answer
        /// </summary>
        public bool SingleAnswer { get; set; }

        public List<AsignEntitiesInputDto> Entities { get; set; }
    }
}
