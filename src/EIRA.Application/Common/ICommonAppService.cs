using Abp.Application.Services;
using EIRA.ResultDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EIRA.Common
{
    public interface ICommonAppService : IApplicationService
    {
        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <returns></returns>
        ResultsDto<string> UploadImage();

        /// <summary>
        /// 上傳文件
        /// </summary>
        /// <returns></returns>
        ResultsDto<string> UploadAccessory();
    }
}
