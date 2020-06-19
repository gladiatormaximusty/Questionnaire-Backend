namespace EIRA.ResultDto
{
    public class ResultsDto<T>
    {
        public ResultsDto() => Status = new StatusDev();

        public T Data { get; set; }

        public StatusDev Status { get; set; }
    }

    public class ResultBool
    {
        public bool Success { get; set; }
    }

    public class StatusDev
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }
}
