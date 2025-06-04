namespace PhoneBook.Repository
{
    public class RepositoryResult<T>
    {
        public T? Data { get; set; }
        public bool Success => Error == null;
        public string? Error { get; set; }

        public static RepositoryResult<T> Ok(T data) => new RepositoryResult<T> { Data = data };
        public static RepositoryResult<T> Fail(string error) => new RepositoryResult<T> { Error = error };
    }
}
