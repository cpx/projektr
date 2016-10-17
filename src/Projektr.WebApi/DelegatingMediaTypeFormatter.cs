using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Projektr.WebApi
{
    internal abstract class DelegatingMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly MediaTypeFormatter _inner;

        protected DelegatingMediaTypeFormatter(MediaTypeFormatter inner)
        {
            _inner = inner;
        }

        public override bool CanReadType(Type type)
        {
            return _inner.CanReadType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return _inner.CanWriteType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return WriteToStreamAsync(type, value, writeStream, content, transportContext, CancellationToken.None);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext, CancellationToken cancellationToken)
        {
            return _inner.WriteToStreamAsync(type, value, writeStream, content, transportContext, cancellationToken);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return ReadFromStreamAsync(type, readStream, content, formatterLogger, CancellationToken.None);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger,
            CancellationToken cancellationToken)
        {
            return _inner.ReadFromStreamAsync(type, readStream, content, formatterLogger, cancellationToken);
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return _inner.GetPerRequestFormatterInstance(type, request, mediaType);
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            _inner.SetDefaultContentHeaders(type, headers, mediaType);
        }

        public override IRequiredMemberSelector RequiredMemberSelector
        {
            get { return _inner.RequiredMemberSelector; }
            set { _inner.RequiredMemberSelector = value; }
        }
    }
}