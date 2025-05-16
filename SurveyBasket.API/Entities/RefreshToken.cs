namespace SurveyBasket.API.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string token { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime ExpireOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpireOn;

        public bool IsActive => !IsExpired && RevokedOn is null;


    }
}
