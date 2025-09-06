namespace SurveyBasket.API.Services
{
    public interface INotificationService
    {
        Task SendNewPollNotification(int? pollId = null); 
    }
}
