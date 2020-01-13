using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UiPathCloudAPISharp.Common;
using UiPathCloudAPISharp.Models;
using UiPathCloudAPISharp.Query;

namespace UiPathCloudAPISharp.Managers
{
    public class UiPathConfigurationManager : IManager
    {
        public QueryStore QueryStore { get { throw new NotImplementedException(); } }

        private RequestManager _requestManager;

        internal UiPathConfigurationManager(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public ConfigurationInfo GetConfigurationInfo(int scope = 1)
        {
            string response = _requestManager.SendRequestGetForOdata(string.Format("Settings/UiPath.Server.Configuration.OData.GetExecutionSettingsConfiguration(scope={0})", scope));
            return JsonConvert.DeserializeObject<ConfigurationInfo>(response);
        }

        public Setting GetSetting(string key)
        {
            return GetConfigurationInfo().Configuration.Where(s => s.Key == key).FirstOrDefault();
        }

        public Setting this[string key]
        {
            get
            {
                return GetSetting(key);
            }
        }
    }
}
