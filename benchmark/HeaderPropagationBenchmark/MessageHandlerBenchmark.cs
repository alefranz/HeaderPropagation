using BenchmarkDotNet.Attributes;
using HeaderPropagation;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HeaderPropagationBenchmark
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class MessageHandlerBenchmark
    {
        [GlobalSetup]
        public void GlobalSetup()
        {
            DelegatingHandler handler = new SimpleHandler();
            if (Data.UseHandler)
            {
                handler = new HeaderPropagationMessageHandler(Options.Create(Data.Configuration), Data.State)
                {
                    InnerHandler = handler
                };
            }

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://example.com")
            };
        }

        public HttpClient Client { get; set; }

        [ParamsSource(nameof(ValuesForData))]
        public RunData Data;

        public static IEnumerable<RunData> ValuesForData => new[]
        {
            new RunData
            {
                UseHandler = false,
            },
            BuildRunDataWithHandler(),
            BuildRunDataWithHandler("test"),
            BuildRunDataWithHandler(new[]{"one", "two" })
        };

        [Benchmark]
        public Task<HttpResponseMessage> GetAsync()
        {
            return Client.GetAsync("/");
        }

        private static RunData BuildRunDataWithHandler(StringValues? values = null)
        {
            var configuration = new HeaderPropagationOptions();

            configuration.Headers.Add("in", new HeaderPropagationEntry { OutboundHeaderName = "out" });

            var state = new HeaderPropagationValues();
            state.Headers.Clear();
            if (values.HasValue) state.Headers.Add("in", values.Value);

            return new RunData
            {
                UseHandler = true,
                Configuration = configuration,
                State = state
            };
        }

        private class SimpleHandler : DelegatingHandler
        {
            private readonly Task<HttpResponseMessage> _response = Task.FromResult(new HttpResponseMessage());

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return _response;
            }
        }

        public class RunData
        {
            public bool UseHandler { get; set; }
            public HeaderPropagationValues State { get; set; }
            public HeaderPropagationOptions Configuration { get; set; }
        }
    }
}
