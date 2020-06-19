using Abp.Application.Services.Dto;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionnaireAsignEntitiesBUsDto
    {
        public int QuestionnaireAsignId { get; set; }

        public int BUId { get; set; }

        /// <summary>
        /// BU Name
        /// </summary>
        public string BUName { get; set; }

        public bool IsSeleted { get; set; }
    }
}
