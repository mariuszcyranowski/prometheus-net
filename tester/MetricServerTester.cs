using System;
using System.IO;
using System.Net;
using Prometheus;

namespace tester
{
    internal class MetricServerTester : Tester
    {
        public override IMetricServer InitializeMetricHandler()
        {
            return new MetricServer("localhost", 1234);
        }

        public override void OnObservation()
        {
            var httpRequest = (HttpWebRequest) WebRequest.Create("http://localhost:1234/metrics");
            httpRequest.Method = "GET";

            using (var httpResponse = (HttpWebResponse) httpRequest.GetResponse())
            {
                var text = new StreamReader(httpResponse.GetResponseStream()).ReadToEnd();
                Console.WriteLine(text);
            }
        }
    }
}