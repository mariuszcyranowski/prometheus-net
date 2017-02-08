using System;
using Prometheus.Advanced;
using Prometheus.Advanced.DataContracts;

namespace Prometheus
{
    public interface ICounter
    {
        double Value { get; }
        void Inc(double increment = 1);
    }

    public class Counter : Collector<Counter.Child>, ICounter
    {
        internal Counter(string name, string help, string[] labelNames)
            : base(name, help, labelNames)
        {
        }

        protected override MetricType Type => MetricType.COUNTER;

        public void Inc(double increment = 1)
        {
            Unlabelled.Inc(increment);
        }

        public double Value => Unlabelled.Value;

        public class Child : Advanced.Child, ICounter
        {
            private ThreadSafeDouble _value;

            public void Inc(double increment = 1.0D)
            {
                //Note: Prometheus recommendations are that this assert > 0. However, there are times your measurement results in a zero and it's easier to have the counter handle this elegantly.
                if (increment < 0.0D)
                    throw new InvalidOperationException("Counter cannot go down");

                _value.Add(increment);
            }

            public double Value => _value.Value;

            protected override void Populate(Metric metric)
            {
                metric.counter = new Advanced.DataContracts.Counter {value = Value};
            }
        }
    }
}