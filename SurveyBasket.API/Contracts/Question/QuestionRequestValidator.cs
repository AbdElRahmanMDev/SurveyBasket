namespace SurveyBasket.API.Contracts.Question
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content).NotEmpty().Length(3, 100);


            RuleFor(x => x.Answers)
           .NotNull();

            RuleFor(x => x.Answers)
                .Must(answers => answers.Count > 1)
                .WithMessage("Question should has at least 2 answers")
                .When(x=>x.Answers != null);


       

            RuleFor(x => x.Answers)
                .Must(answers => answers.Distinct().Count() == answers.Count)
                .WithMessage("You can not Add Duplicated Answers for the same question")
                                 .When(x => x.Answers != null);

        }
    }
}
