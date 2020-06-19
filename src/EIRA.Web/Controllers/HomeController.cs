using Abp.Domain.Repositories;
using Abp.Web.Mvc.Authorization;
using EIRA.Table;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Web;
using System.Web.Mvc;

namespace EIRA.Web.Controllers
{
    [AbpMvcAuthorize]
    public class HomeController : EIRAControllerBase
    {
        IRepository<QuestionTypes> _questionTypesRepository;
        IRepository<Questions> _questionsRepository;
        IRepository<QuestionsAnswer> _questionsAnswerRepository;

        public HomeController(IRepository<QuestionTypes> questionTypesRepository, IRepository<Questions> questionsRepository, IRepository<QuestionsAnswer> questionsAnswerRepository)
        {
            _questionTypesRepository = questionTypesRepository;
            _questionsRepository = questionsRepository;
            _questionsAnswerRepository = questionsAnswerRepository;
        }

        public ActionResult Index()
        {
            return View("~/App/Main/views/layout/layout.cshtml"); //Layout of the angular application.
        }

        [HttpPost]
        public ActionResult UpLoadQuestionInfo(HttpPostedFileBase File)
        {
            if (File == null || File.ContentLength == 0)
            {
                throw new Exception("無法取得上傳檔案");
            }
            else
            {
                //創建工作簿對象
                XSSFWorkbook workbook = new XSSFWorkbook(File.InputStream);

                //讀取工作簿第一張表(此處參數可為下標，也可為表名)
                ISheet sheet = workbook.GetSheetAt(0);

                //新建當前工作表行數據
                IRow row;

                //i從1開始，i=0是Title
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    //row讀入第i行數據
                    row = sheet.GetRow(i);

                    #region Question Type

                    string QuestionTypeID = row.GetCell(1) == null ? "" : row.GetCell(1).ToString().Trim();
                    string QuestionType = row.GetCell(2) == null ? "" : row.GetCell(2).ToString().Trim();

                    int questionTypeId = 0;

                    if (!string.IsNullOrWhiteSpace(QuestionTypeID))
                    {
                        var DBquestionTypes = _questionTypesRepository.FirstOrDefault(x => x.QuestionTypeCode == QuestionTypeID);

                        if (DBquestionTypes == null)
                        {
                            DBquestionTypes = new QuestionTypes();
                            DBquestionTypes.QuestionTypeCode = QuestionTypeID;
                        }

                        DBquestionTypes.Status = "Active";
                        DBquestionTypes.QuestionTypeName = QuestionType;

                        questionTypeId = _questionTypesRepository.InsertOrUpdateAndGetId(DBquestionTypes);
                    }

                    #endregion

                    #region Question

                    string QuestionID = row.GetCell(3) == null ? "" : row.GetCell(3).ToString().Trim();
                    string Question = row.GetCell(4) == null ? "" : row.GetCell(4).ToString().Trim();
                    string HighestRating = row.GetCell(5) == null ? "" : row.GetCell(5).ToString().Trim();
                    string ScoringMethod = row.GetCell(6) == null ? "" : row.GetCell(6).ToString().Trim();
                    string FreeText = row.GetCell(7) == null ? "" : row.GetCell(7).ToString().Trim();
                    string FreeTextMandatory = row.GetCell(8) == null ? "" : row.GetCell(8).ToString().Trim();
                    string FreeTextDescription = row.GetCell(9) == null ? "" : row.GetCell(9).ToString().Trim();
                    string SupportingDocument = row.GetCell(10) == null ? "" : row.GetCell(10).ToString().Trim();
                    string SupportingDocumentMandatory = row.GetCell(11) == null ? "" : row.GetCell(11).ToString().Trim();

                    string MC = row.GetCell(12) == null ? "" : row.GetCell(12).ToString().Trim();
                    string MCMandatory = row.GetCell(13) == null ? "" : row.GetCell(13).ToString().Trim();
                    string MCAns1Description = row.GetCell(14) == null ? "" : row.GetCell(14).ToString().Trim();
                    string MCAns1RecommendedScore = row.GetCell(15) == null ? "" : row.GetCell(15).ToString().Trim();
                    string MCAns2Description = row.GetCell(16) == null ? "" : row.GetCell(16).ToString().Trim();
                    string MCAns2RecommendedScore = row.GetCell(17) == null ? "" : row.GetCell(17).ToString().Trim();
                    string MCAns3Description = row.GetCell(18) == null ? "" : row.GetCell(18).ToString().Trim();
                    string MCAns3RecommendedScore = row.GetCell(19) == null ? "" : row.GetCell(19).ToString().Trim();
                    string MCAns4Description = row.GetCell(20) == null ? "" : row.GetCell(20).ToString().Trim();
                    string MCAns4RecommendedScore = row.GetCell(21) == null ? "" : row.GetCell(21).ToString().Trim();
                    string MCAns5Description = row.GetCell(22) == null ? "" : row.GetCell(22).ToString().Trim();
                    string MCAns5RecommendedScore = row.GetCell(23) == null ? "" : row.GetCell(23).ToString().Trim();

                    if (questionTypeId != 0)
                    {
                        var DBquestion = _questionsRepository.FirstOrDefault(x => x.QuestionCode == QuestionID && x.QuestionType_Id == questionTypeId);

                        if (DBquestion == null)
                        {
                            DBquestion = new Questions();
                            DBquestion.QuestionCode = QuestionID;
                            DBquestion.QuestionType_Id = questionTypeId;
                        }

                        DBquestion.Status = "Active";
                        DBquestion.Question = Question;
                        DBquestion.HighestRating = HighestRating;
                        DBquestion.ScoringMethod = ScoringMethod == "N/A" ? "Maximum" : ScoringMethod;
                        DBquestion.HasFreeText = FreeText.ToUpper() == "Y";
                        DBquestion.FreeTextPlaceholder = FreeTextDescription;
                        DBquestion.IsFreeTextMandatory = FreeTextMandatory.ToUpper() == "Y";
                        DBquestion.IsFreeTextNumeric = false;
                        DBquestion.HasSupportingDocument = SupportingDocument.ToUpper() == "Y";
                        DBquestion.IsSupportingDocumentMandatory = SupportingDocumentMandatory.ToUpper() == "Y";
                        DBquestion.HasAnswer = MC.ToUpper() == "Y";
                        DBquestion.IsAnswerMandatory = MCMandatory.ToUpper() == "Y";

                        int questionId = _questionsRepository.InsertOrUpdateAndGetId(DBquestion);

                        if (DBquestion.HasAnswer)
                        {
                            if (!string.IsNullOrWhiteSpace(MCAns1Description))
                            {
                                var answer = _questionsAnswerRepository.FirstOrDefault(x => x.Question_Id == questionId && x.AnswerContent == MCAns1Description);
                                if (answer == null)
                                {
                                    answer = new QuestionsAnswer();
                                    answer.Question_Id = questionId;
                                    answer.AnswerContent = MCAns1Description;
                                }

                                answer.Status = "Active";
                                answer.RecommendedScore = MCAns1RecommendedScore;

                                _questionsAnswerRepository.InsertOrUpdate(answer);
                            }

                            if (!string.IsNullOrWhiteSpace(MCAns2Description))
                            {
                                var answer = _questionsAnswerRepository.FirstOrDefault(x => x.Question_Id == questionId && x.AnswerContent == MCAns2Description);
                                if (answer == null)
                                {
                                    answer = new QuestionsAnswer();
                                    answer.Question_Id = questionId;
                                    answer.AnswerContent = MCAns2Description;
                                }

                                answer.Status = "Active";
                                answer.RecommendedScore = MCAns1RecommendedScore;

                                _questionsAnswerRepository.InsertOrUpdate(answer);
                            }

                            if (!string.IsNullOrWhiteSpace(MCAns3Description))
                            {
                                var answer = _questionsAnswerRepository.FirstOrDefault(x => x.Question_Id == questionId && x.AnswerContent == MCAns3Description);
                                if (answer == null)
                                {
                                    answer = new QuestionsAnswer();
                                    answer.Question_Id = questionId;
                                    answer.AnswerContent = MCAns3Description;
                                }

                                answer.Status = "Active";
                                answer.RecommendedScore = MCAns3RecommendedScore;

                                _questionsAnswerRepository.InsertOrUpdate(answer);
                            }

                            if (!string.IsNullOrWhiteSpace(MCAns4Description))
                            {
                                var answer = _questionsAnswerRepository.FirstOrDefault(x => x.Question_Id == questionId && x.AnswerContent == MCAns4Description);
                                if (answer == null)
                                {
                                    answer = new QuestionsAnswer();
                                    answer.Question_Id = questionId;
                                    answer.AnswerContent = MCAns4Description;
                                }

                                answer.Status = "Active";
                                answer.RecommendedScore = MCAns4RecommendedScore;

                                _questionsAnswerRepository.InsertOrUpdate(answer);
                            }

                            if (!string.IsNullOrWhiteSpace(MCAns5Description))
                            {
                                var answer = _questionsAnswerRepository.FirstOrDefault(x => x.Question_Id == questionId && x.AnswerContent == MCAns5Description);
                                if (answer == null)
                                {
                                    answer = new QuestionsAnswer();
                                    answer.Question_Id = questionId;
                                    answer.AnswerContent = MCAns5Description;
                                }

                                answer.Status = "Active";
                                answer.RecommendedScore = MCAns5RecommendedScore;

                                _questionsAnswerRepository.InsertOrUpdate(answer);
                            }
                        }
                    }

                    #endregion
                }
            }

            return RedirectToAction("Index","Home");
        }
    }
}