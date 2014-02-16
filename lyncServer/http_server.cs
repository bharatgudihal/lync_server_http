using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    class http_server
    {

        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        public Person pstack;

        public void setPersonStack(Person p)
        {
            pstack = p;
        }
        public http_server(string[] prefixes, Func<HttpListenerRequest, string> method)
        {

            

            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");
 
            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
 
            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");
 
            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);
 
            _responderMethod = method;
            _listener.Start();
        }

        public http_server(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }
 
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);

                                HttpListenerRequest request = ctx.Request;
                                 ShowRequestData(request);
                                 ShowRequestProperties1(request);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }
        

        public  void ShowRequestProperties1(HttpListenerRequest request)
        {
            // Display the MIME types that can be used in the response. 
            string[] types = request.AcceptTypes;
            if (types != null)
            {
                Console.WriteLine("Acceptable MIME types:");
                foreach (string s in types)
                {
                    Console.WriteLine(s);
                }
            }
            // Display the language preferences for the response.
            types = request.UserLanguages;
            if (types != null)
            {
                Console.WriteLine("Acceptable natural languages:");
                foreach (string l in types)
                {
                    Console.WriteLine(l);
                }
            }
            String status, empid;
            // Display the URL used by the client.
            Console.WriteLine("URL: {0}", request.Url.OriginalString);
            Console.WriteLine("Raw URL: {0}", request.RawUrl);
            Console.WriteLine("emp id" + request.QueryString["id"]);
           Console.WriteLine("status"+ request.QueryString["status"]);
          //  Console.WriteLine("status" + request.QueryString["id"]);

           empid = request.QueryString["id"];
           status = request.QueryString["status"];
           Person emp = Person.get(empid);
           Program prg = new Program();
            prg.connect(emp ,status); 


           // var queryString = HttpUtility.ParseQueryString(request.Url.Query);
            // Display the referring URI.
            Console.WriteLine("Referred by: {0}", request.UrlReferrer);

            //Display the HTTP method.
            Console.WriteLine("HTTP Method: {0}", request.HttpMethod);
            //Display the host information specified by the client;
            Console.WriteLine("Host name: {0}", request.UserHostName);
            Console.WriteLine("Host address: {0}", request.UserHostAddress);
            Console.WriteLine("User agent: {0}", request.UserAgent);
        }
        public  void ShowRequestData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
               // return;
            }
            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Console.WriteLine("Client data content type {0}", request.ContentType);
            }
            Console.WriteLine("Client data content length {0}", request.ContentLength64);

            Console.WriteLine("Start of client data:");
            // Convert the data to a string and display it on the console. 
            string s = reader.ReadToEnd();
            Console.WriteLine(s);
            Console.WriteLine("End of client data:");
            body.Close();
            reader.Close();
            // If you are finished with the request, it should be closed also.
        }
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }



 
    

    }


    
}
