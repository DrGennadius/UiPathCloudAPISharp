namespace UiPathCloudAPISharp.Models
{
    public class StartJobsInfo
    {
        public StartJobsInfo(string releaseKey, string strategy, int[] robotIds, int noOfRobots)
        {
            ReleaseKey = releaseKey;
            Strategy = "All";
            RobotIds = robotIds;
            NoOfRobots = 0;
        }

        public StartJobsInfo()
        {
            Strategy = "All";
            NoOfRobots = 0;
        }

        public string ReleaseKey { get; set; }

        public string Strategy { get; set; }

        public int[] RobotIds { get; set; }

        public int NoOfRobots { get; set; }
    }
}
