using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Projektr.WebApi
{
    internal class ProjektrFormatter : DelegatingMediaTypeFormatter
    {
        private readonly string _filter;

        public ProjektrFormatter(MediaTypeFormatter inner, string filter) : base(inner)
        {
            _filter = filter;
        }

        public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            var projektr = ProjektrCompiler.Compile(type, _filter);
            value = projektr(value);
            await base.WriteToStreamAsync(type, value, writeStream, content, transportContext, cancellationToken);
        }
    }
}