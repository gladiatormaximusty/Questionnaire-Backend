using Abp.Web.Mvc.Views;

namespace EIRA.Web.Views
{
    public abstract class EIRAWebViewPageBase : EIRAWebViewPageBase<dynamic>
    {

    }

    public abstract class EIRAWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected EIRAWebViewPageBase()
        {
            LocalizationSourceName = EIRAConsts.LocalizationSourceName;
        }
    }
}