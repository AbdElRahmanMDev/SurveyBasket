using SurveyBasket.API.Contracts.Answers;
using SurveyBasket.API.Contracts.Question;
using SurveyBasket.API.Contracts.Students;

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

            //Mapping from Answer to AnswerResponse
            config.NewConfig<Answer, AnswerResponse>()
                  ;

            // Mapping from QuestionRequest to Question (already correct)
            config.NewConfig<QuestionRequest, Question>()
                  .Map(dest => dest.answers, src => src.Answers.Select(answer => new Answer { Content = answer }))
                 ;

            // ✅ Add this mapping from Question to QuestionResponse
            config.NewConfig<Question, QuestionResponse>()
                  .Map(dest => dest.answers, src => src.answers.Adapt<IEnumerable<AnswerResponse>>())
                 ;
        }
    }
}
