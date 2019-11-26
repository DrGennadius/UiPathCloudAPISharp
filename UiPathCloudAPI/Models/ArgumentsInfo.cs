using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    public class ArgumentsInfo
    {
        public ArgumentsInfo(BasicArgumentsInfo basicArgumentsInfo)
        {
            Set(basicArgumentsInfo);
        }

        public ArgumentsInfo()
        {
        }

        public InputArgumentInfo[] Input { get; set; }

        public ArgumentInfo[] Output { get; set; }

        public void Set(BasicArgumentsInfo basicArgumentsInfo)
        {
            SetInputArguments(basicArgumentsInfo.Input);
            SetOutputArguments(basicArgumentsInfo.Output);
        }

        public void SetInputArguments(string inputBasicArgumentsInfo)
        {
            if (!string.IsNullOrEmpty(inputBasicArgumentsInfo))
            {
                Input = JsonConvert.DeserializeObject<InputArgumentInfo[]>(inputBasicArgumentsInfo);
            }
        }

        public void SetOutputArguments(string outputBasicArgumentsInfo)
        {
            if (!string.IsNullOrEmpty(outputBasicArgumentsInfo))
            {
                Output = JsonConvert.DeserializeObject<ArgumentInfo[]>(outputBasicArgumentsInfo);
            }
        }

        public string GetInputBasicArgumentsInfo()
        {
            return JsonConvert.SerializeObject(Input);
        }

        public string GetOutputBasicArgumentsInfo()
        {
            return JsonConvert.SerializeObject(Output);
        }

        internal BasicArgumentsInfo GetBaseArguments()
        {
            return new BasicArgumentsInfo(GetInputBasicArgumentsInfo(), GetOutputBasicArgumentsInfo());
        }
    }
}
