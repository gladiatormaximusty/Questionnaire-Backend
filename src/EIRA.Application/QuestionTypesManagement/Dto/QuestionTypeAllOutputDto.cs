using Abp.Application.Services.Dto;

namespace EIRA.QuestionTypesManagement.Dto
{
    public class QuestionTypeAllOutputDto
    {
        /// <summary>
        /// Question Type Id
        /// </summary>
        public int QuestionTypeId { get; set; }

        /// <summary>
        /// Question Type Name
        /// </summary>
        public string QuestionTypeName { get; set; }

        public bool IsSeleted { get; set; }
    }
}
