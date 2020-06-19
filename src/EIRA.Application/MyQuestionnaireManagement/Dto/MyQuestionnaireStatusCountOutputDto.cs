namespace EIRA.MyQuestionnaireManagement.Dto
{
    public class MyQuestionnaireStatusCountOutputDto
    {
        public int PendingQuestionnaireCount { get; set; }

        public int ReviewingQuestionnaireCount { get; set; }

        public int FinishedQuestionnaireCount { get; set; }
    }
}
