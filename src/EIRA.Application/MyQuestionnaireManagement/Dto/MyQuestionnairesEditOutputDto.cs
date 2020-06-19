using System.Collections.Generic;

namespace EIRA.MyQuestionnaireManagement.Dto
{
    public class MyQuestionnairesEditOutputDto
    {
        /// <summary>
        /// 已做答問題總數
        /// </summary>
        public int HasBeenAnsweredCount { get; set; }

        /// <summary>
        /// 問題總數
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 是否只讀
        /// Status:Finished 只能查看
        /// Admin System 進入也是只能查看，要登入回答問卷系統才能編輯
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// 問題
        /// </summary>
        public List<MyQuestionnaireEditDto> Questionnaire { get; set; }
    }
}
