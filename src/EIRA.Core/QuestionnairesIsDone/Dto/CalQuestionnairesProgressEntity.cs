namespace EIRA.QuestionnairesIsDone.Dto
{
    public class CalQuestionnairesProgressEntity
    {
        public int QuestionnairesId { get; set; }

        public int questionnairesAsignId { get; set; }

        public int BUId { get; set; }
        public int EntitiesId { get; set; }
       
        public bool HasAnswer { get; set; }
        public bool IsAnswerMandatory { get; set; }

        public bool HasFreeText { get; set; }
        public bool IsFreeTextMandatory { get; set; }
        public bool HasSupportingDocument { get; set; }
        public bool IsSupportingDocumentMandatory { get; set; }
        public string SelectedAnswer { get; set; }
        public string FreeText { get; set; }
        public int SupportingDocumentCount { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone { get; set; }
    }
}
