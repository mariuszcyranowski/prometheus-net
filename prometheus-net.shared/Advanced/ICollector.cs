using Prometheus.Advanced.DataContracts;

namespace Prometheus.Advanced
{
    public interface ICollector
    {
        string Name { get; }

        string[] LabelNames { get; }
        MetricFamily Collect();
    }
}