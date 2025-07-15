namespace SurveyBasket.API.Entities
{
    public class VoteAnswers
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;


        public int AnswerId { get; set; }
        public Answer Answer { get; set; } = default!;

        public int VoteId { get; set; }

        public Vote Vote { get; set; } = default!;
    }
}
