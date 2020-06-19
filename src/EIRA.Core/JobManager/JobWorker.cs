using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using EIRA.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIRA.JobManager
{
    public class JobWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        IRepository<Questionnaires> _questionnairesRepository;

        public JobWorker(AbpTimer timer, IRepository<Questionnaires> questionnairesRepository)
            : base(timer)
        {
            timer.Period = (int)TimeSpan.FromDays(1).TotalMilliseconds;

            _questionnairesRepository = questionnairesRepository;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            try
            {
                var list = _questionnairesRepository.GetAllList(x => x.Status == "Pending" && DateTime.Today > x.SubmissionDeadline);

                foreach (var item in list)
                {
                    item.Status = "Reviewing";
                    _questionnairesRepository.Update(item);
                }
            }
            catch (Exception e)
            {
                Logger.Info("JobWorker Error:" + e);
            }
        }
    }
}
