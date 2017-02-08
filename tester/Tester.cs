using Prometheus;

namespace tester
{
    internal abstract class Tester
    {
        public virtual void OnStart()
        {
        }

        public virtual void OnObservation()
        {
        }

        public virtual void OnEnd()
        {
        }

        public abstract IMetricServer InitializeMetricHandler();
    }
}