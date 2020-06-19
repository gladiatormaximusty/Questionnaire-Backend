using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EIRA.Authorization.Users;
using EIRA.Common;
using EIRA.Dto;
using EIRA.Enums;
using EIRA.MyQuestionnaireManagement.Dto;
using EIRA.QuestionnaireManagement.Dto;
using EIRA.QuestionnairesIsDone;
using EIRA.QuestionnairesIsDone.Dto;
using EIRA.ResultDto;
using EIRA.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace EIRA.QuestionnaireManagement
{
    [AbpAuthorize]
    public class MyQuestionnaireAppService : EIRAAppServiceBase, IMyQuestionnaireAppService
    {
        IRepository<Questionnaires> _questionnairesRepository;
        IRepository<QuestionnairesAsign> _questionnairesAsignRepository;
        IRepository<Questions> _questionsRepository;
        IRepository<QuestionsAnswer> _questionsAnswerRepository;
        IRepository<Entities> _entitiesRepository;
        IRepository<BUs> _bUsRepository;
        IRepository<QuestionTypes> _questionTypesRepository;
        IRepository<SupportingDocument> _supportingDocumentRepository;
        IRepository<QuestionnairesFinished> _questionnairesFinishedRepository;
        IRepository<User, long> _userRepository;
        QuestionnairesManager _questionnairesManager;
        IRepository<QuestionsAsign> _questionsAsignRepository;

        public MyQuestionnaireAppService(IRepository<Questionnaires> questionnairesRepository, IRepository<QuestionnairesAsign> questionnairesAsignRepository, IRepository<Questions> questionsRepository, IRepository<QuestionsAnswer> questionsAnswerRepository, IRepository<Entities> entitiesRepository, IRepository<BUs> bUsRepository, IRepository<QuestionTypes> questionTypesRepository
            , IRepository<SupportingDocument> supportingDocumentRepository, IRepository<QuestionnairesFinished> questionnairesFinishedRepository, IRepository<User, long> userRepository, QuestionnairesManager questionnairesManager
            , IRepository<QuestionsAsign> questionsAsignRepository)
        {
            _questionnairesRepository = questionnairesRepository;
            _questionnairesAsignRepository = questionnairesAsignRepository;
            _questionsRepository = questionsRepository;
            _questionsAnswerRepository = questionsAnswerRepository;
            _entitiesRepository = entitiesRepository;
            _bUsRepository = bUsRepository;
            _questionTypesRepository = questionTypesRepository;
            _supportingDocumentRepository = supportingDocumentRepository;
            _questionnairesFinishedRepository = questionnairesFinishedRepository;
            _userRepository = userRepository;
            _questionnairesManager = questionnairesManager;
            _questionsAsignRepository = questionsAsignRepository;
        }

        public ResultsDto<MyQuestionnaireStatusCountOutputDto> GetMyQuestionnaireStatusCount()
        {
            ResultsDto<MyQuestionnaireStatusCountOutputDto> resultDto = new ResultsDto<MyQuestionnaireStatusCountOutputDto>();

            try
            {
                MyQuestionnaireStatusCountOutputDto myQuestionnaireStatusCountOutputDto = new MyQuestionnaireStatusCountOutputDto();

                var query = _questionnairesRepository.GetAll();

                #region 找出當前使用者 BU 作爲查詢條件

                var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                int BUId = 0;

                if (userInfo != null && userInfo.BU_Id.HasValue)
                {
                    BUId = userInfo.BU_Id.Value;
                }

                List<int> questionnairesIds = _questionnairesAsignRepository.GetAll().Where(x => x.BU_Id == BUId).Select(x => x.Questionnaire_Id.Value).Distinct().ToList();

                query = query.Where(x => questionnairesIds.Contains(x.Id));

                #endregion

                myQuestionnaireStatusCountOutputDto.PendingQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Pending.ToString()).ToList().Count;
                myQuestionnaireStatusCountOutputDto.ReviewingQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Reviewing.ToString()).ToList().Count;
                myQuestionnaireStatusCountOutputDto.FinishedQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Finished.ToString()).ToList().Count;

                resultDto.Data = myQuestionnaireStatusCountOutputDto;

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        public ResultsDto<PagedResultDto<QuestionnaireOutputDto>> GetAll(MyQuestionnaireInputDto input)
        {
            ResultsDto<PagedResultDto<QuestionnaireOutputDto>> resultDto = new ResultsDto<PagedResultDto<QuestionnaireOutputDto>>();

            try
            {
                var query = from questionnaires in _questionnairesRepository.GetAll()
                            select new QuestionnaireOutputDto
                            {
                                Id = questionnaires.Id,
                                QuestionnaireName = questionnaires.QuestionnaireName,
                                SubmissionDeadline = questionnaires.SubmissionDeadline,
                                Status = questionnaires.Status,
                                RiskType = questionnaires.RiskType,
                                IsEdit = (questionnaires.Status != QuestionnairesStatus.Finished.ToString() && questionnaires.Status != QuestionnairesStatus.Reviewing.ToString())
                            };

                #region 找出當前使用者 BU 作爲查詢條件

                var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                int BUId = 0;

                if (userInfo != null && userInfo.BU_Id.HasValue)
                {
                    BUId = userInfo.BU_Id.Value;
                }

                List<int> questionnairesIds = _questionnairesAsignRepository.GetAll().Where(x => x.BU_Id == BUId).Select(x => x.Questionnaire_Id.Value).Distinct().ToList();

                query = query.Where(x => questionnairesIds.Contains(x.Id));

                #endregion

                //查詢條件
                query = query.WhereIf(!input.Status.IsNullOrWhiteSpace(), x => x.Status.ToUpper() == input.Status.ToUpper());

                //排序
                if (string.IsNullOrWhiteSpace(input.Sorting))
                {
                    input.Sorting = "SubmissionDeadline desc";
                }

                //排序
                query = query.OrderBy(input.Sorting);

                //记录总数
                var resultCount = query.Count();

                //分页
                query = query.PageBy(input);

                List<QuestionnaireOutputDto> QuestionnaireOutputDtoList = query.ToList();

                foreach (var item in QuestionnaireOutputDtoList)
                {
                    //計算問卷完成情況
                    List<CalQuestionnairesProgressEntity> CalQuestionnairesProgressEntityList = _questionnairesManager.GetCalProgressEntity(item.Id).Where(x => x.BUId == BUId).ToList();
                    if (CalQuestionnairesProgressEntityList.Any())
                    {
                        //*1.0:是因為int類型除以int類型會得出0
                        decimal Progress = (decimal)(CalQuestionnairesProgressEntityList.Where(x => x.IsDone).ToList().Count * 1.0 / CalQuestionnairesProgressEntityList.Count) * 100;

                        item.Progress = ExtensionHelper.Round(Progress, 0);
                    }
                }

                resultDto.Data = new PagedResultDto<QuestionnaireOutputDto>(resultCount, QuestionnaireOutputDtoList);

                resultDto.Status.Code = Succeed;

            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        /// <summary>
        /// 取得問卷中的所有 Question Type（admin 會傳 BUId，不是 admin 則取當前BUId）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResultsDto<List<MyQuestionnaireQuestionTypeDto>> GetQuestionnaireQuestionType(GetMyQuestionnaireInfoInputDto input)
        {
            ResultsDto<List<MyQuestionnaireQuestionTypeDto>> resultDto = new ResultsDto<List<MyQuestionnaireQuestionTypeDto>>();

            try
            {
                int BUId = input.BUId;

                //普通用戶，取得所屬的 BU，作為條件查詢
                if (input.BUId == 0)
                {
                    var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                    if (userInfo != null && userInfo.BU_Id.HasValue)
                    {
                        BUId = userInfo.BU_Id.Value;
                    }
                }

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.QuestionnaireId);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        List<int> questionIds = _questionnairesAsignRepository.GetAll()
                                                .Where(x => x.Questionnaire_Id == input.QuestionnaireId && x.BU_Id == BUId)
                                                .Select(x => x.Question_Id.Value).Distinct().ToList();

                        List<int> questionTypeIds = _questionsRepository.GetAll().Where(x => questionIds.Contains(x.Id) && x.Status != QuestionsStatus.InActive.ToString()).Select(x => x.QuestionType_Id.Value).Distinct().ToList();

                        var query = _questionTypesRepository.GetAll().Where(x => questionTypeIds.Contains(x.Id) && x.Status != QuestionTypesStatus.InActive.ToString());

                        resultDto.Data = ObjectMapper.Map<List<MyQuestionnaireQuestionTypeDto>>(query.ToList());

                        #endregion
                    }
                    else
                    {
                        //取得Finished Questionnaire Question Type 資料
                        resultDto.Data = GetFinishedQuestionnaireQuestionType(input.QuestionnaireId, BUId);
                    }
                }

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        /// <summary>
        /// 取得問卷中的所有 Entity（admin 會傳 BUId，不是 admin 則取當前BUId）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ResultsDto<List<MyQuestionnaireEntityDto>> GetQuestionnaireEntity(GetMyQuestionnaireInfoInputDto input)
        {
            ResultsDto<List<MyQuestionnaireEntityDto>> resultDto = new ResultsDto<List<MyQuestionnaireEntityDto>>();

            try
            {
                int BUId = input.BUId;

                if (input.BUId == 0)
                {
                    var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                    if (userInfo != null && userInfo.BU_Id.HasValue)
                    {
                        BUId = userInfo.BU_Id.Value;
                    }
                }

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.QuestionnaireId);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        //Questionnaire Status為Drafting、Pending、Reviewing的資料查詢
                        var questionIds = _questionsRepository.GetAll().Where(x => x.QuestionType_Id == input.QuestionTypeId).Select(x => x.Id).ToList();

                        List<int> entityIds = _questionnairesAsignRepository.GetAll()
                            .Where(x => x.Questionnaire_Id == input.QuestionnaireId && x.BU_Id == BUId && questionIds.Contains(x.Question_Id.Value))
                            .Select(x => x.Entity_Id.Value).Distinct().ToList();

                        var query = _entitiesRepository.GetAll().Where(x => entityIds.Contains(x.Id) && x.Status != EntitiesStatus.InActive.ToString());

                        resultDto.Data = ObjectMapper.Map<List<MyQuestionnaireEntityDto>>(query.ToList());
                    }
                    else
                    {
                        //取得 Finished Questionnaire Entity 資料
                        resultDto.Data = GetFinishedQuestionnaireEntity(input.QuestionnaireId, BUId, input.QuestionTypeId);
                    }
                }

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        public ResultsDto<MyQuestionnairesEditOutputDto> GetQuestionnaireInfo(GetMyQuestionnaireInfoInputDto input)
        {
            ResultsDto<MyQuestionnairesEditOutputDto> resultDto = new ResultsDto<MyQuestionnairesEditOutputDto>();

            try
            {
                List<MyQuestionnaireEditDto> myQuestionnaireEditDto = new List<MyQuestionnaireEditDto>();

                int BUId = input.BUId;

                if (input.BUId == 0)
                {
                    var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                    if (userInfo != null && userInfo.BU_Id.HasValue)
                    {
                        BUId = userInfo.BU_Id.Value;
                    }
                }

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.QuestionnaireId);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        var query = from questionnairesAsign in _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.QuestionnaireId && x.BU_Id == BUId && x.Entity_Id == input.EntitiesId)
                                    join questions in _questionsRepository.GetAll().Where(x => x.Status != QuestionsStatus.InActive.ToString()) on questionnairesAsign.Question_Id equals questions.Id
                                    join questionTypes in _questionTypesRepository.GetAll().Where(x => x.Id == input.QuestionTypeId && x.Status != QuestionTypesStatus.InActive.ToString()) on questions.QuestionType_Id equals questionTypes.Id
                                    select new MyQuestionnaireEditDto
                                    {
                                        QuestionnairesAsignId = questionnairesAsign.Id,
                                        FreeText = questionnairesAsign.FreeText,
                                        SelectedAnswer = questionnairesAsign.SelectedAnswer,
                                        QuestionId = questions.Id,
                                        Question = questions.Question,
                                        QuestionCode = questions.QuestionCode,
                                        FreeTextPlaceholder = questions.FreeTextPlaceholder == null ? "" : questions.FreeTextPlaceholder,
                                        HasAnswer = questions.HasAnswer,
                                        IsAnswerMandatory = questions.IsAnswerMandatory,
                                        HasFreeText = questions.HasFreeText,
                                        IsFreeTextMandatory = questions.IsFreeTextMandatory,
                                        IsFreeTextNumeric = questions.IsFreeTextNumeric,
                                        HasSupportingDocument = questions.HasSupportingDocument,
                                        IsSupportingDocumentMandatory = questions.IsSupportingDocumentMandatory
                                    };

                        query = query.OrderBy("QuestionCode");

                        myQuestionnaireEditDto = query.ToList();

                        #region 找出所有符合條件的 Answer

                        List<int> QuestionIds = myQuestionnaireEditDto.Select(x => x.QuestionId).ToList();
                        var DBAnswerList = _questionsAnswerRepository.GetAll().Where(x => QuestionIds.Contains(x.Question_Id.Value) && x.Status != QuestionsAnswerStatus.InActive.ToString()).ToList();

                        #endregion

                        #region 找出所有符合條件的 Supporting Document

                        List<int> QuestionnairesAsignIds = myQuestionnaireEditDto.Select(x => x.QuestionnairesAsignId).ToList();
                        var DBSupportingDocumentList = _supportingDocumentRepository.GetAll().Where(x => QuestionnairesAsignIds.Contains(x.QuestionnairesAsign_Id)).ToList();

                        #endregion

                        foreach (var item in myQuestionnaireEditDto)
                        {
                            //取得對應的 Answer 資料
                            var DBAnswer = DBAnswerList.Where(x => x.Question_Id == item.QuestionId).ToList();
                            item.Answers = ObjectMapper.Map<List<MyQuestionnaireQuestionAnswerDto>>(DBAnswer);

                            if (!string.IsNullOrWhiteSpace(item.SelectedAnswer))
                            {
                                item.SelectedAnswerId = Convert.ToInt32(item.SelectedAnswer);
                            }

                            //取得對應的Supporting Document資料
                            var DBSupportingDocument = DBSupportingDocumentList.Where(x => x.QuestionnairesAsign_Id == item.QuestionnairesAsignId).ToList();
                            item.SupportingDocument = ObjectMapper.Map<List<MyQuestionnaireSupportingDocumentDto>>(DBSupportingDocument);

                            //判斷是否已經答題
                            if (!string.IsNullOrWhiteSpace(item.SelectedAnswer) || !string.IsNullOrWhiteSpace(item.FreeText) || DBSupportingDocument.Any())
                            {
                                item.HasBeenAnswered = true;
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        //取得 Finished Questionnaire 資料
                        myQuestionnaireEditDto = GetFinishedQuestionnaireInfo(input.QuestionnaireId, BUId, input.QuestionTypeId, input.EntitiesId);
                    }

                    MyQuestionnairesEditOutputDto myQuestionnairesEditOutputDto = new MyQuestionnairesEditOutputDto();

                    myQuestionnairesEditOutputDto.Questionnaire = myQuestionnaireEditDto;

                    myQuestionnairesEditOutputDto.QuestionCount = myQuestionnaireEditDto.Count;

                    myQuestionnairesEditOutputDto.HasBeenAnsweredCount = myQuestionnaireEditDto.Where(x => x.HasBeenAnswered).Count();

                    myQuestionnairesEditOutputDto.IsReadOnly = (questionnaires.Status == QuestionnairesStatus.Finished.ToString() || questionnaires.Status == QuestionnairesStatus.Reviewing.ToString());

                    resultDto.Data = myQuestionnairesEditOutputDto;
                }

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        public ResultsDto<bool> UpdateMyQuestionnaire(List<MyQuestionnaireEditDto> input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            #region 必要項驗證

            List<string> ErrorMessage = new List<string>();

            foreach (var questionnairesAsignDto in input)
            {
                if (questionnairesAsignDto.IsAnswerMandatory && questionnairesAsignDto.SelectedAnswerId == 0)
                {
                    ErrorMessage.Add(string.Format("Question Id {0} Answer Is Mandatory", questionnairesAsignDto.QuestionCode));
                }

                if (questionnairesAsignDto.IsFreeTextMandatory && string.IsNullOrWhiteSpace(questionnairesAsignDto.FreeText))
                {
                    ErrorMessage.Add(string.Format("Question Id {0} Free Text Is Mandatory", questionnairesAsignDto.QuestionCode));
                }

                if (questionnairesAsignDto.IsFreeTextNumeric && !ExtensionHelper.IsNumberic(questionnairesAsignDto.FreeText))
                {
                    ErrorMessage.Add(string.Format("Question Id {0} Free Text Is Numeric", questionnairesAsignDto.QuestionCode));
                }

                if (questionnairesAsignDto.IsSupportingDocumentMandatory && !questionnairesAsignDto.SupportingDocument.Any())
                {
                    ErrorMessage.Add(string.Format("Question Id {0} Supporting Document Is Mandatory", questionnairesAsignDto.QuestionCode));
                }
            }

            if (ErrorMessage.Any())
            {
                resultDto.Data = false;
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = string.Join("\r\n", ErrorMessage);
            }

            #endregion

            try
            {
                var userInfo = _userRepository.FirstOrDefault(AbpSession.UserId.Value);

                int BUId = userInfo.BU_Id.Value;

                //找出 Questionnaires Id
                var QuestionnairesId = 0;
                if (input.Any())
                {
                    var DBQuestionnairesAsign = _questionnairesAsignRepository.FirstOrDefault(input.FirstOrDefault().QuestionnairesAsignId);
                    if (DBQuestionnairesAsign != null)
                    {
                        QuestionnairesId = DBQuestionnairesAsign.Questionnaire_Id.Value;
                    }
                }

                //取得所有的 questions Asign
                var questionsAsignList = _questionsAsignRepository.GetAllList(x => x.Questionnaire_Id == QuestionnairesId);

                foreach (var questionnairesAsignDto in input)
                {
                    var UpdateQuestionnairesAsignList = new List<QuestionnairesAsign>();

                    var DBQuestionsAsign = questionsAsignList.FirstOrDefault(x => x.Question_Id == questionnairesAsignDto.QuestionId);

                    if (DBQuestionsAsign != null)
                    {
                        //若同一個問題的 Single Answer 設置為true
                        if (DBQuestionsAsign.SingleAnswer)
                        {
                            //更新 BU 在該 Questionnaires 下同一個問題的所有 Entities 的所有 Questionnaires Asign
                            UpdateQuestionnairesAsignList = _questionnairesAsignRepository.GetAll()
                                                        .Where(x => x.BU_Id == BUId && x.Questionnaire_Id == DBQuestionsAsign.Questionnaire_Id && x.Question_Id == DBQuestionsAsign.Question_Id)
                                                        .ToList();
                        }
                        else
                        {
                            //只更新一條記錄
                            var DBQuestionnairesAsign = _questionnairesAsignRepository.FirstOrDefault(questionnairesAsignDto.QuestionnairesAsignId);

                            UpdateQuestionnairesAsignList.Add(DBQuestionnairesAsign);
                        }
                    }

                    foreach (var questionnairesAsign in UpdateQuestionnairesAsignList)
                    {
                        #region 更新 Questionnaires Asign 資料

                        questionnairesAsign.FreeText = questionnairesAsignDto.FreeText;

                        if (questionnairesAsignDto.SelectedAnswerId != 0)
                        {
                            questionnairesAsign.SelectedAnswer = questionnairesAsignDto.SelectedAnswerId.ToString();
                        }
                        else
                        {
                            questionnairesAsign.SelectedAnswer = "";
                        }

                        questionnairesAsign.AnswerUserId = AbpSession.UserId;
                        questionnairesAsign.AnswerTime = DateTime.Now;

                        _questionnairesAsignRepository.Update(questionnairesAsign);

                        #endregion

                        #region 更新 Supporting Document 資料

                        //刪除舊所有的 Supporting Document (由於要更新所有 entities ，當前 supporting document id 與其他 id 不一樣，因為要同步，只能刪除後再添加)
                        _supportingDocumentRepository.Delete(x => x.QuestionnairesAsign_Id == questionnairesAsign.Id);

                        if (questionnairesAsignDto.SupportingDocument != null)
                        {
                            foreach (var item in questionnairesAsignDto.SupportingDocument)
                            {
                                var supportingDocument = new SupportingDocument();
                                supportingDocument.QuestionnairesAsign_Id = questionnairesAsign.Id;
                                supportingDocument.Name = item.Name;
                                supportingDocument.PathUrl = item.PathUrl;

                                //加入新的Supporting Document
                                _supportingDocumentRepository.Insert(supportingDocument);
                            }
                        }

                        #endregion
                    }
                }

                resultDto.Data = true;

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        #region Finished Questionnaires info

        /// <summary>
        /// 取得 Finished Questionnaire Question Type 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire Id</param>
        /// <param name="BUId">BU Id</param>
        /// <returns></returns>
        private List<MyQuestionnaireQuestionTypeDto> GetFinishedQuestionnaireQuestionType(int QuestionnaireId, int BUId)
        {
            List<MyQuestionnaireQuestionTypeDto> myQuestionnaireQuestionTypeDtoList = new List<MyQuestionnaireQuestionTypeDto>();

            #region Questionnaire Status為Finished的資料

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId && x.BUId == BUId).ToList();

            List<int> QuestionTypeIds = new List<int>();

            foreach (var item in questionnairesFinished)
            {
                #region 取出 BU 下所有的 Entities下的所有Question的Question Type 資料

                if (!string.IsNullOrWhiteSpace(item.EntityQuestionsJson))
                {
                    var EntityQuestionsList = JsonConvert.DeserializeObject<List<QuestionnairesFinishedEntityQuestionsDto>>(item.EntityQuestionsJson);

                    if (EntityQuestionsList != null && EntityQuestionsList.Any())
                    {
                        foreach (var EntityQuestions in EntityQuestionsList)
                        {
                            if (EntityQuestions.Questions != null && EntityQuestions.Questions.Any())
                            {
                                foreach (var question in EntityQuestions.Questions)
                                {
                                    if (!QuestionTypeIds.Contains(question.QuestionType_Id.Value))
                                    {
                                        QuestionTypeIds.Add(question.QuestionType_Id.Value);

                                        MyQuestionnaireQuestionTypeDto myQuestionnaireQuestionTypeDto = new MyQuestionnaireQuestionTypeDto();
                                        myQuestionnaireQuestionTypeDto.Id = question.QuestionType_Id.Value;
                                        myQuestionnaireQuestionTypeDto.QuestionTypeName = question.QuestionTypeName;

                                        myQuestionnaireQuestionTypeDtoList.Add(myQuestionnaireQuestionTypeDto);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            return myQuestionnaireQuestionTypeDtoList;
        }

        /// <summary>
        /// 取得 Finished Questionnaire Entity 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire Id</param>
        /// <param name="BUId">BU Id</param>
        /// <param name="QuestionTypeId">QuestionType Id</param>
        /// <returns></returns>
        private List<MyQuestionnaireEntityDto> GetFinishedQuestionnaireEntity(int QuestionnaireId, int BUId, int QuestionTypeId)
        {
            List<MyQuestionnaireEntityDto> myQuestionnaireEntityDtoList = new List<MyQuestionnaireEntityDto>();

            List<int> EntitiesIds = new List<int>();

            #region Questionnaire Status為Finished的資料查詢

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId && x.BUId == BUId).ToList();

            foreach (var item in questionnairesFinished)
            {
                #region 取出 BU 下所有的 Entities 資料

                if (!string.IsNullOrWhiteSpace(item.EntityQuestionsJson))
                {
                    var EntityQuestionsList = JsonConvert.DeserializeObject<List<QuestionnairesFinishedEntityQuestionsDto>>(item.EntityQuestionsJson);

                    if (EntityQuestionsList != null && EntityQuestionsList.Any())
                    {
                        foreach (var EntityQuestions in EntityQuestionsList)
                        {
                            var Entity = EntityQuestions.Questions.FirstOrDefault(x => x.QuestionType_Id == QuestionTypeId);

                            if (Entity != null && !EntitiesIds.Contains(EntityQuestions.Id))
                            {
                                EntitiesIds.Add(EntityQuestions.Id);

                                MyQuestionnaireEntityDto myQuestionnaireEntityDto = new MyQuestionnaireEntityDto();

                                myQuestionnaireEntityDto.Id = EntityQuestions.Id;
                                myQuestionnaireEntityDto.EntityName = EntityQuestions.EntityName;
                                myQuestionnaireEntityDtoList.Add(myQuestionnaireEntityDto);
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            return myQuestionnaireEntityDtoList;
        }

        /// <summary>
        /// 取得 Finished Questionnaire 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire Id</param>
        /// <param name="BUId">BU Id</param>
        /// <param name="QuestionTypeId">QuestionType Id</param>
        /// <param name="EntitiesId">Entities Id</param>
        /// <returns></returns>
        private List<MyQuestionnaireEditDto> GetFinishedQuestionnaireInfo(int QuestionnaireId, int BUId, int QuestionTypeId, int EntitiesId)
        {
            List<MyQuestionnaireEditDto> myQuestionnaireForEditOutputDtoList = new List<MyQuestionnaireEditDto>();

            #region Questionnaire Status為Finished的資料

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId && x.BUId == BUId).ToList();

            foreach (var item in questionnairesFinished)
            {
                #region 取出 BU 下所有的 Entities下的所有Question 資料

                if (!string.IsNullOrWhiteSpace(item.EntityQuestionsJson))
                {
                    var EntityQuestionsList = JsonConvert.DeserializeObject<List<QuestionnairesFinishedEntityQuestionsDto>>(item.EntityQuestionsJson);

                    if (EntityQuestionsList != null && EntityQuestionsList.Any())
                    {
                        foreach (var EntityQuestions in EntityQuestionsList.Where(x => x.Id == EntitiesId))
                        {
                            if (EntityQuestions.Questions != null && EntityQuestions.Questions.Any())
                            {
                                foreach (var question in EntityQuestions.Questions.Where(x => x.QuestionType_Id == QuestionTypeId))
                                {
                                    MyQuestionnaireEditDto myQuestionnaireEditDto = new MyQuestionnaireEditDto();
                                    myQuestionnaireEditDto.QuestionnairesAsignId = question.QuestionnairesAsignId;
                                    myQuestionnaireEditDto.QuestionId = question.Id;
                                    myQuestionnaireEditDto.QuestionCode = question.QuestionCode;
                                    myQuestionnaireEditDto.Question = question.Question;
                                    myQuestionnaireEditDto.FreeText = question.FreeText;
                                    myQuestionnaireEditDto.HasAnswer = question.HasAnswer;
                                    myQuestionnaireEditDto.IsAnswerMandatory = question.IsAnswerMandatory;
                                    myQuestionnaireEditDto.HasFreeText = question.HasFreeText;
                                    myQuestionnaireEditDto.FreeTextPlaceholder = question.FreeTextPlaceholder;
                                    myQuestionnaireEditDto.IsFreeTextMandatory = question.IsFreeTextMandatory;
                                    myQuestionnaireEditDto.HasSupportingDocument = question.HasSupportingDocument;
                                    myQuestionnaireEditDto.IsSupportingDocumentMandatory = question.IsSupportingDocumentMandatory;
                                    myQuestionnaireEditDto.SelectedAnswer = question.SelectedAnswer;

                                    if (!string.IsNullOrWhiteSpace(question.SelectedAnswer))
                                    {
                                        myQuestionnaireEditDto.SelectedAnswerId = Convert.ToInt32(question.SelectedAnswer);
                                    }

                                    #region 取得對應的Answer資料

                                    myQuestionnaireEditDto.Answers = new List<MyQuestionnaireQuestionAnswerDto>();

                                    if (question.QuestionsAnswers != null && question.QuestionsAnswers.Any())
                                    {
                                        foreach (var Answer in question.QuestionsAnswers)
                                        {
                                            MyQuestionnaireQuestionAnswerDto AnswerDto = new MyQuestionnaireQuestionAnswerDto();

                                            AnswerDto.Id = Answer.Id;
                                            AnswerDto.AnswerContent = Answer.AnswerContent;

                                            myQuestionnaireEditDto.Answers.Add(AnswerDto);
                                        }
                                    }

                                    #endregion

                                    #region 取得對應的Supporting Document資料

                                    myQuestionnaireEditDto.SupportingDocument = new List<MyQuestionnaireSupportingDocumentDto>();

                                    if (question.SupportingDocument != null && question.SupportingDocument.Any())
                                    {
                                        foreach (var supportingDocument in question.SupportingDocument)
                                        {
                                            MyQuestionnaireSupportingDocumentDto SupportingDocumentDto = new MyQuestionnaireSupportingDocumentDto();
                                            SupportingDocumentDto.Id = supportingDocument.Id;
                                            SupportingDocumentDto.Name = supportingDocument.Name;
                                            SupportingDocumentDto.PathUrl = supportingDocument.PathUrl;

                                            myQuestionnaireEditDto.SupportingDocument.Add(SupportingDocumentDto);
                                        }
                                    }

                                    #endregion

                                    if (!string.IsNullOrWhiteSpace(question.SelectedAnswer) || !string.IsNullOrWhiteSpace(question.FreeText) || myQuestionnaireEditDto.SupportingDocument.Any())
                                    {
                                        myQuestionnaireEditDto.HasBeenAnswered = true;
                                    }

                                    myQuestionnaireForEditOutputDtoList.Add(myQuestionnaireEditDto);
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            return myQuestionnaireForEditOutputDtoList;
        }

        #endregion
    }
}
