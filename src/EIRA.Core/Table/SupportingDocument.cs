using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace EIRA.Table
{
    [Table("SupportingDocument")]
    public class SupportingDocument : Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// PathUrl
        /// </summary>		
        public string PathUrl { get; set; }

        /// <summary>
        /// Type
        /// </summary>		
        public string Type { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>		
        public string ContentType { get; set; }

        /// <summary>
        /// QuestionnairesAsign_Id
        /// </summary>
        public int QuestionnairesAsign_Id { get; set; }
    }
}
