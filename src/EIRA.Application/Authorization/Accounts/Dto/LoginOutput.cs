namespace EIRA.Authorization.Accounts.Dto
{
    public class LoginOutput
    {
        /// <summary>
        /// 登錄憑證
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 是否需強制更改密碼
        /// </summary>
        public bool IsForceChangPwd { get; set; }

        /// <summary>
        /// 頭像
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 當前登入人姓名
        /// </summary>
        public string CurrentUser { get; set; }
    }
}
