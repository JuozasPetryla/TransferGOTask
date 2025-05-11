namespace TransferGOTask.NotificationService.Infrastructure.Providers.Email;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Options;
using Domain.Entities;
using Configuration;
using Resilience;

public class SesEmailProvider : BaseEmailProvider
{
    private readonly IAmazonSimpleEmailService _client;
    private bool _identityVerified;
    public override string Key => "SES";

    public SesEmailProvider(
        IAmazonSimpleEmailService client,
        RetryPolicyFactory retryFactory,
        IOptions<NotificationSettings> settings)
        : base(retryFactory, settings)
    {
        _client = client;
        
    }

    protected override async Task SendEmailAsync(Notification notification, CancellationToken cancellationToken)
    {
        const string from = "no-reply@myapp.com";
        
        if (!_identityVerified)
        {
            await _client.VerifyEmailIdentityAsync(
                new VerifyEmailIdentityRequest { EmailAddress = from },
                cancellationToken
            );
            _identityVerified = true;
        }
        
        var sendReq = new SendEmailRequest
        {
            Source = from,
            Destination = new Destination
            {
                ToAddresses = new List<string> { notification.Recipient.Email }
            },
            Message = new Message
            {
                Subject = new Content(notification.Message.Subject),
                Body = new Body
                {
                    Text = new Content(notification.Message.Body)
                }
            }
        };

        await _client.SendEmailAsync(sendReq, cancellationToken);
    }
}