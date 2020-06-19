using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class ControlRiskQuestionnaireDto
    {
        public int Id { get; set; }

        public string BUName { get; set; }

        public List<CRQBUsEntitiesDto> Entities { get; set; }

        public decimal TotalProgress { get; set; }
    }
}
