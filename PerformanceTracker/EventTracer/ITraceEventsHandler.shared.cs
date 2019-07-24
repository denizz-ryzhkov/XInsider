using System;
using System.Collections.Generic;

namespace PerformanceTracker
{
    public interface ITraceEventsHandler
    {
        void Checkpoint(string name, string description, Dictionary<string, object> parameters);
        TraceEvent StartEvent(string name, string description, Dictionary<string, object> parameters);
        bool MarkFinished(Guid id);
        TraceEvent GetEvent(Guid id);
        IList<TraceEvent> GetEvents();
    }
}