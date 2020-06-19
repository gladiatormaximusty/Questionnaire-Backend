using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.QuestionTypesManagement.Dto;
using EIRA.ResultDto;
using System.Collections.Generic;
using System.Web.Http;

namespace EIRA.QuestionTypesManagement
{
    public interface IQuestionTypesAppService : IApplicationService
    {
        /// <summary>
        /// 取得所有的QuestionTypes資料(status==Active)(其它模塊使用)
        /// </summary>
        /// <returns></returns>
        ResultsDto<List<QuestionTypeAllOutputDto>> GetAll();

        /// <summary>
        /// 取得所有的QuestionTypes資料(分頁查詢)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<QuestionTypesOutputDto>> GetPagedAll(QuestionTypesInputDto input);

        /// <summary>
        /// 根據Id獲得對應的QuestionTypes資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        ResultsDto<QuestionTypesForEditOutputDto> GetQuestionTypesById(NullableIdDto input);

        /// <summary>
        /// 更新QuestionTypes資料
        /// </summary>
        /// <param name="input">QuestionTypes 資料</param>
        /// <returns></returns>
        ResultsDto<NullableIdDto> InsertOrUpdate(QuestionTypesDto input);

        /// <summary>
        /// 更新QuestionTypes資料為InActive
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<bool> Delete(NullableIdDto input);
    }
}
