using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// A container for dictionaries of input and output arguments.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Create a container for dictionaries of input and output arguments using the created dictionaries.
        /// </summary>
        /// <param name="inputDictionary"></param>
        /// <param name="outputDictionary"></param>
        public Arguments(Dictionary<string, object> inputDictionary, Dictionary<string, object> outputDictionary)
        {
            Input = inputDictionary;
            Output = outputDictionary;
        }

        /// <summary>
        /// Create a container for dictionaries of input and output arguments.
        /// </summary>
        public Arguments()
        {

        }

        /// <summary>
        /// Input argument dictionary.
        /// </summary>
        public Dictionary<string, object> Input { get; set; }

        /// <summary>
        /// Output argument dictionary.
        /// </summary>
        public Dictionary<string, object> Output { get; set; }

        /// <summary>
        /// Get input and output arguments as strings.
        /// </summary>
        /// <returns></returns>
        public BasicArguments GetBasicArguments()
        {
            return new BasicArguments(
                JsonConvert.SerializeObject(Input),
                JsonConvert.SerializeObject(Output)
                );
        }
    }
}
