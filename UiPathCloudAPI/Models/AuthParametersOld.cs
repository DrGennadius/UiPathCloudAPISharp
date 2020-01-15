using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiPathCloudAPISharp.Models
{
    public class AuthParametersOld
    {
        public string grant_type { get { return "authorization_code"; } }

        public string code { get; set; }

        public string redirect_uri { get { return "https://account.uipath.com/mobile"; } }

        public string code_verifier { get; set; }

        public string client_id { get; set; }
    }
}
