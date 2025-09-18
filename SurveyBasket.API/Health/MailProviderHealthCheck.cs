using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SurveyBasket.API.Authentication;

namespace SurveyBasket.API.Health
{
    public class MailProviderHealthCheck : IHealthCheck
    {
        private readonly MailOptions _mailSettings;

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var smtp = new SmtpClient();

                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                return await Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return await Task.FromResult(HealthCheckResult.Unhealthy(exception: exception));
            }


        }
    }
}
