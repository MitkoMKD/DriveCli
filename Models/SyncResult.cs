namespace DriveCli.Models
{
    public class SyncResult
    {
        public int Total { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public double ElapsedSeconds { get; set; }
    }
}
