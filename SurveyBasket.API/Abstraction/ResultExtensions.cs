namespace SurveyBasket.API.Abstraction
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result,int statusCode) {

            if (result.IsSuccess)
                throw new InvalidOperationException("can not conver succcess result to problem");

            var problem = Results.Problem(statusCode: statusCode);
            var porblemDetails=problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(problem) as ProblemDetails;
            porblemDetails!.Extensions = new Dictionary<string, object?>
                {
                    {
                        "Error",new[]{
                            new
                            {
                                result.Error.Code,
                                result.Error.Description
                            }
                        }
                    }
                };
        
        
            return new ObjectResult(porblemDetails);
        }
    }
}
