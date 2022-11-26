using System.Net;
using System.Net.Mail;
using System.Text;
using shunda.Core.Config;
using shunda.Core.Service;

namespace shunda.Service.CustomerMessage;

[Service(ServiceType.Scoped)]
public class SmtpMailService
{
    public void SendMail(string addressSendTo, string subject, string body)
    {
        SmtpClient smtpClient = new SmtpClient() {
            Host = SmtpConfig.SmtpServer,
            Port = SmtpConfig.SmtpPort,
            Credentials = new NetworkCredential(SmtpConfig.SmtpUser, SmtpConfig.SmtpPwd),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = true,
        };

        MailAddress addressFrom = new MailAddress(SmtpConfig.AddressFrom, SmtpConfig.NickNameFrom);
        MailAddress addressTo = new MailAddress(addressSendTo);

        MailMessage mailMessage = new MailMessage(addressFrom, addressTo) {
            Subject = subject,
            Body = body,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true,
            Priority = MailPriority.Normal,
        };

        Task taskSendMail = smtpClient.SendMailAsync(mailMessage);
        taskSendMail.Wait();
    }
}