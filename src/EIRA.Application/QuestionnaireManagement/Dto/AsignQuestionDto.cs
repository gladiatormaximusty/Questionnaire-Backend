using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class AsignQuestionDto
    {
        /// <summary>
        /// Question Id
        /// </summary>
        public int QuestionId { get; set; }

        /// <summary>
        /// Question Code（頁面顯示的Question Id）
        /// </summary>
        public string QuestionCode { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Single Answer
        /// </summary>
        public bool SingleAnswer { get; set; }

        /// <summary>
        /// Entities
        /// </summary>
        public List<AsignEntitiesDto> Entities { get; set; }
    }
}
