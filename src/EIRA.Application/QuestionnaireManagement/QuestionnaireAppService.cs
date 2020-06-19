using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using EIRA.Authorization;
using EIRA.BUsManagement.Dto;
using EIRA.Common;
using EIRA.Dto;
using EIRA.EntitiesManagement.Dto;
using EIRA.Enums;
using EIRA.IRepositories;
using EIRA.QuestionnaireManagement.Dto;
using EIRA.QuestionnairesIsDone;
using EIRA.QuestionnairesIsDone.Dto;
using EIRA.QuestionTypesManagement.Dto;
using EIRA.ResultDto;
using EIRA.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace EIRA.QuestionnaireManagement
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class QuestionnaireAppService : EIRAAppServiceBase, IQuestionnaireAppService
    {
        IRepository<Questionnaires> _questionnairesRepository;
        IEIRARepository<QuestionnairesAsign> _questionnairesAsignRepository;
        IRepository<Questions> _questionsRepository;
        IRepository<QuestionsAnswer> _questionsAnswerRepository;
        IRepository<Entities> _entitiesRepository;
        IRepository<BUs> _bUsRepository;
        IRepository<QuestionTypes> _questionTypesRepository;
        IRepository<SupportingDocument> _supportingDocumentRepository;
        IRepository<QuestionnairesFinished> _questionnairesFinishedRepository;
        IEIRARepository<QuestionsAsign> _questionsAsignRepository;

        QuestionnairesManager _questionnairesManager;

        public QuestionnaireAppService(IRepository<Questionnaires> questionnairesRepository, IEIRARepository<QuestionnairesAsign> questionnairesAsignRepository, IRepository<Questions> questionsRepository
            , IRepository<QuestionsAnswer> questionsAnswerRepository, IRepository<Entities> entitiesRepository, IRepository<BUs> bUsRepository, IRepository<QuestionTypes> questionTypesRepository
            , IRepository<SupportingDocument> supportingDocumentRepository, IRepository<QuestionnairesFinished> questionnairesFinishedRepository, IEIRARepository<QuestionsAsign> questionsAsignRepository
            , QuestionnairesManager questionnairesManager)
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
            _questionsAsignRepository = questionsAsignRepository;
            _questionnairesManager = questionnairesManager;
        }

        public ResultsDto<PagedResultDto<QuestionnaireOutputDto>> GetAll(QuestionnaireInputDto input)
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
                                IsEdit = input.Status != QuestionnairesStatus.Finished.ToString()
                            };

                //查詢條件
                query = query.WhereIf(!input.Status.IsNullOrWhiteSpace(), x => x.Status.ToUpper() == input.Status.ToUpper());
                query = query.WhereIf(!input.QuestionnaireName.IsNullOrWhiteSpace(), x => x.QuestionnaireName.Contains(input.QuestionnaireName.Trim()));

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
                    List<CalQuestionnairesProgressEntity> CalQuestionnairesProgressEntityList = _questionnairesManager.GetCalProgressEntity(item.Id);

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

        public ResultsDto<QuestionnaireDto> GetQuestionnaireById(NullableIdDto input)
        {
            ResultsDto<QuestionnaireDto> resultDto = new ResultsDto<QuestionnaireDto>();

            try
            {
                QuestionnaireDto questionnaireDto = new QuestionnaireDto();

                var questionnaire = _questionnairesRepository.FirstOrDefault(input.Id.Value);

                if (questionnaire != null)
                {
                    //取得 Questionnaire Info
                    questionnaireDto = ObjectMapper.Map<QuestionnaireDto>(questionnaire);

                    questionnaireDto.SubmissionDeadline = questionnaire.SubmissionDeadline.ToString("yyyy/MM/dd");
                }
                else
                {
                    questionnaireDto = new QuestionnaireDto();
                }

                #region 取得Question Type

                questionnaireDto.QuestionType = new List<QuestionTypeAllOutputDto>();

                var query = from questionTypes in _questionTypesRepository.GetAll().Where(x => x.Status == QuestionTypesStatus.Active.ToString())
                            select new QuestionTypeAllOutputDto
                            {
                                QuestionTypeId = questionTypes.Id,
                                QuestionTypeName = questionTypes.QuestionTypeName,
                            };

                //排序
                query = query.OrderBy("QuestionTypeId");

                questionnaireDto.QuestionType = query.ToList();

                if (questionnaire != null && questionnaire.QuestionTypeIds != null)
                {
                    foreach (var item in questionnaireDto.QuestionType.Where(x => questionnaire.QuestionTypeIds.Contains(x.QuestionTypeId.ToString())))
                    {
                        item.IsSeleted = true;
                    }
                }

                #endregion

                resultDto.Data = questionnaireDto;

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

        public async Task<ResultsDto<AsignDto>> GetQuestionnaireAsign(QuestionnaireAsignInputDto input)
        {
            ResultsDto<AsignDto> resultDto = new ResultsDto<AsignDto>();

            try
            {
                AsignDto asignDto = new AsignDto();

                //取出所有的 Entities
                var EntitiesList = await _entitiesRepository.GetAll().Where(x => x.Status != EntitiesStatus.InActive.ToString()).OrderBy(x => x.EntityName).ToListAsync();

                asignDto.Entities = ObjectMapper.Map<List<EntitiesDto>>(EntitiesList);

                //取出所有的 BUs
                var BUsList = await _bUsRepository.GetAll().Where(x => x.Status != BUstatus.InActive.ToString()).OrderBy(x => x.BUName).ToListAsync();

                #region 取得 Questionnaire Asign Info

                //取得 DB 中 QuestionnaireId 下的所有questionnaires Asign 資料
                var questionnairesAsignList = await _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.QuestionnaireId).ToListAsync();

                //取得選中的 Question Types 資料
                var queryQuestionTypes = from questionsType in _questionTypesRepository.GetAll().Where(x => x.Status != QuestionTypesStatus.InActive.ToString() && input.SelectedQuestionTypeId.Contains(x.Id))
                                         select new AsignQuestionTypeDto
                                         {
                                             QuestionTypeId = questionsType.Id,
                                             QuestionTypeName = questionsType.QuestionTypeName
                                         };

                var asignQuestionTypeDtoList = await queryQuestionTypes.OrderBy(x => x.QuestionTypeId).ToListAsync();

                var questionsList = _questionsRepository.GetAll().Where(x => x.Status != QuestionsStatus.InActive.ToString() && input.SelectedQuestionTypeId.Contains(x.QuestionType_Id.Value)).ToList();

                //取得所有的 questions Asign
                var questionsAsignList = new List<QuestionsAsign>();
                if (input.QuestionnaireId != 0)
                    questionsAsignList = _questionsAsignRepository.GetAllList(x => x.Questionnaire_Id == input.QuestionnaireId);

                //找出 Question Types 對應的 Questions
                foreach (var asignQuestionType in asignQuestionTypeDtoList)
                {
                    var queryQuestions = from questions in questionsList.Where(x => x.QuestionType_Id == asignQuestionType.QuestionTypeId)
                                         select new AsignQuestionDto
                                         {
                                             QuestionId = questions.Id,
                                             QuestionCode = questions.QuestionCode,
                                             Question = questions.Question,
                                         };

                    asignQuestionType.Questions = queryQuestions.OrderBy(x => x.QuestionCode).ToList();

                    foreach (var questions in asignQuestionType.Questions)
                    {
                        var questionsAsign = questionsAsignList.FirstOrDefault(x => x.Question_Id == questions.QuestionId);

                        if (questionsAsign != null)
                        {
                            questions.SingleAnswer = questionsAsign.SingleAnswer;
                        }

                        questions.Entities = new List<AsignEntitiesDto>();

                        foreach (var entities in EntitiesList)
                        {
                            AsignEntitiesDto entitiesDto = new AsignEntitiesDto();
                            entitiesDto.EntitiesId = entities.Id;
                            entitiesDto.EntityName = entities.EntityName;
                            entitiesDto.BUs = new List<QuestionnaireAsignEntitiesBUsDto>();

                            foreach (var bUs in BUsList)
                            {
                                QuestionnaireAsignEntitiesBUsDto bUsDto = new QuestionnaireAsignEntitiesBUsDto();
                                bUsDto.BUId = bUs.Id;
                                bUsDto.BUName = bUs.BUName;

                                var Asign = questionnairesAsignList.FirstOrDefault(x => x.BU_Id == bUs.Id && x.Entity_Id == entities.Id && x.Question_Id == questions.QuestionId);

                                if (Asign != null)
                                {
                                    bUsDto.QuestionnaireAsignId = Asign.Id;
                                    bUsDto.IsSeleted = true;
                                }

                                entitiesDto.BUs.Add(bUsDto);
                            }

                            questions.Entities.Add(entitiesDto);
                        }
                    }
                }

                asignDto.AsignQuestionType = asignQuestionTypeDtoList;

                #endregion

                resultDto.Data = asignDto;

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

        public ResultsDto<bool> InsertOrUpdate(QuestionnaireForEditInputDto input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            #region 驗證 questionnaire Id 是否有重複

            if (_questionnairesRepository.GetAll().Where(x => x.Id != input.Questionnaire.Id && x.QuestionnaireCode == input.Questionnaire.QuestionnaireCode).Any())
            {
                resultDto.Data = false;
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "The same Questionnaire Id already exists!";

                return resultDto;
            }

            #endregion

            try
            {
                #region Questionnaire Info 

                Questionnaires _Questionnaires = new Questionnaires();

                if (input.Questionnaire.Id != 0)
                {
                    //更新
                    _Questionnaires = _questionnairesRepository.FirstOrDefault(input.Questionnaire.Id);
                }

                _Questionnaires.QuestionnaireCode = input.Questionnaire.QuestionnaireCode;
                _Questionnaires.QuestionnaireName = input.Questionnaire.QuestionnaireName;
                _Questionnaires.RiskType = input.Questionnaire.RiskType;

                //如果 Status 為 Pending,並且 Submission Deadline 已過期就直接設 Status 為 Reviewing
                if (input.Questionnaire.Status == QuestionnairesStatus.Pending.ToString() && Convert.ToDateTime(input.Questionnaire.SubmissionDeadline) < DateTime.Today)
                {
                    _Questionnaires.Status = QuestionnairesStatus.Reviewing.ToString();
                }
                else
                {
                    _Questionnaires.Status = input.Questionnaire.Status;
                }

                _Questionnaires.SubmissionDeadline = Convert.ToDateTime(input.Questionnaire.SubmissionDeadline);

                //Seleted Question Type Info
                if (input.AsignQuestionType != null)
                {
                    List<int> QuestionTypeIds = input.AsignQuestionType.Select(x => x.QuestionTypeId).ToList();
                    _Questionnaires.QuestionTypeIds = string.Join(",", QuestionTypeIds);
                }

                int questionnaireId = _questionnairesRepository.InsertOrUpdateAndGetId(_Questionnaires);

                #endregion

                #region Questionnaire Asign info

                #region 新增或更新Questionnaire Asign

                var newQuestionnairesAsignList = new List<QuestionnairesAsign>();
                var deleteQestionnairesAsignList = new List<QuestionnairesAsign>();

                var existsSelectQuestionsAsignList = new List<int>();
                var existsUnSelectQuestionsAsignList = new List<int>();
                var newQuestionsAsignList = new List<QuestionsAsign>();
                var DBQuestionsAsignList = _questionsAsignRepository.GetAll().Where(x => x.Questionnaire_Id == questionnaireId);

                if (input.AsignQuestionType != null)
                {
                    foreach (var questionTypeDto in input.AsignQuestionType)
                    {
                        foreach (var questionDto in questionTypeDto.Questions)
                        {
                            var questionsAsign = DBQuestionsAsignList.FirstOrDefault(x => x.Question_Id == questionDto.QuestionId);

                            if (questionsAsign != null)
                            {
                                if (questionsAsign.SingleAnswer != questionDto.SingleAnswer)
                                {
                                    if (questionDto.SingleAnswer)
                                    {
                                        existsSelectQuestionsAsignList.Add(questionsAsign.Id);
                                    }
                                    else
                                    {
                                        existsUnSelectQuestionsAsignList.Add(questionsAsign.Id);
                                    }
                                }
                            }
                            else
                            {
                                questionsAsign = new QuestionsAsign
                                {
                                    Questionnaire_Id = questionnaireId,
                                    Question_Id = questionDto.QuestionId,
                                    SingleAnswer = questionDto.SingleAnswer
                                };

                                newQuestionsAsignList.Add(questionsAsign);
                            }

                            foreach (var entitiesDto in questionDto.Entities)
                            {
                                foreach (var bUDto in entitiesDto.BUs)
                                {
                                    if (bUDto.IsSeleted)
                                    {
                                        if (bUDto.QuestionnaireAsignId == 0)
                                        {
                                            QuestionnairesAsign questionnairesAsign = new QuestionnairesAsign();

                                            questionnairesAsign.Questionnaire_Id = questionnaireId;

                                            questionnairesAsign.BU_Id = bUDto.BUId;

                                            questionnairesAsign.Entity_Id = entitiesDto.EntitiesId;

                                            questionnairesAsign.Question_Id = questionDto.QuestionId;

                                            questionnairesAsign.SingleAnswer = questionDto.SingleAnswer;

                                            newQuestionnairesAsignList.Add(questionnairesAsign);
                                        }
                                    }
                                    else if (bUDto.QuestionnaireAsignId != 0)
                                    {
                                        deleteQestionnairesAsignList.Add(new QuestionnairesAsign() { Id = bUDto.QuestionnaireAsignId });
                                    }
                                }
                            }
                        }
                    }
                }

                //insert questions Asign
                _questionsAsignRepository.BulkInsert(newQuestionsAsignList);

                //update questions Asign

                _questionsAsignRepository.BatchUpdate(p => existsSelectQuestionsAsignList.Contains(p.Id), q => new QuestionsAsign { SingleAnswer = true });
                _questionsAsignRepository.BatchUpdate(p => existsUnSelectQuestionsAsignList.Contains(p.Id), q => new QuestionsAsign { SingleAnswer = false });

                //Delete questionnaires Asign
                _questionnairesAsignRepository.DeleteRangeByKey(deleteQestionnairesAsignList);

                //Insert questionnaires Asign
                _questionnairesAsignRepository.BulkInsert(newQuestionnairesAsignList);

                #endregion

                #endregion

                //當Status為Finished時，存儲Questionnaire資料，不會再作任何更改
                if (input.Questionnaire.Status == QuestionnairesStatus.Finished.ToString())
                {
                    InsertOrUpdateQuestionnairesFinished(questionnaireId);
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

        public ResultsDto<QuestionnaireStatusCountOutputDto> GetQuestionnaireStatusCount()
        {
            ResultsDto<QuestionnaireStatusCountOutputDto> resultDto = new ResultsDto<QuestionnaireStatusCountOutputDto>();

            try
            {
                QuestionnaireStatusCountOutputDto questionnaireStatusCountOutputDto = new QuestionnaireStatusCountOutputDto();

                var query = _questionnairesRepository.GetAll();

                questionnaireStatusCountOutputDto.PendingQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Pending.ToString()).ToList().Count;
                questionnaireStatusCountOutputDto.ReviewingQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Reviewing.ToString()).ToList().Count;
                questionnaireStatusCountOutputDto.FinishedQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Finished.ToString()).ToList().Count;
                questionnaireStatusCountOutputDto.DraftingQuestionnaireCount = query.Where(x => x.Status == QuestionnairesStatus.Drafting.ToString()).ToList().Count;

                resultDto.Data = questionnaireStatusCountOutputDto;

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

        public ResultsDto<List<QuestionnaireQuestionTypeDto>> GetQuestionnaireQuestionType(NullableIdDto input)
        {
            ResultsDto<List<QuestionnaireQuestionTypeDto>> resultDto = new ResultsDto<List<QuestionnaireQuestionTypeDto>>();

            try
            {
                List<QuestionnaireQuestionTypeDto> questionnaireQuestionTypeDtoList = new List<QuestionnaireQuestionTypeDto>();

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.Id.Value);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        List<int> questionIds = _questionnairesAsignRepository.GetAll()
                                                .Where(x => x.Questionnaire_Id == input.Id.Value)
                                                .Select(x => x.Question_Id.Value).Distinct().ToList();

                        List<int> questionTypeIds = _questionsRepository.GetAll().Where(x => questionIds.Contains(x.Id) && x.Status != QuestionsStatus.InActive.ToString()).Select(x => x.QuestionType_Id.Value).Distinct().ToList();

                        var query = _questionTypesRepository.GetAll().Where(x => questionTypeIds.Contains(x.Id) && x.Status != QuestionTypesStatus.InActive.ToString());

                        resultDto.Data = ObjectMapper.Map<List<QuestionnaireQuestionTypeDto>>(query.ToList());

                        #endregion
                    }
                    else
                    {
                        //取得Finished Questionnaire Question Type 資料
                        resultDto.Data = GetFinishedQuestionnaireQuestionType(input.Id.Value);
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

        public ResultsDto<List<BUsDto>> GetQuestionnaireBUs(NullableIdDto input)
        {
            ResultsDto<List<BUsDto>> resultDto = new ResultsDto<List<BUsDto>>();

            try
            {
                List<BUsDto> BUsDtoList = new List<BUsDto>();

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.Id.Value);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        var bUIds = _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.Id).Select(x => x.BU_Id).Distinct().ToList();

                        var query = from bUs in _bUsRepository.GetAll().Where(x => bUIds.Contains(x.Id) && x.Status != BUstatus.InActive.ToString())
                                    select new BUsDto
                                    {
                                        Id = bUs.Id,
                                        BUName = bUs.BUName,
                                    };

                        BUsDtoList = query.ToList();

                        #endregion
                    }
                    else
                    {
                        #region Questionnaire Status為Finished的資料查詢

                        //取出Questionnaires Finished 資料
                        var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == input.Id).ToList();

                        foreach (var item in questionnairesFinished)
                        {
                            BUsDto bUsDto = new BUsDto();
                            bUsDto.Id = item.BUId;
                            bUsDto.BUName = item.BUName;

                            BUsDtoList.Add(bUsDto);
                        }

                        #endregion
                    }
                }

                resultDto.Data = BUsDtoList;

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

        public ResultsDto<List<EntitiesDto>> GetQuestionnaireEntity(NullableIdDto input)
        {
            ResultsDto<List<EntitiesDto>> resultDto = new ResultsDto<List<EntitiesDto>>();

            try
            {
                resultDto.Data = GetQuestionnaireEntityCommon(input.Id.Value);

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

        public ResultsDto<PagedResultDto<ControlRiskQuestionnaireDto>> GetControlRiskQuestionnaire(ControlRiskQuestionnaireInputDto input)
        {
            ResultsDto<PagedResultDto<ControlRiskQuestionnaireDto>> resultDto = new ResultsDto<PagedResultDto<ControlRiskQuestionnaireDto>>();

            try
            {
                List<ControlRiskQuestionnaireDto> controlRiskQuestionnaireDtoList = new List<ControlRiskQuestionnaireDto>();

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.Id);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        var bUIds = _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.Id).Select(x => x.BU_Id).Distinct().ToList();

                        var query = from bUs in _bUsRepository.GetAll().Where(x => bUIds.Contains(x.Id) && x.Status != BUstatus.InActive.ToString())
                                    select new ControlRiskQuestionnaireDto
                                    {
                                        Id = bUs.Id,
                                        BUName = bUs.BUName,
                                    };

                        controlRiskQuestionnaireDtoList = query.ToList();

                        #endregion
                    }
                    else
                    {
                        #region Questionnaire Status為Finished的資料查詢

                        //取出Questionnaires Finished 資料
                        var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == input.Id).ToList();

                        foreach (var item in questionnairesFinished)
                        {
                            ControlRiskQuestionnaireDto controlRiskQuestionnaireDto = new ControlRiskQuestionnaireDto();
                            controlRiskQuestionnaireDto.Id = item.BUId;
                            controlRiskQuestionnaireDto.BUName = item.BUName;

                            controlRiskQuestionnaireDtoList.Add(controlRiskQuestionnaireDto);
                        }

                        #endregion
                    }
                }

                int resultCount = controlRiskQuestionnaireDtoList.Count;

                //分頁
                controlRiskQuestionnaireDtoList = ExtensionHelper.GetPaging(controlRiskQuestionnaireDtoList, input.SkipCount, input.MaxResultCount);

                #region 取得所有的 Questionnaire Entity

                var questionnairesEntitiesList = GetQuestionnaireEntityCommon(input.Id);

                foreach (var item in controlRiskQuestionnaireDtoList)
                {
                    item.Entities = new List<CRQBUsEntitiesDto>();

                    foreach (var entities in questionnairesEntitiesList)
                    {
                        CRQBUsEntitiesDto _CRQBUsEntitiesDto = new CRQBUsEntitiesDto();
                        _CRQBUsEntitiesDto.Id = entities.Id;
                        _CRQBUsEntitiesDto.EntityName = entities.EntityName;
                        _CRQBUsEntitiesDto.Progress = "-";

                        item.Entities.Add(_CRQBUsEntitiesDto);
                    }
                }

                #endregion

                #region 計算問卷完成情況

                //取得該問卷的所有完成資料
                List<CalQuestionnairesProgressEntity> CalQuestionnairesProgressEntityList = _questionnairesManager.GetCalProgressEntity(input.Id);

                if (CalQuestionnairesProgressEntityList.Any())
                {
                    //計算問卷BU的完成進度
                    foreach (var collcontrolRiskQuestionnaire in controlRiskQuestionnaireDtoList)
                    {
                        var CalBUProgressList = CalQuestionnairesProgressEntityList.Where(x => x.BUId == collcontrolRiskQuestionnaire.Id).ToList();

                        if (CalBUProgressList.Any())
                        {
                            //*1.0:是因為int類型除以int類型會得出0
                            decimal TotalProgress = (decimal)(CalBUProgressList.Where(x => x.IsDone).ToList().Count * 1.0 / CalBUProgressList.Count) * 100;

                            collcontrolRiskQuestionnaire.TotalProgress = ExtensionHelper.Round(TotalProgress, 0);

                            //計算問卷BU,Entities的完成進度
                            foreach (var entities in collcontrolRiskQuestionnaire.Entities)
                            {
                                var CalEntitiesProgressList = CalBUProgressList.Where(x => x.EntitiesId == entities.Id).ToList();

                                if (CalEntitiesProgressList.Any())
                                {
                                    //*1.0:是因為int類型除以int類型會得出0
                                    decimal entitiesProgress = (decimal)(CalEntitiesProgressList.Where(x => x.IsDone).ToList().Count * 1.0 / CalEntitiesProgressList.Count) * 100;

                                    entities.Progress = ExtensionHelper.Round(entitiesProgress, 0).ToString();
                                }
                            }
                        }
                    }
                }

                #endregion

                resultDto.Data = new PagedResultDto<ControlRiskQuestionnaireDto>(resultCount, controlRiskQuestionnaireDtoList);

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

        public ResultsDto<PagedResultDto<SupportingDocumentOutputDto>> GetSupportingDocument(SupportingDocumentInputDto input)
        {
            ResultsDto<PagedResultDto<SupportingDocumentOutputDto>> resultDto = new ResultsDto<PagedResultDto<SupportingDocumentOutputDto>>();

            try
            {
                List<SupportingDocumentOutputDto> supportingDocumentOutputDtoList = new List<SupportingDocumentOutputDto>();

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.Id);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        var query = from supportingDocument in _supportingDocumentRepository.GetAll()
                                    join questionnairesAsign in _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.Id) on supportingDocument.QuestionnairesAsign_Id equals questionnairesAsign.Id
                                    join bUs in _bUsRepository.GetAll() on questionnairesAsign.BU_Id equals bUs.Id
                                    join entities in _entitiesRepository.GetAll() on questionnairesAsign.Entity_Id equals entities.Id
                                    join questions in _questionsRepository.GetAll().Where(x => x.Status != QuestionsStatus.InActive.ToString()) on questionnairesAsign.Question_Id equals questions.Id
                                    join questionTypes in _questionTypesRepository.GetAll() on questions.QuestionType_Id equals questionTypes.Id
                                    select new SupportingDocumentOutputDto
                                    {
                                        BUId = bUs.Id,
                                        BUName = bUs.BUName,
                                        EntitiesId = entities.Id,
                                        EntityName = entities.EntityName,
                                        QuestionTypeId = questionTypes.Id,
                                        QuestionTypeName = questionTypes.QuestionTypeName,
                                        QuestionCode = questions.QuestionCode,
                                        Name = supportingDocument.Name,
                                        PathUrl = supportingDocument.PathUrl,
                                        Id = supportingDocument.Id
                                    };

                        query = query.WhereIf(input.BUId != 0, x => x.BUId == input.BUId);

                        supportingDocumentOutputDtoList = query.ToList();

                        #endregion
                    }
                    else
                    {
                        //取出Questionnaires Finished SupportingDocument 資料
                        supportingDocumentOutputDtoList = GetFinishedSupportingDocument(input.Id, input.BUId);
                    }

                    if (supportingDocumentOutputDtoList.Any())
                    {
                        supportingDocumentOutputDtoList = supportingDocumentOutputDtoList.WhereIf(!input.Name.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Name.Trim())).ToList();
                        supportingDocumentOutputDtoList = supportingDocumentOutputDtoList.WhereIf(input.EntitiesId != 0, x => x.EntitiesId == input.EntitiesId).ToList();
                        supportingDocumentOutputDtoList = supportingDocumentOutputDtoList.WhereIf(input.QuestionTypeId != 0, x => x.QuestionTypeId == input.QuestionTypeId).ToList();

                        supportingDocumentOutputDtoList = supportingDocumentOutputDtoList.OrderBy(x => x.BUName).ThenBy(x => x.EntityName).ThenBy(x => x.QuestionTypeName).ThenBy(x => x.QuestionCode).ThenBy(x => x.Name).ToList();
                    }
                }

                int resultCount = supportingDocumentOutputDtoList.Count;

                //分頁
                supportingDocumentOutputDtoList = ExtensionHelper.GetPaging(supportingDocumentOutputDtoList, input.SkipCount, input.MaxResultCount);

                resultDto.Data = new PagedResultDto<SupportingDocumentOutputDto>(resultCount, supportingDocumentOutputDtoList);

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

        public ResultsDto<string> ExportQuestionnaire(NullableIdDto input)
        {
            ResultsDto<string> resultDto = new ResultsDto<string>();

            try
            {
                List<ExportQuestionnairesDto> ExportQuestionnairesDtoList = new List<ExportQuestionnairesDto>();

                #region 取得 Questionnaires 資料

                var questionnaires = _questionnairesRepository.FirstOrDefault(input.Id.Value);

                if (questionnaires != null)
                {
                    if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                    {
                        #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                        //取出問卷所有的問題
                        var questionnairesAsignList = _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == input.Id.Value).ToList();

                        foreach (var questionnairesAsign in questionnairesAsignList)
                        {
                            ExportQuestionnairesDto exportQuestionnairesDto = new ExportQuestionnairesDto();

                            //entities 資料
                            var entities = _entitiesRepository.FirstOrDefault(questionnairesAsign.Entity_Id.Value);
                            exportQuestionnairesDto.Entity = entities?.EntityName;

                            //bU 資料
                            var bU = _bUsRepository.FirstOrDefault(questionnairesAsign.BU_Id.Value);
                            exportQuestionnairesDto.BU = bU?.BUName;

                            #region questions 資料

                            var questions = _questionsRepository.FirstOrDefault(questionnairesAsign.Question_Id.Value);
                            exportQuestionnairesDto.QuestionId = questions?.QuestionCode;
                            exportQuestionnairesDto.Question = questions?.Question;
                            exportQuestionnairesDto.HighestRating = questions?.HighestRating;

                            if (questions != null && questions.HasAnswer)
                            {
                                exportQuestionnairesDto.MCAnswer = "null";
                                exportQuestionnairesDto.RecommendedScore = "null";

                                if (!string.IsNullOrWhiteSpace(questionnairesAsign.SelectedAnswer))
                                {
                                    var SelectedQuestionsAnswer = _questionsAnswerRepository.FirstOrDefault(Convert.ToInt32(questionnairesAsign.SelectedAnswer));

                                    if (SelectedQuestionsAnswer != null)
                                    {
                                        exportQuestionnairesDto.MCAnswer = SelectedQuestionsAnswer.AnswerContent;
                                        exportQuestionnairesDto.RecommendedScore = SelectedQuestionsAnswer.RecommendedScore;
                                    }
                                }
                            }
                            else
                            {
                                exportQuestionnairesDto.MCAnswer = "N/A";
                                exportQuestionnairesDto.RecommendedScore = "N/A";
                            }

                            if (questions != null && questions.HasFreeText)
                            {
                                exportQuestionnairesDto.FreeTextAnswer = "null";

                                if (!string.IsNullOrWhiteSpace(questionnairesAsign.FreeText))
                                {
                                    exportQuestionnairesDto.FreeTextAnswer = questionnairesAsign.FreeText;
                                }
                            }
                            else
                            {
                                exportQuestionnairesDto.FreeTextAnswer = "N/A";
                            }

                            #endregion

                            if (questions != null)
                            {
                                //question Type 資料
                                var questionType = _questionTypesRepository.FirstOrDefault(questions.QuestionType_Id.Value);
                                exportQuestionnairesDto.QuestionTypeId = questionType.QuestionTypeCode;
                                exportQuestionnairesDto.QuestionType = questionType.QuestionTypeName;
                            }

                            //supporting Documents 資料
                            if (questions != null && questions.HasSupportingDocument)
                            {
                                var supportingDocuments = _supportingDocumentRepository.GetAll().Where(x => x.QuestionnairesAsign_Id == questionnairesAsign.Id).ToList();
                                exportQuestionnairesDto.NomberOfSupportingDocument = supportingDocuments.Count.ToString();
                            }
                            else
                            {
                                exportQuestionnairesDto.NomberOfSupportingDocument = "N/A";
                            }

                            ExportQuestionnairesDtoList.Add(exportQuestionnairesDto);
                        }

                        #endregion
                    }
                    else
                    {
                        //取出 Finished Questionnaires  Question 資料
                        ExportQuestionnairesDtoList = GetFinishedQuestionnaires(input.Id.Value);
                    }
                }

                //排序:Entity,BU,QuestionId,QuestionType,QuestionId,Question
                ExportQuestionnairesDtoList = ExportQuestionnairesDtoList.OrderBy(x => x.Entity).ThenBy(x => x.BU).ThenBy(x => x.QuestionId).ThenBy(x => x.QuestionType).ThenBy(x => x.QuestionId).ThenBy(x => x.Question).ToList();

                #endregion

                FileHelper fileHelper = new FileHelper();

                string Domain = ConfigurationSettings.AppSettings["Domain"];

                resultDto.Data = Domain + fileHelper.Export(ExportQuestionnairesDtoList);

                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = L("SystemError");
            }

            return resultDto;
        }

        #region 内部方法

        /// <summary>
        /// 取得 Questionnaire Entity 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire Id</param>
        /// <returns></returns>
        private List<EntitiesDto> GetQuestionnaireEntityCommon(int QuestionnaireId)
        {
            List<EntitiesDto> EntitiesDtoList = new List<EntitiesDto>();

            var questionnaires = _questionnairesRepository.FirstOrDefault(QuestionnaireId);

            if (questionnaires != null)
            {
                if (questionnaires.Status != QuestionnairesStatus.Finished.ToString())
                {
                    List<int> entityIds = _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == QuestionnaireId).Select(x => x.Entity_Id.Value).Distinct().ToList();

                    var query = _entitiesRepository.GetAll().Where(x => entityIds.Contains(x.Id) && x.Status != EntitiesStatus.InActive.ToString());

                    EntitiesDtoList = ObjectMapper.Map<List<EntitiesDto>>(query.ToList());
                }
                else
                {
                    //取得 Finished Questionnaire Entity 資料
                    EntitiesDtoList = GetFinishedQuestionnaireEntity(QuestionnaireId);
                }
            }

            return EntitiesDtoList;
        }

        #endregion

        #region Finished Questionnaires info

        /// <summary>
        /// 取得 Finished Questionnaire Question Type 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaire Id</param>
        /// <returns></returns>
        private List<QuestionnaireQuestionTypeDto> GetFinishedQuestionnaireQuestionType(int QuestionnaireId)
        {
            List<QuestionnaireQuestionTypeDto> questionnaireQuestionTypeDtoList = new List<QuestionnaireQuestionTypeDto>();

            #region Questionnaire Status為Finished的資料

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId).ToList();

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

                                        QuestionnaireQuestionTypeDto questionnaireQuestionTypeDto = new QuestionnaireQuestionTypeDto();
                                        questionnaireQuestionTypeDto.Id = question.QuestionType_Id.Value;
                                        questionnaireQuestionTypeDto.QuestionTypeName = question.QuestionTypeName;

                                        questionnaireQuestionTypeDtoList.Add(questionnaireQuestionTypeDto);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            return questionnaireQuestionTypeDtoList;
        }

        /// <summary>
        /// 存儲Questionnaire資料,作為後期Questionnaire Finished資料查詢使用
        /// </summary>
        /// <param name="QuestionnairesId">Questionnaires Id</param>
        private void InsertOrUpdateQuestionnairesFinished(int QuestionnairesId)
        {
            //找出該Questionnaire所有的questionnaires Asign
            var questionnairesAsignList = _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == QuestionnairesId).ToList();

            var BUIds = questionnairesAsignList.Select(x => x.BU_Id).Distinct().ToList();
            var bUsList = _bUsRepository.GetAll().Where(x => BUIds.Contains(x.Id)).ToList();

            var entitiesIds = questionnairesAsignList.Select(x => x.Entity_Id).Distinct().ToList();
            var entitiesList = _entitiesRepository.GetAll().Where(x => entitiesIds.Contains(x.Id)).ToList();

            var questionsIds = questionnairesAsignList.Select(x => x.Question_Id).Distinct().ToList();
            var questionsList = _questionsRepository.GetAllIncluding(x => x.QuestionsAnswers).Where(x => questionsIds.Contains(x.Id)).ToList();

            var questionnairesAsignIds = questionnairesAsignList.Select(x => x.Id).ToList();
            var supportingDocumentList = _supportingDocumentRepository.GetAll().Where(x => questionnairesAsignIds.Contains(x.QuestionnairesAsign_Id)).ToList();

            var DBQuestionsAsignList = _questionsAsignRepository.GetAll().Where(x => x.Questionnaire_Id == QuestionnairesId).ToList();

            foreach (var BU in bUsList)
            {
                QuestionnairesFinished questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnairesId && x.BUId == BU.Id).FirstOrDefault();

                if (questionnairesFinished == null)
                {
                    questionnairesFinished = new QuestionnairesFinished();
                    questionnairesFinished.QuestionnairesId = QuestionnairesId;
                    questionnairesFinished.BUId = BU.Id;
                }

                questionnairesFinished.BUName = BU.BUName;

                #region Entity info

                List<QuestionnairesFinishedEntityQuestionsDto> questionnairesFinishedJson = new List<QuestionnairesFinishedEntityQuestionsDto>();

                var entityIds = questionnairesAsignList.Where(x => x.Questionnaire_Id == QuestionnairesId && x.BU_Id == BU.Id).Select(x => x.Entity_Id).ToList();

                foreach (var entities in entitiesList.Where(x => entityIds.Contains(x.Id)).ToList())
                {
                    QuestionnairesFinishedEntityQuestionsDto EntityQuestionsDto = new QuestionnairesFinishedEntityQuestionsDto();

                    EntityQuestionsDto.Id = entities.Id;

                    EntityQuestionsDto.EntityName = entities.EntityName;

                    #region Question info

                    List<QuestionnairesFinishedQuestionsDto> QuestionsList = new List<QuestionnairesFinishedQuestionsDto>();

                    foreach (var questionnairesAsign in questionnairesAsignList.Where(x => x.Questionnaire_Id == QuestionnairesId && x.BU_Id == BU.Id && x.Entity_Id == entities.Id).ToList())
                    {
                        var questions = questionsList.Where(x => x.Id == questionnairesAsign.Question_Id.Value).FirstOrDefault();

                        QuestionnairesFinishedQuestionsDto QuestionsDto = ObjectMapper.Map<QuestionnairesFinishedQuestionsDto>(questions);

                        var questionTypes = _questionTypesRepository.FirstOrDefault(questions.QuestionType_Id.Value);

                        QuestionsDto.QuestionnairesAsignId = questionnairesAsign.Id;

                        QuestionsDto.QuestionTypeCode = questionTypes?.QuestionTypeCode;
                        QuestionsDto.QuestionTypeName = questionTypes?.QuestionTypeName;

                        var DBQuestionsAsign = DBQuestionsAsignList.FirstOrDefault(x => x.Question_Id == questionnairesAsign.Question_Id.Value);

                        if (DBQuestionsAsign != null)
                        {
                            QuestionsDto.SingleAnswer = DBQuestionsAsign.SingleAnswer;
                        }

                        QuestionsDto.SelectedAnswer = questionnairesAsign.SelectedAnswer;
                        QuestionsDto.FreeText = questionnairesAsign.FreeText;

                        var QuestionsAnswers = questions.QuestionsAnswers.Where(x => x.Status != QuestionsAnswerStatus.InActive.ToString()).ToList();

                        QuestionsDto.QuestionsAnswers = ObjectMapper.Map<List<QuestionnairesFinishedQuestionsAnswer>>(QuestionsAnswers);

                        #region Supporting Document

                        QuestionsDto.SupportingDocument = new List<SupportingDocumentOutputDto>();

                        foreach (var supportingDocument in supportingDocumentList.Where(x => x.QuestionnairesAsign_Id == questionnairesAsign.Id).ToList())
                        {
                            SupportingDocumentOutputDto supportingDocumentOutputDto = new SupportingDocumentOutputDto();

                            supportingDocumentOutputDto.Id = supportingDocument.Id;
                            supportingDocumentOutputDto.Name = supportingDocument.Name;
                            supportingDocumentOutputDto.PathUrl = supportingDocument.PathUrl;

                            supportingDocumentOutputDto.BUId = BU.Id;
                            supportingDocumentOutputDto.BUName = BU.BUName;

                            supportingDocumentOutputDto.EntitiesId = entities.Id;
                            supportingDocumentOutputDto.EntityName = entities?.EntityName;

                            supportingDocumentOutputDto.QuestionTypeId = questions.QuestionType_Id.Value;
                            supportingDocumentOutputDto.QuestionTypeName = questionTypes?.QuestionTypeName;
                            supportingDocumentOutputDto.QuestionCode = questions?.QuestionCode;

                            QuestionsDto.SupportingDocument.Add(supportingDocumentOutputDto);
                        }

                        #endregion

                        QuestionsList.Add(QuestionsDto);
                    }

                    EntityQuestionsDto.Questions = QuestionsList;

                    #endregion

                    questionnairesFinishedJson.Add(EntityQuestionsDto);
                }

                //將對象轉為json存儲在DB
                questionnairesFinished.EntityQuestionsJson = JsonConvert.SerializeObject(questionnairesFinishedJson);

                #endregion

                _questionnairesFinishedRepository.InsertOrUpdate(questionnairesFinished);
            }
        }

        /// <summary>
        /// 取得 Control Risk Finished Questionnaire Entity
        /// </summary>
        /// <param name="QuestionnaireId"></param>
        /// <returns></returns>
        private List<EntitiesDto> GetFinishedQuestionnaireEntity(int QuestionnaireId)
        {
            List<EntitiesDto> entitiesDtoList = new List<EntitiesDto>();

            List<int> EntitiesIds = new List<int>();

            #region Questionnaire Status為Finished的資料查詢

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId).ToList();

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
                            if (!EntitiesIds.Contains(EntityQuestions.Id))
                            {
                                EntitiesIds.Add(EntityQuestions.Id);

                                EntitiesDto entitiesDto = new EntitiesDto();

                                entitiesDto.Id = EntityQuestions.Id;
                                entitiesDto.EntityName = EntityQuestions.EntityName;
                                entitiesDtoList.Add(entitiesDto);
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            return entitiesDtoList;
        }

        /// <summary>
        /// 取得 Finished Questionnaires SupportingDocument 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaires Id</param>
        /// <param name="BUId">BU Id</param>
        /// <returns></returns>
        private List<SupportingDocumentOutputDto> GetFinishedSupportingDocument(int QuestionnaireId, int BUId)
        {
            List<SupportingDocumentOutputDto> supportingDocumentOutputDtoList = new List<SupportingDocumentOutputDto>();

            #region 取出Supporting Document 資料

            //取出Questionnaires Finished 資料
            var questionnairesFinished = _questionnairesFinishedRepository.GetAll()
                                        .Where(x => x.QuestionnairesId == QuestionnaireId)
                                        .WhereIf(BUId != 0, x => x.BUId == BUId)
                                        .ToList();

            //取出Questionnaires Finished 資料
            foreach (var item in questionnairesFinished)
            {
                if (!string.IsNullOrWhiteSpace(item.EntityQuestionsJson))
                {
                    var EntityQuestionsList = JsonConvert.DeserializeObject<List<QuestionnairesFinishedEntityQuestionsDto>>(item.EntityQuestionsJson);

                    if (EntityQuestionsList != null && EntityQuestionsList.Any())
                    {
                        foreach (var EntityQuestions in EntityQuestionsList)
                        {
                            if (EntityQuestions.Questions != null && EntityQuestions.Questions.Any())
                            {
                                foreach (var questions in EntityQuestions.Questions)
                                {
                                    if (questions.SupportingDocument != null && questions.SupportingDocument.Any())
                                    {
                                        supportingDocumentOutputDtoList.AddRange(questions.SupportingDocument);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            return supportingDocumentOutputDtoList;
        }

        /// <summary>
        /// 取得 Finished Questionnaires Question 資料
        /// </summary>
        /// <param name="QuestionnaireId">Questionnaires Id</param>
        /// <returns></returns>
        private List<ExportQuestionnairesDto> GetFinishedQuestionnaires(int QuestionnaireId)
        {
            List<ExportQuestionnairesDto> ExportQuestionnairesDtoList = new List<ExportQuestionnairesDto>();

            var FinishedQuestionnaire = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnaireId).ToList();

            foreach (var item in FinishedQuestionnaire)
            {
                if (!string.IsNullOrWhiteSpace(item.EntityQuestionsJson))
                {
                    var EntityQuestionsList = JsonConvert.DeserializeObject<List<QuestionnairesFinishedEntityQuestionsDto>>(item.EntityQuestionsJson);

                    if (EntityQuestionsList != null && EntityQuestionsList.Any())
                    {
                        foreach (var EntityQuestions in EntityQuestionsList)
                        {
                            if (EntityQuestions.Questions != null && EntityQuestions.Questions.Any())
                            {
                                foreach (var questions in EntityQuestions.Questions)
                                {
                                    ExportQuestionnairesDto exportQuestionnairesDto = new ExportQuestionnairesDto();

                                    //entities 資料
                                    exportQuestionnairesDto.Entity = EntityQuestions.EntityName;

                                    //bU 資料
                                    exportQuestionnairesDto.BU = item.BUName;

                                    #region questions 資料

                                    exportQuestionnairesDto.QuestionId = questions.QuestionCode;
                                    exportQuestionnairesDto.Question = questions.Question;
                                    exportQuestionnairesDto.HighestRating = questions.HighestRating;

                                    if (questions.HasAnswer)
                                    {
                                        exportQuestionnairesDto.MCAnswer = "null";
                                        exportQuestionnairesDto.RecommendedScore = "null";

                                        if (!string.IsNullOrWhiteSpace(questions.SelectedAnswer))
                                        {
                                            var SelectedQuestionsAnswer = questions.QuestionsAnswers.Where(x => x.Id == Convert.ToInt32(questions.SelectedAnswer)).FirstOrDefault();

                                            if (SelectedQuestionsAnswer != null)
                                            {
                                                exportQuestionnairesDto.MCAnswer = SelectedQuestionsAnswer.AnswerContent;
                                                exportQuestionnairesDto.RecommendedScore = SelectedQuestionsAnswer.RecommendedScore;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        exportQuestionnairesDto.MCAnswer = "N/A";
                                        exportQuestionnairesDto.RecommendedScore = "N/A";
                                    }

                                    if (questions.HasFreeText)
                                    {
                                        exportQuestionnairesDto.FreeTextAnswer = "null";

                                        if (!string.IsNullOrWhiteSpace(questions.FreeText))
                                        {
                                            exportQuestionnairesDto.FreeTextAnswer = questions.FreeText;
                                        }
                                    }
                                    else
                                    {
                                        exportQuestionnairesDto.FreeTextAnswer = "N/A";
                                    }

                                    #endregion

                                    //question Type 資料
                                    exportQuestionnairesDto.QuestionTypeId = questions.QuestionTypeCode;
                                    exportQuestionnairesDto.QuestionType = questions.QuestionTypeName;

                                    //supporting Documents 資料
                                    if (questions.HasSupportingDocument)
                                    {
                                        exportQuestionnairesDto.NomberOfSupportingDocument = questions.SupportingDocument.Count.ToString();
                                    }
                                    else
                                    {
                                        exportQuestionnairesDto.NomberOfSupportingDocument = "N/A";
                                    }

                                    ExportQuestionnairesDtoList.Add(exportQuestionnairesDto);
                                }
                            }
                        }
                    }
                }
            }

            return ExportQuestionnairesDtoList;
        }

        #endregion
    }
}
