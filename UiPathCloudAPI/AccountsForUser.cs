using Newtonsoft.Json;
using System.Collections.Generic;

namespace UiPathCloudAPISharp
{
    public class AccountsForUser
    {
        [JsonProperty(PropertyName = "userEmail")]
        public string UserEmail { get; set; }

        [JsonProperty(PropertyName = "accounts")]
        public List<Account> Accounts { get; set; }
    }
}
