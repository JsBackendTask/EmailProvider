using Azure.Messaging.ServiceBus;
using EmailProvider.Models;

namespace EmailProvider.Interfaces
{
    public interface IEmailServices
    {
        bool SendEmail(EmailRequest emailRequest);
        EmailRequest UnPackEmailRequest(ServiceBusReceivedMessage message);
    }
}