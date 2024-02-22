namespace Vision.ViewModels
{
    public class WorkingHoursVM
    {
        public string? Day { get; set; }
        public string? StartTime1 { get; set; }
        public string? StartTime2 { get; set; }

        public string? EndTime1 { get; set; }
        public string? EndTime2 { get; set; }
        public bool Isclosed { get; set; }
        public long ClassifiedBusinessId { get; set; }
    }
}
