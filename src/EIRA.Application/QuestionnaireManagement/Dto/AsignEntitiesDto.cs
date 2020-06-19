using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class AsignEntitiesDto
    {
        /// <summary>
        /// Entities Id
        /// </summary>
        public int EntitiesId { get; set; }

        /// <summary>
        /// Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// 是否更改BU(給前端使用)
        /// </summary>
        public bool IsChangeBU { get; set; }

        /// <summary>
        /// Questionnaire Asign Entities BUs
        /// </summary>
        public List<QuestionnaireAsignEntitiesBUsDto> BUs { get; set; }
    }
}
