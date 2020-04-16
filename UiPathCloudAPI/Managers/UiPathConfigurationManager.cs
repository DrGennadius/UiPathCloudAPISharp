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

        private RequestExecutor _requestExecutor;

        internal UiPathConfigurationManager(RequestExecutor requestExecutor)
        {
            _requestExecutor = requestExecutor;
        }

        public ConfigurationInfo GetConfigurationInfo(Folder folder)
        {
            return GetConfigurationInfo(1, folder);
        }

        public ConfigurationInfo GetConfigurationInfo(int scope = 1, Folder folder = null)
        {
            string response = _requestExecutor.SendRequestGetForOdata(string.Format("Settings/UiPath.Server.Configuration.OData.GetExecutionSettingsConfiguration(scope={0})", scope), folder);
            return JsonConvert.DeserializeObject<ConfigurationInfo>(response);
        }

        public Setting GetSetting(string key, Folder folder = null)
        {
            return GetConfigurationInfo(folder).Configuration.Where(s => s.Key == key).FirstOrDefault();
        }

        public Setting this[string key]
        {
            get
            {
                return GetSetting(key);
            }
        }

        public Setting this[string key, Folder folder]
        {
            get
            {
                return GetSetting(key, folder);
            }
        }
    }
}
