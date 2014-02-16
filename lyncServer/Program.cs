using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Lync.Model;
using System.Net;
using System.Runtime.InteropServices;
using System.Collections;
namespace ConsoleApplication1
{


    class Person
    {
        public  string empid;
        public  string SipiD;
        public  string email;
        public  string password;

        public static Hashtable table;
        public static void  setHashMap()
        {
            table = new Hashtable();

        }
        public static void add(string empid, Person p)
        {
            table.Add(empid, p);
        }
        public static Person get(String empid)
        {
            return (Person)table[empid];
        }
    }
    class Program
    {
        private LyncClient lyncClient;
        static void Main(string[] args)
        {


            Person p = new Person();
            //set parameters here 
            p.empid = "";
            p.email = "";
            p.SipiD = "";
            p.password = "";


            Program.startWebserver(p);

            

            //Program p = new Program();

           

            //p.connect("hello india");

        }

        public static void startWebserver(Person p)
        {

            // hit url like  http://localhost:8089/test?id=6759&status=hello

            http_server ws = new http_server(SendResponse, "http://localhost:8089/test/");
            ws.setPersonStack(p);
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }
        public static string SendResponse(HttpListenerRequest request)
        {
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
        public  void connect(Person p,string status)
        {
            try
            {
                //"sip:abhijit.roy@comviva.com", "abhijit.roy@comviva.com", ""


                lyncClient = LyncClient.GetClient();
                //lyncClient.SignInConfiguration.InternalServerUrl = "comviva.com";
               
                if(lyncClient.State == ClientState.SignedOut)
                lyncClient.BeginSignIn(p.SipiD, p.email, p.password, SignInCallback, null);
                Console.WriteLine("please wait we are signing in");
                while (lyncClient.State == ClientState.SigningIn)
                {
                        
                }

                if (lyncClient.State == ClientState.SignedIn)
                {
                    Console.WriteLine("client has signed in");
                    setnote(status);
                    Console.WriteLine("status updated");


                    object[] asyncState = { lyncClient, "" };
                    lyncClient.BeginSignOut(SignOutCallback, asyncState);

                }

                


            }
            catch (ClientNotFoundException clientNotFoundException)
            {
                Console.WriteLine(clientNotFoundException);
                return;
            }
            catch (NotStartedByUserException notStartedByUserException)
            {
                Console.Out.WriteLine(notStartedByUserException);
                return;
            }
            catch (LyncClientException lyncClientException)
            {
                Console.Out.WriteLine(lyncClientException);
                return;
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                    return;
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

           

        }
        private void SignOutCallback(IAsyncResult result)
        {
            try
            {
                lyncClient.EndSignOut(result);
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }
        private void SignInCallback(IAsyncResult result)
        {
            try
            {
                lyncClient.EndSignIn(result);
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }
        private bool IsLyncException(SystemException ex)
        {
            return
                ex is NotImplementedException ||
                ex is ArgumentException ||
                ex is NullReferenceException ||
                ex is NotSupportedException ||
                ex is ArgumentOutOfRangeException ||
                ex is IndexOutOfRangeException ||
                ex is InvalidOperationException ||
                ex is TypeLoadException ||
                ex is TypeInitializationException ||
                ex is InvalidComObjectException ||
                ex is InvalidCastException;
        }
        private void setnote(string status)
        {
            //Add the personal note to the contact information items to be published
            Dictionary<PublishableContactInformationType, object> newInformation =
                new Dictionary<PublishableContactInformationType, object>();
            newInformation.Add(PublishableContactInformationType.PersonalNote, status);

            //Publish the new personal note value
            try
            {
                lyncClient.Self.BeginPublishContactInformation(newInformation, PublishContactInformationCallback, null);
            }
            catch (LyncClientException lyncClientException)
            {
                Console.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }
        private void PublishContactInformationCallback(IAsyncResult result)
        {
            lyncClient.Self.EndPublishContactInformation(result);
        }

    }

}
