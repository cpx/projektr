using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Projektr.WebApi
{
    internal class ProjektrActionInvoker : ApiControllerActionInvoker
    {
        public override async Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var response = await base.InvokeActionAsync(actionContext, cancellationToken);
            var objectContent = response.Content as ObjectContent;
            var parameters = actionContext.ActionDescriptor.GetParameters();
            var filterParameter = parameters.FirstOrDefault(p => p.GetCustomAttributes<ProjektrFilterAttribute>().Any());
            var filterParameterKey = filterParameter?.ParameterName;
            if (objectContent != null && filterParameterKey != null)
            {
                var filter = (string)actionContext.ActionArguments[filterParameterKey];
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    var newContent = new ObjectContent(objectContent.ObjectType, objectContent.Value,
                        new ProjektrFormatter(objectContent.Formatter, filter));
                    response.Content = newContent;
                }
            }
            return response;
        }
    }
}