using Abp.Application.Services.Dto;

namespace EIRA.QuestionTypesManagement.Dto
{
    public class QuestionTypesInputDto : PagedResultRequestDto, ISortedResultRequest
    {
        /// <summary>
        /// Question Types Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sorting { get; set; }

        /// <summary>
        /// 是否只顯示Active Status資料
        /// </summary>
        public bool IsOnlyShowActiveStatus { get; set; }
    }
}
