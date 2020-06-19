using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    [AutoMap(typeof(Entities))]
    public class MyQuestionnaireEntityDto : EntityDto
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }
    }
}
