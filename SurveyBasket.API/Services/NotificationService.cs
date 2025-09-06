
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.API.Helpers;
using SurveyBasket.API.Persistence;

namespace SurveyBasket.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _EmailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationService(ApplicationDbContext context,UserManager<ApplicationUser> userManager, IEmailSender EmailSender, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _EmailSender = EmailSender;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task SendNewPollNotification(int? pollId=null)
        {
            IEnumerable<Poll> polls = [];
            if(pollId.HasValue)
            {
                var poll =await _context.Polls.Where(x => x.Id == pollId && x.IsPublished)
                    .SingleOrDefaultAsync();
                polls = [poll!];
            }
            else
            {
                polls =await _context.Polls.Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                    .AsNoTracking()
                    .ToListAsync();

            }


            var users = await _userManager.Users.AsNoTracking().ToListAsync();

            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;


            foreach (var poll in polls)
            {
                foreach(var user in users )
                {
                    var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{pollTill}}", poll.Title },
                    { "{{endDate}}", poll.EndsAt.ToString() },
                    { "{{url}}", $"{origin}/polls/start/{poll.Id}" }
                };

                    var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholders);

                    await _EmailSender.SendEmailAsync(user.Email!, $"📣 Survey Basket: New Poll - {poll.Title}", body);
                }
            }
        }
    }
}
