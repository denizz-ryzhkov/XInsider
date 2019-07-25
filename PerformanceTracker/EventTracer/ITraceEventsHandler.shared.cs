﻿using System;
using System.Collections.Generic;

namespace PerformanceTracker
{
    public interface ITraceEventsHandler
    {
        void Checkpoint(string name, string description = null, Dictionary<string, object> parameters = null);
        TraceEvent StartEvent(string name, string description = null, Dictionary<string, object> parameters = null);
        void MakeEvent(TimeSpan period, string name, string description = null, Dictionary<string, object> parameters = null);
        bool FinishEvent(Guid id);
        TraceEvent GetEvent(Guid id);
        IList<TraceEvent> GetEvents();
    }
}