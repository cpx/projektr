using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Projektr.WebApi;

namespace Projektr.Samples.WebApiSample
{
    internal class Startup
    {
        static void Main(string[] args)
        {
            const string baseUrl = "http://localhost:8989";
            using (WebApp.Start<Startup>(baseUrl))
            {
                Console.WriteLine($"Listening to '{baseUrl}'. Press ENTER to quit.");
                Console.ReadLine();
            }
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.UseProjektr();
            app.UseWebApi(config);
        }
    }
}