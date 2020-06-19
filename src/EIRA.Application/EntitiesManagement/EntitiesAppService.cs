using Abp.Authorization;
using Abp.Domain.Repositories;
using EIRA.EntitiesManagement.Dto;
using EIRA.Enums;
using EIRA.ResultDto;
using EIRA.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Linq.Extensions;
using EIRA.Authorization;

namespace EIRA.EntitiesManagement
{
    [AbpAuthorize(PermissionNames.Pages_Admin)]
    public class EntitiesAppService : EIRAAppServiceBase, IEntitiesAppService
    {
        IRepository<Entities> _entitiesRepository;

        public EntitiesAppService(IRepository<Entities> entitiesRepository)
        {
            _entitiesRepository = entitiesRepository;
        }

        public ResultsDto<List<EntitiesOutputDto>> GetAll(EntitiesInputDto input)
        {
            ResultsDto<List<EntitiesOutputDto>> resultDto = new ResultsDto<List<EntitiesOutputDto>>();

            try
            {
                var query = from entities in _entitiesRepository.GetAll()
                            select new EntitiesOutputDto
                            {
                                Id = entities.Id,
                                EntityName = entities.EntityName,
                                Status = entities.Status
                            };

                query = query.WhereIf(input.IsOnlyShowActiveStatus, x => x.Status == EntitiesStatus.Active.ToString());

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

        public ResultsDto<bool> InsertOrUpdate(List<EntitiesDto> input)
        {
            ResultsDto<bool> resultDto = new ResultsDto<bool>();

            try
            {
                //input的所有舊有的DB EntitiesIds
                List<int> EntitiesIds = input.Where(x => x.Id != 0).Select(x => x.Id).ToList();

                #region 刪除的Entities（刪除只是把Status改爲InActive）

                //當前刪除的EntitiesId
                List<int> DeletedIds = _entitiesRepository.GetAll().Where(x => x.Status != EntitiesStatus.InActive.ToString() && !EntitiesIds.Contains(x.Id)).Select(x => x.Id).ToList();

                foreach (var Id in DeletedIds)
                {
                    var Entities = _entitiesRepository.FirstOrDefault(x => x.Id == Id);
                    Entities.Status = EntitiesStatus.InActive.ToString();

                    _entitiesRepository.Update(Entities);
                }

                #endregion

                //新增或更新的Entities資料
                List<EntitiesDto> AddEntitiesList = input.Where(x => x.Id == 0).ToList();

                foreach (var EntitiesDto in input)
                {
                    var _Entities = new Entities();

                    if (EntitiesDto.Id == 0)
                    {
                        #region 新增Entities資料

                        _Entities = ObjectMapper.Map<Entities>(EntitiesDto);

                        _Entities.Status = EntitiesStatus.Active.ToString();

                        #endregion
                    }
                    else
                    {
                        #region 更新Entities資料

                        _Entities = _entitiesRepository.FirstOrDefault(x => x.Id == EntitiesDto.Id);

                        _Entities = ObjectMapper.Map(EntitiesDto, _Entities);

                        #endregion
                    }

                    _entitiesRepository.InsertOrUpdate(_Entities);
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
