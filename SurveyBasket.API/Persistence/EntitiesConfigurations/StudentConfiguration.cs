
namespace SurveyBasket.API.Persistence.EntitiesConfigurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(x => x.FirstName).IsRequired();

            builder.HasIndex(x=>x.LastName).IsUnique();
        }
    }
}
