namespace Nagger.Data.Meazure.DTO
{
    using System.Collections.Generic;

    public class TimeEntryModel
    {
        public string Date { get; set; }
        public string Notes { get; set; }
        public string TimeString { get; set; }
        public string ProjectId { get; set; }
        public string TaskId { get; set; }

        public int DurationSeconds { get; set; }

        public List<string> WorkItems { get; set; }
    }
}