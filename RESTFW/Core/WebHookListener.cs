using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Demo.RestServiceFramework
{
    public delegate byte[] ProcessDataDelegate(string data);

    public class WebHookListener
    {
        private const int HandlerThread = 2;
        private readonly ProcessDataDelegate handler;
        private readonly HttpListener listener;

        /// <summary>
        /// Gets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHookListener"/> class.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <param name="url">The URL.</param>
        /// <param name="handler">The handler.</param>
        public WebHookListener(HttpListener listener, string url, ProcessDataDelegate handler)
        {
            this.listener = listener;
            this.handler = handler;
            listener.Prefixes.Add(url);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (IsActive()) return;

            listener.Start();

            for (int i = 0; i < HandlerThread; i++)
            {
                listener.GetContextAsync().ContinueWith(ProcessRequestHandler);
            }
        }

        /// <summary>
        /// Determines whether this instance is active.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </returns>
        public bool IsActive()
        {
            return listener.IsListening;
        }

        /// <summary>
        /// Processes the request handler.
        /// </summary>
        /// <param name="result">The result.</param>
        private void ProcessRequestHandler(Task<HttpListenerContext> result)
        {
            var context = result.Result;

            if (!IsActive()) return;

            // Start new listener which replace this
            listener.GetContextAsync().ContinueWith(ProcessRequestHandler);

            // Read request
            string request = new StreamReader(context.Request.InputStream).ReadToEnd();

            // Prepare response
            var responseBytes = handler.Invoke(request);
            context.Response.ContentLength64 = responseBytes.Length;

            StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), context.Response.StatusCode.ToString());

            var output = context.Response.OutputStream;
            output.WriteAsync(responseBytes, 0, responseBytes.Length);
            output.Close();
        }
    }
}
