
namespace SurveyBasket.API.Persistence.EntitiesConfigurations
{
    public class VoteAnswersConfigurations : IEntityTypeConfiguration<VoteAnswers>
    {
        public void Configure(EntityTypeBuilder<VoteAnswers> builder)
        {
            builder.HasIndex(x => new { x.VoteId, x.QuestionId }).IsUnique();
        }
    }
}
