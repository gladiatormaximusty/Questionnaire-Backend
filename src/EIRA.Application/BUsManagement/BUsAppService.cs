using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Linq.Extensions;
using EIRA.Authorization;
using EIRA.Authorization.Users;
using EIRA.BUsManagement.Dto;
using EIRA.Enums;
using EIRA.ResultDto;
using EIRA.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EIRA.BUsManagement
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class BUsAppService : EIRAAppServiceBase, IBUsAppService
    {
        IRepository<BUs> _bUsRepository;
        private readonly UserManager _userManager;

        public BUsAppService(IRepository<BUs> bUsRepository, UserManager userManager)
        {
            _bUsRepository = bUsRepository;
            _userManager = userManager;
        }

        public ResultsDto<List<BUsOutputDto>> GetAll(BUsInputDto input)
        {
            ResultsDto<List<BUsOutputDto>> resultDto = new ResultsDto<List<BUsOutputDto>>();

            try
            {
                var query = from bUs in _bUsRepository.GetAll()
                            select new BUsOutputDto
                            {
                                Id = bUs.Id,
                                BUName = bUs.BUName,
                                Status = bUs.Status
                            };

                query = query.WhereIf(input.IsOnlyShowActiveStatus, x => x.Status == BUstatus.Active.ToString());

                resultDto.Data = query.ToList();
                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }

        public ResultsDto<bool> InsertOrUpdate(List<BUsDto> input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                //input的所有舊有的DB BUsIds
                List<int> BUsIds = input.Where(x => x.Id != 0).Select(x => x.Id).ToList();

                //當前刪除的BUsId
                List<int> DeletedIds = _bUsRepository.GetAll().Where(x => x.Status != BUstatus.InActive.ToString() && !BUsIds.Contains(x.Id)).Select(x => x.Id).ToList();

                #region 檢驗BU下是否有User，有User不可以刪除

                if (DeletedIds.Any())
                {
                    var user = _userManager.Users.Where(x => DeletedIds.Contains(x.BU_Id.Value)).ToList();

                    if (user != null && user.Any())
                    {
                        var UserBUIds = user.Select(x => x.BU_Id).Distinct().ToList();

                        var HasUserBU = _bUsRepository.GetAll().Where(x => UserBUIds.Contains(x.Id)).ToList();

                        resultDto.Data = false;
                        resultDto.Status.Code = InternalServerError;
                        resultDto.Status.Message = string.Format("User already exists BU:{0}, cannot delete!", string.Join("，", HasUserBU.Select(x => x.BUName).ToList()));

                        return resultDto;
                    }
                }

                #endregion

                #region 刪除的BU（刪除只是把Status改爲InActive）

                foreach (var Id in DeletedIds)
                {
                    var BU = _bUsRepository.FirstOrDefault(x => x.Id == Id);
                    BU.Status = BUstatus.InActive.ToString();

                    _bUsRepository.Update(BU);
                }

                #endregion

                //新增或更新的BU資料
                List<BUsDto> AddBUsList = input.Where(x => x.Id == 0).ToList();

                foreach (var BUsDto in input)
                {
                    var _BUs = new BUs();

                    if (BUsDto.Id == 0)
                    {
                        #region 新增BU資料

                        _BUs = ObjectMapper.Map<BUs>(BUsDto);

                        _BUs.Status = BUstatus.Active.ToString();

                        #endregion
                    }
                    else
                    {
                        #region 更新BU資料

                        _BUs = _bUsRepository.FirstOrDefault(x => x.Id == BUsDto.Id);

                        _BUs = ObjectMapper.Map(BUsDto, _BUs);

                        #endregion
                    }

                    _bUsRepository.InsertOrUpdate(_BUs);
                }

                resultDto.Data = true;
                resultDto.Status.Code = Succeed;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e);
                resultDto.Data = false;
                resultDto.Status.Code = InternalServerError;
                resultDto.Status.Message = "System Error";
            }

            return resultDto;
        }
    }
}
