namespace Domain.Common
{
    public class Result
    {
        public bool Success { get; init; }
        public string? Message { get; init; }

        public static Result Ok()
        {
            return new Result { Success = true };
        }

        public static Result Fail(string message)
        {
            return new Result { Success = false, Message = message };
        }
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Ok(T value)
        {
            return new Result<T> { Success = true, Value = value };
        }

        public static new Result<T> Fail(string message)
        {
            return new Result<T> { Success = false, Message = message };
        }
    }

}