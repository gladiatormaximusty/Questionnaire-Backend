using Abp.Application.Services;
using Abp.Web.Models;
using EIRA.BUsManagement.Dto;
using EIRA.ResultDto;
using System.Collections.Generic;

namespace EIRA.BUsManagement
{
    public interface IBUsAppService : IApplicationService
    {
        /// <summary>
        /// 取得所有的BUs資料
        /// </summary>
        /// <returns></returns>
        ResultsDto<List<BUsOutputDto>> GetAll(BUsInputDto input);

        /// <summary>
        /// 更新BUs資料
        /// </summary>
        /// <param name="input">BUs 資料</param>
        /// <returns></returns>
        ResultsDto<bool> InsertOrUpdate(List<BUsDto> input);
    }
}
