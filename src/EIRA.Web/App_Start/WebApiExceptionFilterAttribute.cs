using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace EIRA.Web.App_Start
{
    public class WebApiExceptionFilterAttribute: ExceptionFilterAttribute
    {
        //重写基类的异常处理方法
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is Abp.Authorization.AbpAuthorizationException)
            {
                //var exp = actionExecutedContext.Exception as Abp.Authorization.AbpAuthorizationException;

                if (HttpContext.Current.User.Identity.IsAuthenticated == false)
                {
                    string Content = "{\"data\": {\"success\": false },\"status\": {\"code\": 401, \"message\": \"" + actionExecutedContext.Exception.Message + "\"}}";
                    //Content = string.Format(Content, actionExecutedContext.Exception.Message);
                    var oResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    oResponse.Content = new StringContent(Content);
                    oResponse.ReasonPhrase = actionExecutedContext.Exception.Message;
                    actionExecutedContext.Response = oResponse;
                }
                else
                {
                    string Content = "{\"data\": {\"success\": false },\"status\": {\"code\": 403, \"message\": \"" + actionExecutedContext.Exception.Message + "\"}}";
                    //Content = string.Format(Content, actionExecutedContext.Exception.Message);
                    var oResponse = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    oResponse.Content = new StringContent(Content);
                    oResponse.ReasonPhrase = actionExecutedContext.Exception.Message;
                    actionExecutedContext.Response = oResponse;
                }
            }
            else
            {
                string Content = "{\"data\": {\"success\": false },\"status\": {\"code\": 500, \"message\": \"" + actionExecutedContext.Exception.Message + "\"}}";
                //Content = string.Format(Content, actionExecutedContext.Exception.Message);
                var oResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                oResponse.Content = new StringContent(Content);
                oResponse.ReasonPhrase = actionExecutedContext.Exception.Message;
                actionExecutedContext.Response = oResponse;
            }

            base.OnException(actionExecutedContext);
        }

    }


}