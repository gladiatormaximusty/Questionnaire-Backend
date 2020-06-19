using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.EntitiesManagement.Dto
{
    [AutoMap(typeof(Entities))]
    public class EntitiesDto : EntityDto
    {
        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }
    }
}
