using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
   public class AsignEntitiesInputDto 
    {
        /// <summary>
        /// Entities Id
        /// </summary>
        public int EntitiesId { get; set; }

        public List<AsignBUsInputDto> BUs { get; set; }
    }
}
