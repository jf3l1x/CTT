using System;

namespace CTT.Models
{
    public class Activity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProjectId { get; set; }
        public string ServiceId { get; set; }
        public string Solicitant { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public TimeSpan TotalTime
        {
            get
            {
                if (End.HasValue && Start.HasValue)
                {
                    return End.Value.Subtract(Start.Value);
                }
                return TimeSpan.Zero;
            }
        }

        public string Description { get; set; }
        public string OrdemServicoId { get; set; }

    }
}