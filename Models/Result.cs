namespace UFUKDER_BAGIS.Models
{
    public class Result<TEntity>
    {
        public bool IsSuccess { get; set; }
        public TEntity? Item { get; set; }
        public IEnumerable<TEntity>? List { get; set; }
        public string? Message { get; set; }

        public static Result<TEntity> Success(TEntity item)
        {
            return new Result<TEntity> { IsSuccess = true, Item = item, List = null, Message = "" };
        }
        public static Result<TEntity> Success(IEnumerable<TEntity> list)
        {
            return new Result<TEntity> { IsSuccess = true, List = list, Message = "" };
        }

        public static Result<TEntity> Success(TEntity item, IEnumerable<TEntity> list, string message)
        {
            return new Result<TEntity> { IsSuccess = true, Item = item, List = list, Message = message };
        }

        public static Result<TEntity> Success(string message)
        {
            return new Result<TEntity> { IsSuccess = true, List = null, Message = message };
        }
        public static Result<TEntity> Failure(string errorMessage)
        {
            return new Result<TEntity> { IsSuccess = false, Message = errorMessage };
        }
    }

    public class Result
    {
        public bool HATA { get; set; }
        public string HATAMESAJI { get; set; }


    }
}
