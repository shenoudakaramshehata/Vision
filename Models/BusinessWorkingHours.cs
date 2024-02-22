namespace Vision.Models
{
    public class BusinessWorkingHours
    {
        public int BusinessWorkingHoursId { get; set; }
        public string? Day { get; set; }
        public string? StartTime1 { get; set; }
        public string? StartTime2 { get; set; }

        public string? EndTime1 { get; set; }
        public string? EndTime2 { get; set; }
        public bool Isclosed { get; set; }
        public long ClassifiedBusinessId { get; set; }
        public virtual ClassifiedBusiness ClassifiedBusiness { get; set; }
    }
}
