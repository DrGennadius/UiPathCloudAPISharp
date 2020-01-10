using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// Queue Item for Transactions.
    /// </summary>
    public class QueueItem
    {
        public int QueueDefinitionId { get; set; }

        public string OutputData { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public string ReviewerUserId { get; set; }

        public string Key { get; set; }

        public string Reference { get; set; }

        public string ProcessingExceptionType { get; set; }

        public string DueDate { get; set; }

        public string Priority { get; set; }

        public string DeferDate { get; set; }

        public string StartProcessing { get; set; }

        public string EndProcessing { get; set; }

        public int SecondsInPreviousAttempts { get; set; }

        public string AncestorId { get; set; }

        public int RetryNumber { get; set; }

        public string SpecificData { get; set; }

        public DateTime CreationTime { get; set; }

        public string Progress { get; set; }

        public string RowVersion { get; set; }

        public int Id { get; set; }

        public ProcessingException ProcessingException { get; set; }

        public object SpecificContent { get; set; }

        [JsonProperty("Output")]
        private object _output = null;

        [JsonIgnore]
        public string Output
        {
            get
            {
                return _output as string;
            }
        }

        public Robot Robot { get; set; }
    }
}
