using System.Collections.Concurrent;
using EmployeeAPI.Models;

namespace EmployeeAPI.Services
{
    // Simula un proceso asíncrono de generación de reportes (job en memoria, sin cola/worker real).
    public class ReportService
    {
        private static readonly TimeSpan ProcessingDuration = TimeSpan.FromSeconds(8);
        private readonly ConcurrentDictionary<string, ReportJob> _jobs = new();
        private readonly MongoDBService _mongoDBService;

        public ReportService(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        public string StartReport()
        {
            var job = new ReportJob
            {
                Id = Guid.NewGuid().ToString(),
                Status = ReportStatus.Processing,
                CreatedAt = DateTime.UtcNow
            };

            _jobs[job.Id] = job;

            return job.Id;
        }

        public async Task<ReportJob?> GetStatusAsync(string id)
        {
            if (!_jobs.TryGetValue(id, out var job))
            {
                return null;
            }

            if (job.Status == ReportStatus.Processing && DateTime.UtcNow - job.CreatedAt >= ProcessingDuration)
            {
                var employees = await _mongoDBService.GetAsync();
                job.Result = new
                {
                    totalEmployees = employees.Count,
                    departments = employees.Select(e => e.Department).Distinct().Count()
                };
                job.CompletedAt = DateTime.UtcNow;
                job.Status = ReportStatus.Completed;
            }

            return job;
        }
    }
}
