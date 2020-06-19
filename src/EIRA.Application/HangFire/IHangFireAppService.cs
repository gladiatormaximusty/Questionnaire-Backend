using Abp.Application.Services;

namespace EIRA.HangFire
{
    public interface IHangFireAppService : IApplicationService
    {
        /// <summary>
        /// 創建或更新JOB
        /// </summary>
        /// <returns></returns>
        string AddOrUpdateJob();

        /// <summary>
        /// 刪除JOB
        /// </summary>
        /// <returns></returns>
        string ClosedJob();

        /// <summary>
        /// 立即執行
        /// </summary>
        /// <returns></returns>
        string StartJob();
    }
}
