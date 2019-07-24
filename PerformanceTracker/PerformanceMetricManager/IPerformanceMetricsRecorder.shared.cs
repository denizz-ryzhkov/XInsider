using System;
using System.Collections.Generic;

namespace PerformanceTracker
{
    public interface IRenderingMetricsRecorder
    {
        void Start();
        void Stop();
        IList<FrameMetricsData> GetFrames();
    }
}