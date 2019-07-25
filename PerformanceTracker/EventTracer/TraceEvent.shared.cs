using System;
using System.Collections.Generic;

namespace PerformanceTracker
{
    public class TraceEvent
    {
        public TraceEvent(string name, string description, Dictionary<string, object> parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters ?? new Dictionary<string, object>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }
        public TraceEventPeriod EventPeriod { get; internal set; }
        public TimeSpan StartedAt { get; protected set; }
        public TimeSpan FinishedAt { get; protected set; }
        public TimeSpan Delta { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public Dictionary<string, object> Parameters { get; protected set; }

        internal TraceEvent SetStartedAt(TimeSpan p)
        {
            StartedAt = p;
            return this;
        }

        internal TraceEvent SetOccuredAt(TimeSpan p)
        {
            StartedAt = p;
            FinishedAt = p;
            return this;
        }

        internal TraceEvent SetFinishedAt(TimeSpan p)
        {
            FinishedAt = p;
            this.Delta = FinishedAt - StartedAt;
            return this;
        }

        public TraceEvent AddParameter(string name, object value)
        {
            Parameters[name] = value;
            return this;
        }
    }
}