using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class QueueItemEvent
    {
        public int QueueItemId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Action { get; set; }

        public string Data { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Status { get; set; }

        public string ReviewStatus { get; set; }

        public string ReviewerUserId { get; set; }

        public string ReviewerUserName { get; set; }

        public int Id { get; set; }
    }
}
