namespace EmployeeAPI.Models
{
    public enum ReportStatus
    {
        Processing,
        Completed
    }

    public class ReportJob
    {
        public required string Id { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public object? Result { get; set; }
    }
}
