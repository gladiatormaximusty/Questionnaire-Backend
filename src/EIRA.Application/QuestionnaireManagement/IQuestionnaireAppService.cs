using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EIRA.BUsManagement.Dto;
using EIRA.Dto;
using EIRA.EntitiesManagement.Dto;
using EIRA.QuestionnaireManagement.Dto;
using EIRA.ResultDto;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace EIRA.QuestionnaireManagement
{
    public interface IQuestionnaireAppService : IApplicationService
    {
        /// <summary>
        /// 取得Questionnaire Status Count
        /// </summary>
        /// <returns></returns>
        ResultsDto<QuestionnaireStatusCountOutputDto> GetQuestionnaireStatusCount();

        /// <summary>
        /// 取得所有的Questionnaire資料
        /// </summary>
        /// <param name="input">查詢條件</param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<QuestionnaireOutputDto>> GetAll(QuestionnaireInputDto input);

        /// <summary>
        /// 根據Id獲得對應的Questionnaire資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        ResultsDto<QuestionnaireDto> GetQuestionnaireById(NullableIdDto input);

        /// <summary>
        /// 獲得對應的Questionnaire Asign資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ResultsDto<AsignDto>> GetQuestionnaireAsign(QuestionnaireAsignInputDto input);

        /// <summary>
        /// 更新Questionnaire資料
        /// </summary>
        /// <param name="input">Questionnaire 資料</param>
        /// <returns></returns>
        ResultsDto<bool> InsertOrUpdate(QuestionnaireForEditInputDto input);

        /// <summary>
        /// 根據Questionnaire Id獲得對應的Question Type資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<List<QuestionnaireQuestionTypeDto>> GetQuestionnaireQuestionType(NullableIdDto input);

        /// <summary>
        /// 根據Questionnaire Id獲得對應的BUs資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<List<BUsDto>> GetQuestionnaireBUs(NullableIdDto input);

        /// <summary>
        /// Get Control Risk Questionnaire Entity
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<List<EntitiesDto>> GetQuestionnaireEntity(NullableIdDto input);

        /// <summary>
        /// Get Control Risk Questionnaire
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<ControlRiskQuestionnaireDto>> GetControlRiskQuestionnaire(ControlRiskQuestionnaireInputDto input);

        /// <summary>
        /// 取得Supporting Document資料
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<PagedResultDto<SupportingDocumentOutputDto>> GetSupportingDocument(SupportingDocumentInputDto input);

        /// <summary>
        /// 導出Control Risk Questionnaire
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ResultsDto<string> ExportQuestionnaire(NullableIdDto input);
    }
}
