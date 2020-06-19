using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EIRA.Authorization;
using EIRA.Common;
using EIRA.Enums;
using EIRA.QuestionTypesManagement.Dto;
using EIRA.ResultDto;
using EIRA.Table;
using EIRA.TableManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace EIRA.QuestionTypesManagement
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class QuestionTypesAppService : EIRAAppServiceBase, IQuestionTypesAppService
    {
        IRepository<QuestionTypes> _questionTypesRepository;
        IRepository<Questions> _questionsRepository;
        AdminUserManager _adminUserManager;

        public QuestionTypesAppService(IRepository<QuestionTypes> questionTypesRepository, AdminUserManager adminUserManager, IRepository<Questions> questionsRepository)
        {
            _questionTypesRepository = questionTypesRepository;
            _adminUserManager = adminUserManager;
            _questionsRepository = questionsRepository;
        }

        public ResultsDto<List<QuestionTypeAllOutputDto>> GetAll()
        {
            ResultsDto<List<QuestionTypeAllOutputDto>> resultDto = new ResultsDto<List<QuestionTypeAllOutputDto>>();

            try
            {
                var query = from questionTypes in _questionTypesRepository.GetAll().Where(x => x.Status == QuestionTypesStatus.Active.ToString())
                            select new QuestionTypeAllOutputDto
                            {
                                QuestionTypeId = questionTypes.Id,
                                QuestionTypeName = questionTypes.QuestionTypeName,
                            };

                //排序
                query = query.OrderBy("QuestionTypeName");

                resultDto.Data = query.ToList();

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

        public ResultsDto<PagedResultDto<QuestionTypesOutputDto>> GetPagedAll(QuestionTypesInputDto input)
        {
            ResultsDto<PagedResultDto<QuestionTypesOutputDto>> resultDto = new ResultsDto<PagedResultDto<QuestionTypesOutputDto>>();

            try
            {
                var query = from questionTypes in _questionTypesRepository.GetAll()
                            select new QuestionTypesOutputDto
                            {
                                Id = questionTypes.Id,
                                QuestionTypeCode = questionTypes.QuestionTypeCode,
                                QuestionTypeName = questionTypes.QuestionTypeName,
                                Status = questionTypes.Status,
                                NumberOfQuestions = 0
                            };

                //查詢條件
                query = query.WhereIf(input.IsOnlyShowActiveStatus, x => x.Status == QuestionTypesStatus.Active.ToString());
                query = query.WhereIf(!input.QuestionTypeName.IsNullOrWhiteSpace(), x => x.QuestionTypeCode.Contains(input.QuestionTypeName.Trim()) || x.QuestionTypeName.Contains(input.QuestionTypeName.Trim()));

                if (string.IsNullOrWhiteSpace(input.Sorting))
                {
                    input.Sorting = "QuestionTypeCode";
                }

                //排序
                query = query.OrderBy(input.Sorting);

                //记录总数
                var resultCount = query.Count();

                var questionTypesList = query.ToList();

                #region 特殊處理 Number Of Questions

                foreach (var item in questionTypesList)
                {
                    item.NumberOfQuestions = _questionsRepository.GetAll().Where(x => x.QuestionType_Id == item.Id && x.Status != QuestionsStatus.InActive.ToString()).ToList().Count;
                }

                if (input.Sorting == "numberOfQuestions desc")
                {
                    questionTypesList = questionTypesList.OrderByDescending(x => x.NumberOfQuestions).ToList();
                }
                else if (input.Sorting == "numberOfQuestions asc")
                {
                    questionTypesList = questionTypesList.OrderBy(x => x.NumberOfQuestions).ToList();
                }

                #endregion

                //分頁
                questionTypesList = ExtensionHelper.GetPaging(questionTypesList, input.SkipCount, input.MaxResultCount);

                resultDto.Data = new PagedResultDto<QuestionTypesOutputDto>(resultCount, questionTypesList);

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

        public ResultsDto<QuestionTypesForEditOutputDto> GetQuestionTypesById(NullableIdDto input)
        {
            ResultsDto<QuestionTypesForEditOutputDto> resultDto = new ResultsDto<QuestionTypesForEditOutputDto>();

            try
            {
                QuestionTypesForEditOutputDto _QuestionTypesForEditOutputDto = new QuestionTypesForEditOutputDto(); ;

                if (input.Id == 0)
                {
                    //add new Question Type頁面返回UpdateTime,UpdateUser給前端顯示
                    _QuestionTypesForEditOutputDto.UpdateTime = DateTime.Now.ToEIRATime();
                    _QuestionTypesForEditOutputDto.UpdateUser = _adminUserManager.GetUserNameById(AbpSession.UserId);
                }
                else
                {
                    var _QuestionTypes = _questionTypesRepository.FirstOrDefault(input.Id.Value);

                    _QuestionTypesForEditOutputDto = ObjectMapper.Map<QuestionTypesForEditOutputDto>(_QuestionTypes);

                    if (_QuestionTypes != null)
                    {
                        if (_QuestionTypes.LastModificationTime.HasValue)
                        {
                            _QuestionTypesForEditOutputDto.UpdateTime = _QuestionTypes.LastModificationTime.Value.ToEIRATime();
                            _QuestionTypesForEditOutputDto.UpdateUser = _adminUserManager.GetUserNameById(_QuestionTypes.LastModifierUserId);
                        }
                        else
                        {
                            _QuestionTypesForEditOutputDto.UpdateTime = _QuestionTypes.CreationTime.ToEIRATime();
                            _QuestionTypesForEditOutputDto.UpdateUser = _adminUserManager.GetUserNameById(_QuestionTypes.CreatorUserId);
                        }
                    }
                }

                resultDto.Data = _QuestionTypesForEditOutputDto;

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

        public ResultsDto<NullableIdDto> InsertOrUpdate(QuestionTypesDto input)
        {
            ResultsDto<NullableIdDto> resultDto = new ResultsDto<NullableIdDto>();

            #region 必要項驗證

            if (string.IsNullOrWhiteSpace(input.QuestionTypeName))
            {
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "Please Fill in the Question Type Name";

                return resultDto;
            }

            #endregion

            try
            {
                var _QuestionTypes = new QuestionTypes();

                if (input.Id == 0)
                {
                    #region 新增QuestionTypes資料

                    _QuestionTypes = ObjectMapper.Map<QuestionTypes>(input);

                    #endregion
                }
                else
                {
                    #region 更新QuestionTypes資料

                    _QuestionTypes = _questionTypesRepository.FirstOrDefault(x => x.Id == input.Id);

                    _QuestionTypes = ObjectMapper.Map(input, _QuestionTypes);

                    #endregion
                }

                int Id = _questionTypesRepository.InsertOrUpdateAndGetId(_QuestionTypes);

                resultDto.Data = new NullableIdDto { Id = Id };

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

        public ResultsDto<bool> Delete(NullableIdDto input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                var _QuestionTypes = _questionTypesRepository.FirstOrDefault(input.Id.Value);
                _QuestionTypes.Status = QuestionTypesStatus.InActive.ToString();

                _questionTypesRepository.Update(_QuestionTypes);

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
    }
}
