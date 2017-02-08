using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using Prometheus.Advanced;

namespace Prometheus
{
    public abstract class MetricHandler : IMetricServer
    {
        protected readonly ICollectorRegistry Registry;
        private IDisposable _schedulerDelegate;

        protected MetricHandler(IEnumerable<IOnDemandCollector> standardCollectors = null,
            ICollectorRegistry registry = null)
        {
            Registry = registry ?? DefaultCollectorRegistry.Instance;
            if (Registry == DefaultCollectorRegistry.Instance)
            {
                // Default to DotNetStatsCollector if none specified
                // For no collectors, pass an empty collection
                if (standardCollectors == null)
                    standardCollectors = new[] {new DotNetStatsCollector()};

                DefaultCollectorRegistry.Instance.RegisterOnDemandCollectors(standardCollectors);
            }
        }

        public void Start(IScheduler scheduler = null)
        {
            _schedulerDelegate = StartLoop(scheduler ?? Scheduler.Default);
        }

        public void Stop()
        {
            _schedulerDelegate?.Dispose();
            StopInner();
        }

        protected virtual void StopInner()
        {
        }

        protected abstract IDisposable StartLoop(IScheduler scheduler);
    }
}