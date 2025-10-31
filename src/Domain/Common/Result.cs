namespace Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }

        public static Result Ok()
        {
            return new Result { IsSuccess = true };
        }

        public static Result Fail(string message)
        {
            return new Result { IsSuccess = false, Message = message };
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Ok(T value)
        {
            return new Result<T> { IsSuccess = true, Value = value };
        }

        public static new Result<T> Fail(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }
    }

}