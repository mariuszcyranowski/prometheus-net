using System;
using System.Diagnostics;
using Prometheus.Advanced;
using Prometheus.Advanced.DataContracts;

namespace Prometheus
{
    public interface IGauge
    {
        double Value { get; }
        void Inc(double increment = 1);
        void Set(double val);
        void Dec(double decrement = 1);
    }

    public class Gauge : Collector<Gauge.Child>, IGauge
    {
        internal Gauge(string name, string help, string[] labelNames)
            : base(name, help, labelNames)
        {
        }

        protected override MetricType Type => MetricType.GAUGE;

        public void Inc(double increment = 1)
        {
            Unlabelled.Inc(increment);
        }

        public void Set(double val)
        {
            Unlabelled.Set(val);
        }

        public void Dec(double decrement = 1)
        {
            Unlabelled.Dec(decrement);
        }

        public double Value => Unlabelled.Value;


        public class Timer
        {
            private readonly Child _child;
            private readonly Stopwatch _stopwatch;

            public Timer(Child child)
            {
                _child = child;
                _stopwatch = Stopwatch.StartNew();
            }

            public void ApplyDuration()
            {
                _child.Set(_stopwatch.Elapsed.Seconds);
            }
        }

        public class Child : Advanced.Child, IGauge
        {
            private ThreadSafeDouble _value;

            public void Inc(double increment = 1)
            {
                _value.Add(increment);
            }

            public void Set(double val)
            {
                _value.Value = val;
            }

            public void Dec(double decrement = 1)
            {
                Inc(-decrement);
            }

            public double Value => _value.Value;

            protected override void Populate(Metric metric)
            {
                metric.gauge = new Advanced.DataContracts.Gauge();
                metric.gauge.value = Value;
            }

            public void SetToCurrentTime()
            {
                var unixTicks = DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
                Set(unixTicks / TimeSpan.TicksPerSecond);
            }

            public Timer StartTimer()
            {
                return new Timer(this);
            }
        }
    }
}