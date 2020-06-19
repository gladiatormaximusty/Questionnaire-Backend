using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;
using EIRA.EntityFramework;

namespace EIRA.Migrations.SeedData
{
    public class DefaultSettingsCreator
    {
        private readonly EIRADbContext _context;

        public DefaultSettingsCreator(EIRADbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Port, "587");


            //AddSettingIfNotExists(EmailSettingNames.Smtp.Host, "smtp.qq.com");
            //AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "");
            //AddSettingIfNotExists(EmailSettingNames.Smtp.Password, "");


            //change the setting
            AddSettingIfNotExists(EmailSettingNames.Smtp.Host, "smtp.gmail.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UserName, "gladiatormaximusty999@gmail.com");
            AddSettingIfNotExists(EmailSettingNames.Smtp.Password, "maximusty7999");



            AddSettingIfNotExists(EmailSettingNames.Smtp.Domain, "");
            AddSettingIfNotExists(EmailSettingNames.Smtp.EnableSsl, "true");
            AddSettingIfNotExists(EmailSettingNames.Smtp.UseDefaultCredentials, "false");

            AddSettingIfNotExists("MailTitle", "Retrieve password");
            AddSettingIfNotExists("MailResetContent", "You have successfully changed your password. The new password is ");
            AddSettingIfNotExists("MailCreatUserContent", "Your account has been created with an initial password of ");
            AddSettingIfNotExists("MailReSetUrl", "");

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}