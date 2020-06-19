using Abp.UI;
using Hangfire;
using System;

namespace EIRA.HangFire
{
    public class HangFireAppService : EIRAAppServiceBase, IHangFireAppService
    {
        public string AddOrUpdateJob()
        {
            try
            {
                RecurringJob.AddOrUpdate<IWorkerAppService>("ChangeQuestionnaires", x => x.ChangeQuestionnaires(), "0 0 0 * * ? ", TimeZoneInfo.Local);
                return "开启周期任务";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public string ClosedJob()
        {
            try
            {
                //删除指定的周期性任务
                RecurringJob.RemoveIfExists("ChangQuestionnaires");
                return "关闭成功";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public string StartJob()
        {
            try
            {
                RecurringJob.Trigger("ChangQuestionnaires");
                return "立即执行了";
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
