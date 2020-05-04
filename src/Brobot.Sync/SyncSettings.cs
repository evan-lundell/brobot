namespace Brobot.Sync
{
    public class SyncSettings
    {
        public string BaseUrl { get; set; }
        public string BrobotToken { get; set; }
        public string ApiKey { get; set; }
        public int SyncIntervalInMinutes { get; set; }
    }
}