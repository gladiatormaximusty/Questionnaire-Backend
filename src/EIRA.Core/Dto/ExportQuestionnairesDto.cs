namespace EIRA.Dto
{
    public class ExportQuestionnairesDto
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// BU Name
        /// </summary>
        public string BU { get; set; }

        /// <summary>
        /// Question Type Code
        /// </summary>
        public string QuestionTypeId { get; set; }

        /// <summary>
        /// Question Type
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// Question Code
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Highest Rating
        /// </summary>
        public string HighestRating { get; set; }

        /// <summary>
        /// MC Answer
        /// </summary>
        public string MCAnswer { get; set; }

        /// <summary>
        /// Recommended Score
        /// </summary>
        public string RecommendedScore { get; set; }

        /// <summary>
        /// Free Text Answer
        /// </summary>
        public string FreeTextAnswer { get; set; }

        /// <summary>
        /// No. of supporting document(s)
        /// </summary>
        public string NomberOfSupportingDocument { get; set; }
    }

}
