namespace UiPathCloudAPISharp.Models
{
    public class StartJobsInfo
    {
        public string ReleaseKey { get; set; }

        public string Strategy { get; set; } = "All";

        public int[] RobotIds { get; set; }

        public int NoOfRobots { get; set; } = 0;
    }
}
