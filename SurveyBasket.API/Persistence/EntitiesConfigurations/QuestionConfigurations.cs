
namespace SurveyBasket.API.Persistence.EntitiesConfigurations;

public class QuestionConfigurations : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.HasIndex(x => new { x.PollId,x.Content}).IsUnique();
    }
}
