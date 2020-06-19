using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("QuestionnairesFinished")]
    public class QuestionnairesFinished : Entity<int>
    {
        /// <summary>
        /// Questionnaires Id
        /// </summary>
        public int QuestionnairesId { get; set; }

        /// <summary>
        /// BU Id
        /// </summary>
        public int BUId { get; set; }

        /// <summary>
        /// BU Name
        /// </summary>
        public string BUName { get; set; }

        /// <summary>
        /// Entity Questions Json
        /// </summary>
        public string EntityQuestionsJson { get; set; }
    }
}
