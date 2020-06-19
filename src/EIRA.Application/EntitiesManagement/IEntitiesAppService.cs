using Abp.Application.Services;
using EIRA.EntitiesManagement.Dto;
using EIRA.ResultDto;
using System.Collections.Generic;

namespace EIRA.EntitiesManagement
{
    public interface IEntitiesAppService : IApplicationService
    {
        /// <summary>
        /// 取得所有的Entities資料
        /// </summary>
        /// <returns></returns>
        ResultsDto<List<EntitiesOutputDto>> GetAll(EntitiesInputDto input);

        /// <summary>
        /// 更新Entities資料
        /// </summary>
        /// <param name="input">Entities 資料</param>
        /// <returns></returns>
        ResultsDto<bool> InsertOrUpdate(List<EntitiesDto> input);
    }
}
