using Abp.Domain.Repositories;
using Abp.Domain.Services;
using EIRA.Authorization.Users;

namespace EIRA.TableManager
{
    public class AdminUserManager : IDomainService
    {
        private readonly IRepository<User, long> _repository;

        public AdminUserManager(IRepository<User, long> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 根据Id返回姓名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserNameById(long? UserId)
        {
            string Name = "";
            var UserInfo = _repository.FirstOrDefault(x => x.Id == UserId);

            if (UserInfo != null)
                Name = UserInfo.Name + " " + UserInfo.Surname;
            return Name;
        }
    }
}
