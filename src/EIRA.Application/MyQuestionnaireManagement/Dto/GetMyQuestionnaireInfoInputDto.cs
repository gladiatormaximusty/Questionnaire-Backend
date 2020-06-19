namespace EIRA.MyQuestionnaireManagement.Dto
{
    public class GetMyQuestionnaireInfoInputDto
    {
        public int QuestionnaireId { get; set; }

        public int EntitiesId { get; set; }

        public int QuestionTypeId { get; set; }

        /// <summary>
        /// admin 進來會傳入BUId
        /// </summary>
        public int BUId { get; set; }
    }
}
