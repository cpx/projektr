using System.Web.Http;
using System.Web.Http.Controllers;

namespace Projektr.WebApi
{
    public static class HttpConfigurationExtensions
    {
        public static void UseProjektr(this HttpConfiguration configuration)
        {
            configuration.Services.Replace(typeof(IHttpActionInvoker), new ProjektrActionInvoker());
        }
    }
}
