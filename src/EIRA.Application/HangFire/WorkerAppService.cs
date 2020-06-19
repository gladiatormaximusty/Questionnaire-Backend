using Abp.Domain.Repositories;
using EIRA.Table;
using System;

namespace EIRA.HangFire
{
    public class WorkerAppService : EIRAAppServiceBase, IWorkerAppService
    {
        IRepository<Questionnaires> _questionnairesRepository;
        public WorkerAppService(IRepository<Questionnaires> questionnairesRepository)
        {
            _questionnairesRepository = questionnairesRepository;
        }

        public void ChangeQuestionnaires()
        {
            try
            {
                Logger.Info("JobWorker Time:" + DateTime.Now);

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
