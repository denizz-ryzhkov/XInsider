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
            Debug.WriteLine($"TraceEventsHandler.Checkpoint {name}");
#endif

            var ts = PTrackerTimeProvider.Source.Elapsed;
            var evt = new TraceEvent(name, description, parameters)
            {
                EventMode = TraceEventMode.CheckPoint
            };
            evt.SetOccuredAt(ts);
            this._events[evt.Id] = evt;
        }
        public TraceEvent StartEvent(string name, string description, Dictionary<string, object> parameters)
        {
            var ts = PTrackerTimeProvider.Source.Elapsed;
            var evt = new TraceEvent(name, description, parameters)
            {
                EventMode = TraceEventMode.Continuous
            };

            evt.SetStartedAt(ts);
            this._events[evt.Id] = evt;
            return evt;
        }

        public bool MarkFinished(Guid id)
        {
            if (this._events.TryGetValue(id, out var evt))
            {
                if (evt.EventMode == TraceEventMode.Continuous)
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