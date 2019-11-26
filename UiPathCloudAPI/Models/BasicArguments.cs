using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// A container for input and output as a representation of strings.
    /// </summary>
    public class BasicArguments : IContainerWithArguments
    {
        /// <summary>
        /// Create a container for input and output as a representation of strings using the created strings.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public BasicArguments(string input, string output)
        {
            Input = input;
            Output = output;
        }

        /// <summary>
        /// Create a container for input and output as a representation of strings.
        /// </summary>
        public BasicArguments()
        {

        }

        /// <summary>
        /// Representation of input arguments as a single string.
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// Representation of output arguments as a single string.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Get input and output arguments as dictionaries.
        /// </summary>
        /// <returns></returns>
        public Arguments GetArguments()
        {
            return new Arguments(
                JsonConvert.DeserializeObject<Dictionary<string, object>>(Input),
                JsonConvert.DeserializeObject<Dictionary<string, object>>(Output)
                );
        }
    }
}
