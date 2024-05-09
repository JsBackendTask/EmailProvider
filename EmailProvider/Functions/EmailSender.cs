using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailProvider.Interfaces;
using EmailProvider.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailProvider.Functions;

public class EmailSender(ILogger<EmailSender> logger, IEmailServices emailServices)
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly IEmailServices _emailServices = emailServices;

    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email_request", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        try
        {
            var emailRequest = _emailServices.UnPackEmailRequest(message);
            if (emailRequest != null && !string.IsNullOrEmpty(emailRequest.To))
            {
                if (_emailServices.SendEmail(emailRequest))
                {
                    _logger.LogInformation($"Email sent to {emailRequest.To}");
                    await messageActions.CompleteMessageAsync(message);
                }
                else
                {
                    _logger.LogError($"Failed to send email to {emailRequest.To}");
                    await messageActions.AbandonMessageAsync(message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : EmailSender.Run() :: {ex.Message}");
        }
    }
}
