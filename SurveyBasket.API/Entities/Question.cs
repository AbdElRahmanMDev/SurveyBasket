namespace SurveyBasket.API.Entities;

public sealed class Question : AuditableEntity
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public int PollId { get; set; }

    public Poll poll { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public ICollection<Answer> answers { get; set; } = [];

    
}

