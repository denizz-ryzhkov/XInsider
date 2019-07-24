using System;
using System.Collections.Generic;
using System.Linq;

namespace PerformanceTracker
{
    public partial class RenderingMetricsRecorder : IRenderingMetricsRecorder
    {
        public static RenderingMetricsRecorder Current { get; }

        static RenderingMetricsRecorder()
        {
            Current = new RenderingMetricsRecorder();
        }

        private List<FrameMetricsData> _storage;

        protected RenderingMetricsRecorder()
        {
            _storage = new List<FrameMetricsData>(500);
        }

        public void Start()
        {
            this.PStart();
        }

        public void Stop()
        {
            this.PStop();
        }

        public IList<FrameMetricsData> GetFrames()
        {
            return this.PGetFrames();
        }
    }
}