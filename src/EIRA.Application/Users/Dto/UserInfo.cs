using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using EIRA.Authorization.Users;
using EIRA.Table;

namespace EIRA.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserInfo : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }

        [MaxLength(50)]
        public string Account { get; set; }

        [MaxLength(1024 * 64)]
        public string Image { get; set; }

        public int? BU_Id { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 當前修改帳號是否是當前登錄人
        /// </summary>
        public bool IsCurrentLoginUser { get; set; }
    }
}
