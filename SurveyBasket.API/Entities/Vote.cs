namespace SurveyBasket.API.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int PollId { get; set; }

        public Poll Poll { get; set; } = default!;

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = default!;

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;

        public ICollection<VoteAnswers> voteAnswers { get; set; } = [];
    }
}
