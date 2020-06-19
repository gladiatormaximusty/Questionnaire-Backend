using EIRA.ResultDto;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web;

namespace EIRA.Common
{
    public class CommonAppService : EIRAAppServiceBase, ICommonAppService
    {
        public string Domain = ConfigurationSettings.AppSettings["Domain"];

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public ResultsDto<string> UploadImage()
        {
            var resultDto = new ResultsDto<string>();
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                if (file == null)
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Please upload the file";
                    return resultDto;
                }

                if (!file.ContentType.Contains("image"))
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Please upload the picture";
                    return resultDto;
                }

                if (new Regex(UrlRegex).IsMatch(file.FileName))
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Image name contains URL special character escape, please select again!";
                    return resultDto;
                }

                var rs = FileHelper.UploadFile(file, "Image");
                resultDto.Data = Domain + rs;
                resultDto.Status.Code = Succeed;

                if (rs == null)
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Upload failed";
                    return resultDto;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        /// <summary>
        /// 上傳附件
        /// </summary>
        /// <returns></returns>
        public ResultsDto<string> UploadAccessory()
        {
            var resultDto = new ResultsDto<string>();
            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
                if (file == null)
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Please upload the file";
                    return resultDto;
                }

                if (new Regex(UrlRegex).IsMatch(file.FileName))
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "File name contains URL special character escape, please select again!";
                    return resultDto;
                }

                var rs = FileHelper.UploadFile(file, "Accessory");
                resultDto.Data = Domain + rs;
                resultDto.Status.Code = Succeed;
                if (rs == null)
                {
                    resultDto.Status.Code = InternalServerError;
                    resultDto.Status.Message = "Upload failed";
                    return resultDto;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }
    }
}
