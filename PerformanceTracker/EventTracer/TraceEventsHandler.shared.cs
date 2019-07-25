using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PerformanceTracker
{
    public class TraceEventsHandler : ITraceEventsHandler
    {
        private Dictionary<Guid, TraceEvent> _events;
        public static TraceEventsHandler Current { get; }

        static TraceEventsHandler()
        {
            Current = new TraceEventsHandler();
        }

        protected TraceEventsHandler()
        {
            _events = new Dictionary<Guid, TraceEvent>(10);
        }

        public void Checkpoint(string name, string description = null, Dictionary<string, object> parameters = null)
        {
#if DEBUG
            Debug.WriteLine($"TraceEventsHandler.Single {name}");
#endif

            var ts = PTrackerTimeProvider.Source.Elapsed;
            var evt = new TraceEvent(name, description, parameters)
            {
                EventPeriod = TraceEventPeriod.Single
            };
            evt.SetOccuredAt(ts);
            this._events[evt.Id] = evt;
        }

        public TraceEvent StartEvent(string name, string description = null, Dictionary<string, object> parameters = null)
        {
            var ts = PTrackerTimeProvider.Source.Elapsed;
            var evt = new TraceEvent(name, description, parameters)
            {
                EventPeriod = TraceEventPeriod.Period
            };

            evt.SetStartedAt(ts);
            this._events[evt.Id] = evt;
            return evt;
        }

        public void MakeEvent(TimeSpan period, string name, string description = null, Dictionary<string, object> parameters = null)
        {
            var ts = PTrackerTimeProvider.Source.Elapsed;
            var evt = new TraceEvent(name, description, parameters)
            {
                EventPeriod = TraceEventPeriod.PredefinedPeriod
            };

            evt.SetStartedAt(ts);
            evt.SetFinishedAt(ts.Add(period));
            this._events[evt.Id] = evt;
        }

        public bool FinishEvent(Guid id)
        {
            if (this._events.TryGetValue(id, out var evt))
            {
                if (evt.EventPeriod == TraceEventPeriod.Period)
                {
                    var ts = PTrackerTimeProvider.Source.Elapsed;
                    evt.SetFinishedAt(ts);
                    return true;
                }
            }

            return false;
        }

        public TraceEvent GetEvent(Guid id)
        {
            if (this._events.TryGetValue(id, out var evt))
            {
                return evt;
            }

            return null;
        }

        public IList<TraceEvent> GetEvents()
        {
            return this._events.Select(x => x.Value).OrderBy(x => x.StartedAt).ToArray();
        }
    }
}