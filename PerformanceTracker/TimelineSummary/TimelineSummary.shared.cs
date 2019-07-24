using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTracker
{
    public class TimelineSummary
    {
        public Task<string> GetTextSummary()
        {
            var t = new TaskCompletionSource<string>();

            Task.Run(() =>
            {
                try
                {
                    var events = TraceEventsHandler.Current.GetEvents().ToArray();
                    var fpsTimeline = RenderingMetricsRecorder.Current.GetFrames().ToArray();


                    var sb = new StringBuilder();

                    sb.AppendLine("======= EVENTS =======");
                    foreach (var traceEvent in events)
                    {
                        sb.AppendLine($"{traceEvent.Name} at +{traceEvent.StartedAt:g} till +{traceEvent.FinishedAt:g}");
                    }

                    sb.AppendLine("======= FPS =======");
                    foreach (var fps in fpsTimeline)
                    {
                        sb.AppendLine($"Junky frame at +{fps.Offset:g} {fps.InformationAboutFrame()}");
                    }

                    t.SetResult(sb.ToString());
                }
                catch (Exception e)
                {
                    throw;
                }
            });

            return t.Task;
        }
    }
}
