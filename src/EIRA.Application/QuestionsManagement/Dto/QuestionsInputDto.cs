using Abp.Application.Services.Dto;

namespace EIRA.QuestionsManagement.Dto
{
    public class QuestionsInputDto : PagedResultRequestDto, ISortedResultRequest
    {
        /// <summary>
        /// Question Type
        /// </summary>
        public int QuestionTypeId { get; set; }

        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Sorting
        /// </summary>
        public string Sorting { get; set; }

        public string Status { get; set; }
    }
}
