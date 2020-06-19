using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    [AutoMap(typeof(SupportingDocument))]
    public class MyQuestionnaireSupportingDocumentDto : EntityDto
    {
        /// <summary>
        /// Supporting Document Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// PathUrl
        /// </summary>
        public string PathUrl { get; set; }
    }
}
