using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Extensions;
using EIRA.Dto;
using EIRA.QuestionnairesIsDone.Dto;
using EIRA.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EIRA.QuestionnairesIsDone
{
    public class QuestionnairesManager : IDomainService
    {
        IRepository<Questionnaires> _questionnairesRepository;
        IRepository<QuestionnairesAsign> _questionnairesAsignRepository;
        IRepository<Questions> _questionsRepository;
        IRepository<QuestionTypes> _questionTypesRepository;
        IRepository<SupportingDocument> _supportingDocumentRepository;
        IRepository<QuestionnairesFinished> _questionnairesFinishedRepository;

        public QuestionnairesManager(IRepository<Questionnaires> questionnairesRepository, IRepository<QuestionnairesAsign> questionnairesAsignRepository, IRepository<Questions> questionsRepository
            , IRepository<QuestionTypes> questionTypesRepository
            , IRepository<SupportingDocument> supportingDocumentRepository, IRepository<QuestionnairesFinished> questionnairesFinishedRepository)
        {
            _questionnairesRepository = questionnairesRepository;
            _questionnairesAsignRepository = questionnairesAsignRepository;
            _questionsRepository = questionsRepository;
            _questionTypesRepository = questionTypesRepository;
            _supportingDocumentRepository = supportingDocumentRepository;
            _questionnairesFinishedRepository = questionnairesFinishedRepository;
        }

        /// <summary>
        /// 計算問卷完成情況
        /// </summary>
        /// <param name="QuestionnairesId"></param>
        /// <param name="BUId"></param>
        /// <param name="EntitiesId"></param>
        /// <returns></returns>
        public List<CalQuestionnairesProgressEntity> GetCalProgressEntity(int QuestionnairesId)
        {
            List<CalQuestionnairesProgressEntity> result = new List<CalQuestionnairesProgressEntity>();

            var questionnaires = _questionnairesRepository.FirstOrDefault(QuestionnairesId);

            if (questionnaires.Status != "Finished")
            {
                #region Questionnaire Status為Drafting、Pending、Reviewing的資料查詢

                var query = from questionnairesAsign in _questionnairesAsignRepository.GetAll().Where(x => x.Questionnaire_Id == QuestionnairesId)
                            join question in _questionsRepository.GetAll().Where(x => x.Status == "Active") on questionnairesAsign.Question_Id equals question.Id
                            select new CalQuestionnairesProgressEntity
                            {
                                QuestionnairesId = questionnairesAsign.Questionnaire_Id.Value,
                                BUId = questionnairesAsign.BU_Id.Value,
                                EntitiesId = questionnairesAsign.Entity_Id.Value,
                                questionnairesAsignId = questionnairesAsign.Id,
                                HasAnswer = question.HasAnswer,
                                IsAnswerMandatory = question.IsAnswerMandatory,
                                HasFreeText = question.HasFreeText,
                                IsFreeTextMandatory = question.IsFreeTextMandatory,
                                HasSupportingDocument = question.HasSupportingDocument,
                                IsSupportingDocumentMandatory = question.IsSupportingDocumentMandatory,
                                SelectedAnswer = questionnairesAsign.SelectedAnswer,
                                FreeText = questionnairesAsign.FreeText,
                                SupportingDocumentCount = 0,
                                IsDone = false
                            };

                result = query.ToList();

                #endregion

                List<int> questionnairesAsignIds = result.Select(x => x.questionnairesAsignId).ToList();

                var DBSupportingDocumentList = _supportingDocumentRepository.GetAll().Where(x => questionnairesAsignIds.Contains(x.QuestionnairesAsign_Id)).ToList();

                //檢查問卷問題是否完成
                foreach (var CalProgress in result)
                {
                    var DBSupportingDocument = DBSupportingDocumentList.Where(x => x.QuestionnairesAsign_Id == CalProgress.questionnairesAsignId).ToList();

                    if (!string.IsNullOrWhiteSpace(CalProgress.SelectedAnswer) || !string.IsNullOrWhiteSpace(CalProgress.FreeText) || DBSupportingDocument.Any())
                    {
                        CalProgress.IsDone = true;
                    }
                }
            }
            else
            {
                #region 取出 Finished Questionnaires  Question 資料

                var FinishedQuestionnaire = _questionnairesFinishedRepository.GetAll().Where(x => x.QuestionnairesId == QuestionnairesId).ToList();

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
                                        CalQuestionnairesProgressEntity calQuestionnairesProgressEntity = new CalQuestionnairesProgressEntity();

                                        calQuestionnairesProgressEntity.QuestionnairesId = item.QuestionnairesId;
                                        calQuestionnairesProgressEntity.BUId = item.BUId;
                                        calQuestionnairesProgressEntity.EntitiesId = EntityQuestions.Id;
                                        calQuestionnairesProgressEntity.questionnairesAsignId = questions.QuestionnairesAsignId;
                                        calQuestionnairesProgressEntity.HasAnswer = questions.HasAnswer;
                                        calQuestionnairesProgressEntity.IsAnswerMandatory = questions.IsAnswerMandatory;
                                        calQuestionnairesProgressEntity.HasFreeText = questions.HasFreeText;
                                        calQuestionnairesProgressEntity.IsFreeTextMandatory = questions.IsFreeTextMandatory;
                                        calQuestionnairesProgressEntity.HasSupportingDocument = questions.HasSupportingDocument;
                                        calQuestionnairesProgressEntity.IsSupportingDocumentMandatory = questions.IsSupportingDocumentMandatory;
                                        calQuestionnairesProgressEntity.SelectedAnswer = questions.SelectedAnswer;
                                        calQuestionnairesProgressEntity.FreeText = questions.FreeText;
                                        calQuestionnairesProgressEntity.SupportingDocumentCount = questions.SupportingDocument.Count;

                                        calQuestionnairesProgressEntity.IsDone = false;

                                        if (!string.IsNullOrWhiteSpace(questions.SelectedAnswer) || !string.IsNullOrWhiteSpace(questions.FreeText) || questions.SupportingDocument.Any())
                                        {
                                            calQuestionnairesProgressEntity.IsDone = true;
                                        }

                                        result.Add(calQuestionnairesProgressEntity);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            return result;
        }
    }
}
