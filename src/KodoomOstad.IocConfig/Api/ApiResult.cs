namespace KodoomOstad.IocConfig.Api
{
    public class ApiResult
    {
        public ApiResult(object data, params string[] errors)
        {
            Data = data;
            Errors = errors;
        }

        public ApiResult(params string[] errors) : this(null, errors)
        {
        }

        public ApiResult(object data) : this(data, null)
        {
        }

        public object Data { get; set; }

        public string[] Errors { get; set; }
    }
}
