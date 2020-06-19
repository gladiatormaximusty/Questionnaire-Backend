using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.QuestionsManagement.Dto;
using EIRA.ResultDto;
using System.Web.Http;

namespace EIRA.QuestionsManagement
{
    public interface IQuestionsAppService : IApplicationService
    {
        /// <summary>
        /// 取得所有的Question資料
        /// </summary>
        /// <param name="input">查詢條件</param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<QuestionsOutputDto>> GetAll(QuestionsInputDto input);

        /// <summary>
        /// 更新Questions資料為InActive
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<bool> Delete(NullableIdDto input);

        /// <summary>
        /// 根據Id獲得對應的Questions資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        ResultsDto<QuestionsDto> GetQuestionsById(NullableIdDto input);

        /// <summary>
        /// 更新Questions資料
        /// </summary>
        /// <param name="input">Questions 資料</param>
        /// <returns></returns>
        ResultsDto<bool> InsertOrUpdate(QuestionsDto input);
    }
}
