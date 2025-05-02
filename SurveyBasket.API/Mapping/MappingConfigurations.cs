namespace SurveyBasket.API.Mapping
{
    public class MappingConfigurations : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Student, StudentResponse>().
                Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName} {src.MiddleName}").
                Map(dest=>dest.Age,src=>DateTime.Now.Year-src.DateOfBirth!.Value.Year,
                srcCond=>srcCond.DateOfBirth.HasValue).
                Ignore(dest=>dest.DepartmentName);
            
        }
    }
}
