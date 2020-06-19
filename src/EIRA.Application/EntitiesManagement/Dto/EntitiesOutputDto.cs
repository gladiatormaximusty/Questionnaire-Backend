using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.EntitiesManagement.Dto
{
    [AutoMap(typeof(Entities))]
    public class EntitiesOutputDto : EntityDto
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
    }
}
