using Abp.Application.Services.Dto;

namespace EIRA.QuestionnaireManagement.Dto
{
    public class QuestionnaireInputDto : PagedResultRequestDto, ISortedResultRequest
    {
        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 問卷名稱
        /// </summary>
        public string QuestionnaireName { get; set; }

        /// <summary>
        /// Sorting
        /// </summary>
        public string Sorting { get; set; }
    }
}
