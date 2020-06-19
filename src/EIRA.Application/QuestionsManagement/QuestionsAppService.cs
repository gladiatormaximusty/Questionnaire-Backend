using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EIRA.Authorization;
using EIRA.Common;
using EIRA.Enums;
using EIRA.QuestionsManagement.Dto;
using EIRA.ResultDto;
using EIRA.Table;
using EIRA.TableManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace EIRA.QuestionsManagement
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class QuestionsAppService : EIRAAppServiceBase, IQuestionsAppService
    {
        IRepository<Questions> _questionsRepository;
        IRepository<QuestionTypes> _questionTypesRepository;
        AdminUserManager _adminUserManager;
        IRepository<Questionnaires> _questionnairesRepository;
        IRepository<QuestionnairesAsign> _questionnairesAsignRepository;
        IRepository<SupportingDocument> _supportingDocumentRepository;

        public QuestionsAppService(IRepository<Questions> questionsRepository, IRepository<QuestionTypes> questionTypesRepository, AdminUserManager adminUserManager
            , IRepository<Questionnaires> questionnairesRepository, IRepository<QuestionnairesAsign> questionnairesAsignRepository, IRepository<SupportingDocument> supportingDocumentRepository)
        {
            _questionsRepository = questionsRepository;
            _questionTypesRepository = questionTypesRepository;
            _adminUserManager = adminUserManager;
            _questionnairesRepository = questionnairesRepository;
            _questionnairesAsignRepository = questionnairesAsignRepository;
            _supportingDocumentRepository = supportingDocumentRepository;
        }

        public ResultsDto<bool> Delete(NullableIdDto input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                var _Questions = _questionsRepository.FirstOrDefault(input.Id.Value);

                if (_Questions != null)
                {
                    _Questions.Status = QuestionsStatus.InActive.ToString();

                    _questionsRepository.Update(_Questions);
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

        public ResultsDto<PagedResultDto<QuestionsOutputDto>> GetAll(QuestionsInputDto input)
        {
            ResultsDto<PagedResultDto<QuestionsOutputDto>> resultDto = new ResultsDto<PagedResultDto<QuestionsOutputDto>>();

            try
            {
                var query = from questions in _questionsRepository.GetAll()
                            join questionTypes in _questionTypesRepository.GetAll() on questions.QuestionType_Id equals questionTypes.Id
                            into Temp
                            from questionsList in Temp.DefaultIfEmpty()
                            select new QuestionsOutputDto
                            {
                                Id = questions.Id,
                                Question = questions.Question,
                                QuestionCode = questions.QuestionCode,
                                Status = questions.Status,
                                QuestionTypeId = questions.QuestionType_Id == null ? 0 : questions.QuestionType_Id,
                                QuestionTypeName = questionsList.QuestionTypeName == null ? "" : questionsList.QuestionTypeName,
                            };

                //查詢條件
                query = query.WhereIf(!input.Status.IsNullOrWhiteSpace(), x => x.Status == input.Status.Trim());
                query = query.WhereIf(input.QuestionTypeId != 0, x => x.QuestionTypeId == input.QuestionTypeId);
                query = query.WhereIf(!input.Question.IsNullOrWhiteSpace(), x => x.QuestionCode.Contains(input.Question.Trim()) || x.Question.Contains(input.Question.Trim()));

                //排序
                if (string.IsNullOrWhiteSpace(input.Sorting))
                {
                    input.Sorting = "QuestionTypeName,QuestionCode";
                }

                query = query.OrderBy(input.Sorting);

                //记录总数
                var resultCount = query.Count();

                //分页
                query = query.PageBy(input);

                resultDto.Data = new PagedResultDto<QuestionsOutputDto>(resultCount, query.ToList());

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

        public ResultsDto<QuestionsDto> GetQuestionsById(NullableIdDto input)
        {
            ResultsDto<QuestionsDto> resultDto = new ResultsDto<QuestionsDto>();

            try
            {
                if (input.Id == 0)
                {
                    QuestionsDto _QuestionsDto = new QuestionsDto();

                    //add new Questions頁面返回UpdateTime,UpdateUser給前端顯示
                    _QuestionsDto.UpdateTime = DateTime.Now.ToEIRATime();
                    _QuestionsDto.UpdateUser = _adminUserManager.GetUserNameById(AbpSession.UserId);

                    resultDto.Data = _QuestionsDto;
                }
                else
                {
                    var questions = _questionsRepository.GetAllIncluding(x => x.QuestionsAnswers).Where(x => x.Id == input.Id.Value).FirstOrDefault();

                    //QuestionsAnswers 過濾InActive狀態資料
                    questions.QuestionsAnswers = questions?.QuestionsAnswers.Where(x => x.Status != QuestionsAnswerStatus.InActive.ToString()).ToList();

                    var _QuestionsDto = ObjectMapper.Map<QuestionsDto>(questions);

                    if (string.IsNullOrWhiteSpace(_QuestionsDto.FreeTextPlaceholder))
                    {
                        _QuestionsDto.FreeTextPlaceholder = "";
                    }

                    if (questions != null)
                    {
                        _QuestionsDto.QuestionTypeId = questions.QuestionType_Id.Value;

                        if (questions.LastModificationTime.HasValue)
                        {
                            _QuestionsDto.UpdateTime = questions.LastModificationTime.Value.ToEIRATime();
                            _QuestionsDto.UpdateUser = _adminUserManager.GetUserNameById(questions.LastModifierUserId);
                        }
                        else
                        {
                            _QuestionsDto.UpdateTime = questions.CreationTime.ToEIRATime();
                            _QuestionsDto.UpdateUser = _adminUserManager.GetUserNameById(questions.CreatorUserId);
                        }
                    }

                    resultDto.Data = _QuestionsDto;
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

        public ResultsDto<bool> InsertOrUpdate(QuestionsDto input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            #region 必要項驗證

            List<string> ErrorMessage = new List<string>();

            if (input.QuestionTypeId == 0)
            {
                ErrorMessage.Add("Please select the Question Type!");
            }

            if (string.IsNullOrWhiteSpace(input.QuestionCode))
            {
                ErrorMessage.Add("Please Fill in the Question Id!");
            }

            if (string.IsNullOrWhiteSpace(input.Question))
            {
                ErrorMessage.Add("Please Fill in the Question");
            }

            if (ErrorMessage.Any())
            {
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = string.Join("\r\n", ErrorMessage);

                return resultDto;
            }

            #endregion

            #region 驗證同一問題類型是否有重複的QuestionCode

            if (_questionsRepository.GetAll().Where(x => x.Id != input.Id && x.QuestionCode == input.QuestionCode).Any())
            {
                resultDto.Data = false;
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "The same Question Type already exists with the same Question Id,please input again!";

                return resultDto;
            }

            #endregion

            try
            {
                #region 存儲Questions資料

                var _Questions = new Questions();

                if (input.Id == 0)
                {
                    #region 新增Questions資料

                    _Questions = ObjectMapper.Map<Questions>(input);

                    //設置QuestionsAnswer狀態為Active
                    foreach (var item in _Questions.QuestionsAnswers)
                    {
                        item.Status = QuestionsAnswerStatus.Active.ToString();
                    }

                    #endregion
                }
                else
                {
                    #region 更新Questions資料

                    var DBquestions = _questionsRepository.GetAllIncluding(x => x.QuestionsAnswers).Where(x => x.Id == input.Id).FirstOrDefault();

                    //DB已有的QuestionsAnswer
                    List<QuestionsAnswer> DBQuestionsAnswer = new List<QuestionsAnswer>();
                    DBQuestionsAnswer.AddRange(DBquestions.QuestionsAnswers);

                    _Questions = ObjectMapper.Map(input, DBquestions);

                    #region 設置QuestionsAnswers資料

                    _Questions.QuestionsAnswers = new List<QuestionsAnswer>();

                    //把舊有已刪除的資料先加入QuestionsAnswers，如果不加DB會清掉Question_Id的值
                    _Questions.QuestionsAnswers.AddRange(DBQuestionsAnswer.Where(x => x.Status == QuestionsAnswerStatus.InActive.ToString()).ToList());

                    //input中已有的DB QuestionsAnswerIds
                    List<int> QuestionsAnswerIds = input.QuestionsAnswers.Where(x => x.Id != 0).Select(x => x.Id).ToList();

                    //此次刪除的Questions Answers資料，修改Status為InActive，加入QuestionsAnswers
                    foreach (var item in DBQuestionsAnswer.Where(x => x.Status != QuestionsAnswerStatus.InActive.ToString() && !QuestionsAnswerIds.Contains(x.Id)))
                    {
                        item.Status = QuestionsAnswerStatus.InActive.ToString();

                        _Questions.QuestionsAnswers.Add(item);
                    }

                    foreach (var item in input.QuestionsAnswers)
                    {
                        if (item.Id != 0)
                        {
                            //修改Questions Answers
                            foreach (var _QuestionsAnswers in DBQuestionsAnswer.Where(x => x.Status != QuestionsAnswerStatus.InActive.ToString() && QuestionsAnswerIds.Contains(x.Id)))
                            {
                                if (item.Id == _QuestionsAnswers.Id)
                                {
                                    _QuestionsAnswers.AnswerContent = item.AnswerContent;
                                    _QuestionsAnswers.RecommendedScore = item.RecommendedScore;

                                    _Questions.QuestionsAnswers.Add(_QuestionsAnswers);
                                }
                            }
                        }
                        else
                        {
                            //新增Questions Answers
                            QuestionsAnswer questionsAnswer = ObjectMapper.Map<QuestionsAnswer>(item);
                            questionsAnswer.Status = QuestionsAnswerStatus.Active.ToString();

                            _Questions.QuestionsAnswers.Add(questionsAnswer);
                        }
                    }

                    #endregion

                    #endregion

                    #region 重置該問題的所有問卷答案

                    //找出該問題所有的問卷Id
                    var questionnairesIds = _questionnairesAsignRepository.GetAll().Where(x => x.Question_Id == input.Id).Select(x => x.Questionnaire_Id).ToList();

                    //找出需要修改的問卷（status不為Finished）
                    var updateQuestionnairesIds = _questionnairesRepository.GetAll().Where(x => x.Status != QuestionnairesStatus.Finished.ToString() && questionnairesIds.Contains(x.Id)).Select(x => x.Id).ToList();

                    var questionnairesAsignList = _questionnairesAsignRepository.GetAll().Where(x => updateQuestionnairesIds.Contains(x.Questionnaire_Id.Value) && x.Question_Id == input.Id).ToList();

                    if (questionnairesAsignList.Any())
                    {
                        UpdateQuestionnaires(questionnairesAsignList);
                    }

                    #endregion
                }

                _Questions.QuestionType_Id = input.QuestionTypeId;

                int Id = _questionsRepository.InsertOrUpdateAndGetId(_Questions);

                #endregion

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

        /// <summary>
        /// 重置該問題的所有問卷答案
        /// </summary>
        /// <param name="parQuestionnairesAsignList">Questionnaires Asign 資料</param>
        private void UpdateQuestionnaires(List<QuestionnairesAsign> parQuestionnairesAsignList)
        {
            #region 重置該問題的所有問卷的 Answer 和 FreeText

            foreach (var item in parQuestionnairesAsignList)
            {
                item.SelectedAnswer = "";
                item.FreeText = "";
                _questionnairesAsignRepository.Update(item);
            }

            #endregion

            #region 移除該問題的所有問卷的 Supporting Document

            var DeleteQuestionnairesAsignIds = parQuestionnairesAsignList.Select(x => x.Id).ToList();

            _supportingDocumentRepository.Delete(x => DeleteQuestionnairesAsignIds.Contains(x.QuestionnairesAsign_Id));

            #endregion
        }
    }
}
