namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionnaireStatusCountOutputDto
    {
        public int PendingQuestionnaireCount { get; set; }

        public int ReviewingQuestionnaireCount { get; set; }

        public int FinishedQuestionnaireCount { get; set; }

        public int DraftingQuestionnaireCount { get; set; }
    }
}
