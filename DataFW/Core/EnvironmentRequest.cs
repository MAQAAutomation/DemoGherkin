using System.Collections.Generic;

namespace Demo.DataUniverseFramework
{
    public class EnvironmentRequest
    {
        /// <summary>
        /// Gets or sets the XML request.
        /// </summary>
        /// <value>
        /// The XML request.
        /// </value>
        public string XmlRequest { get; set; }
        /// <summary>
        /// Gets or sets the json request.
        /// </summary>
        /// <value>
        /// The json request.
        /// </value>
        public string JsonRequest { get; set; }
        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public string Response { get; set; }
        /// <summary>
        /// Gets or sets the query parameter.
        /// </summary>
        /// <value>
        /// The query parameter.
        /// </value>
        public string QueryParam { get; set; }
        /// <summary>
        /// Gets or sets the path parameter.
        /// </summary>
        /// <value>
        /// The path parameter.
        /// </value>
        public string PathParam { get; set; }
        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public List<string> Param { get; set; }
    }
}
