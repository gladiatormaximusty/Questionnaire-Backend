using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EIRA.Table;

namespace EIRA.BUsManagement.Dto
{
    [AutoMap(typeof(BUs))]
    public class BUsOutputDto : EntityDto
    {
        /// <summary>
        /// BU Name
        /// </summary>
        public string BUName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }
    }
}
