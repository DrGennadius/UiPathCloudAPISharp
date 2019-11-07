namespace UiPathOrchestrator
{
    public class Process
    {
        public string Key { get; set; }
        
        public string ProcessKey { get; set; }
        
        public string ProcessVersion { get; set; }
        
        public string IsLatestVersion { get; set; }
        
        public string IsProcessDeleted { get; set; }
        
        public string Description { get; set; }
        
        public string Name { get; set; }
        
        public string EnvironmentId { get; set; }
        
        public string EnvironmentName { get; set; }

        public string InputArguments { get; set; }

        public string Id { get; set; }

        //public Arguments Arguments { get; set; }
    }
}
