using System;
using System.Net;
using Core;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Message;

namespace shunda.Service.CustomerMessage
{
    [Service(ServiceType.Scoped)]
    public class SmtpMailService
    {
        private SmtpConfigService smtpConfigService = null;

        public SmtpMailService(SmtpConfigService smtpConfigService)
        {
            this.smtpConfigService = smtpConfigService;
        }

        public void SendMail(string addressSendTo, string subject, string body)
        {
            SmtpConfigInfo smtpConfig = smtpConfigService.GetSmtpConfig();

            int.TryParse(smtpConfig.SmtpPort, out int smtpPort);

            SmtpClient smtpClient = new SmtpClient(smtpConfig.SmtpServer, smtpPort);
            smtpClient.Host = smtpConfig.SmtpServer;
            smtpClient.Port = smtpPort;
            smtpClient.Credentials = new NetworkCredential(smtpConfig.SmtpUser, smtpConfig.SmtpPwd);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailAddress addressFrom = new MailAddress(smtpConfig.AddressFrom, smtpConfig.NickNameFrom);
            MailAddress addressTo = new MailAddress(addressSendTo);

            MailMessage mailMessage = new MailMessage(addressFrom, addressTo);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Normal;

            Task taskSendMail = smtpClient.SendMailAsync(mailMessage);
            taskSendMail.Wait();
        }
    }
}