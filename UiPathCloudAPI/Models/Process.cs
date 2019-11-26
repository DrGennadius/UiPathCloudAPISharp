using Newtonsoft.Json;

namespace UiPathCloudAPISharp.Models
{
    public class Process
    {
        public string Key { get; set; }
        
        public string ProcessKey { get; set; }
        
        public string ProcessVersion { get; set; }
        
        public bool IsLatestVersion { get; set; }
        
        public bool IsProcessDeleted { get; set; }
        
        public string Description { get; set; }
        
        public string Name { get; set; }
        
        public int EnvironmentId { get; set; }
        
        public string EnvironmentName { get; set; }

        public string InputArguments { get; set; }

        public int Id { get; set; }

        [JsonProperty(PropertyName = "Arguments")]
        public BasicArgumentsInfo BasicArguments { get; set; }

        [JsonIgnore]
        public ArgumentsInfo Arguments
        {
            get
            {
                return new ArgumentsInfo(BasicArguments);
            }
            set
            {
                BasicArguments = value.GetBaseArguments();
            }
        }
    }
}
