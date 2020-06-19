using Abp.Application.Services.Dto;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class SupportingDocumentInputDto : PagedResultRequestDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Entity Id
        /// </summary>
        public int EntitiesId { get; set; }

        /// <summary>
        /// BU Id
        /// </summary>
        public int BUId { get; set; }

        /// <summary>
        /// Question Type Id
        /// </summary>
        public int QuestionTypeId { get; set; }
    }
}
