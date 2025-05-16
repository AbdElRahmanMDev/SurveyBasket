using SurveyBasket.API.Abstraction;

namespace SurveyBasket.API.Abstraction
{
    public class Result
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; } = default!;

        public Result(bool isSuccess, Error error)
        {
            if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
                throw new InvalidOperationException();
            IsSuccess= isSuccess;
            Error = error;
        }


        public static Result Succes() => new(true,Error.None);
        public static Result Failure(Error error) => new(false, error);


        public static TResult<T> Succes<T>(T value) => new(value, true, Error.None);
        public static TResult<T> Failure<T>(Error error) => new(default, false, error);

    }
        


}

public class TResult<T> : Result
{
    private readonly T? _value;

    public TResult(T? value, bool success,Error error) : base(success, error)
    {
        _value = value;
        
    }

    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Failure value can not have value");


}
