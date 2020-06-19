using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.MyQuestionnaireManagement.Dto;
using EIRA.QuestionnaireManagement.Dto;
using EIRA.ResultDto;
using System.Collections.Generic;

namespace EIRA.QuestionnaireManagement
{
    public interface IMyQuestionnaireAppService : IApplicationService
    {
        /// <summary>
        /// 取得Questionnaire Status Count
        /// </summary>
        /// <returns></returns>
        ResultsDto<MyQuestionnaireStatusCountOutputDto> GetMyQuestionnaireStatusCount();

        /// <summary>
        /// 取得用戶(部門的)所有Questionnaire資料
        /// </summary>
        /// <param name="input">查詢條件</param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<QuestionnaireOutputDto>> GetAll(MyQuestionnaireInputDto input);

        /// <summary>
        /// 根據Questionnaire Id、BUId(admin會傳BUId)獲得對應的Question Type資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<List<MyQuestionnaireQuestionTypeDto>> GetQuestionnaireQuestionType(GetMyQuestionnaireInfoInputDto input);

        /// <summary>
        /// 根據Questionnaire Id、BUId(admin要傳BUId)、Question Type Id獲得對應的Entities資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<List<MyQuestionnaireEntityDto>> GetQuestionnaireEntity(GetMyQuestionnaireInfoInputDto input);

        /// <summary>
        /// 根據Questionnaire Id、Entity Id、Question Type Id、(admin會傳BUId)獲得對應的Questionnaire資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<MyQuestionnairesEditOutputDto> GetQuestionnaireInfo(GetMyQuestionnaireInfoInputDto input);

        /// <summary>
        /// 更新Questionnaire資料
        /// </summary>
        /// <param name="input">Questionnaire 資料</param>
        /// <returns></returns>
        ResultsDto<bool> UpdateMyQuestionnaire(List<MyQuestionnaireEditDto> input);
    }
}
