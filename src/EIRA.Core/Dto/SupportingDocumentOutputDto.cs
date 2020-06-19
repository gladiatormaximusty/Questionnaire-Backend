using Abp.Application.Services.Dto;

namespace EIRA.Dto
{
    public class SupportingDocumentOutputDto : EntityDto
    {
        public string BUName { get; set; }

        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// Question Code（頁面顯示的Question Id）
        /// </summary>
        public string QuestionCode { get; set; }

        /// <summary>
        /// Document Name
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// PathUrl
        /// </summary>
        public string PathUrl { get; set; }

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
