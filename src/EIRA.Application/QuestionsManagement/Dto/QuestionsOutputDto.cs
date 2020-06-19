namespace EIRA.QuestionsManagement.Dto
{
    public class QuestionsOutputDto
    {
        /// <summary>
        /// Question Type Id
        /// </summary>
        public int? QuestionTypeId { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// Question Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Question Code（頁面顯示的Question Id）
        /// </summary>
        public string QuestionCode { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        public string  Status { get; set; }
    }
}
