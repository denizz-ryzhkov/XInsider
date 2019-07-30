using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
                const int cw = -30;
                int linesw = Math.Abs(cw) * 4;

                try
                {
                    var events = TraceEventsHandler.Current.GetEvents().ToArray();
                    var fpsTimeline = RenderingMetricsRecorder.Current.GetFrames().ToArray();

                    var eSummary = TraceEventsHandler.Current.GetEvents().Select(x => new EventSummary(x)).ToArray();

                    foreach (var eventSummary in eSummary)
                    {
                        eventSummary.FillFramesFromSource(fpsTimeline).Calculate();
                    }

                    var sb = new StringBuilder();

                    sb.AppendLine("======= EVENTS =======");
                    foreach (var es in eSummary)
                    {

                        sb.AppendLine("");


                        sb.AppendLine($"".PadLeft(linesw, '-'));
                        sb.AppendLine($"".PadLeft(linesw, '-'));
                        sb.AppendLine($"EVENT: {es.Event.Name}");
                        sb.AppendLine($"+{es.Event.StartedAt:g}...+{es.Event.FinishedAt:g}, total {es.Event.Delta.TotalMilliseconds}ms");

                        if (!es.Frames.Any())
                        {
                            continue;
                        }

                        sb.AppendLine($"JFRAMES, {es.Report[EventSummary.JUNKY_FRAMES_TOTAL]} total, {es.Report[EventSummary.JUNKY_FRAMES_PERCENTAGE_AVG]}%");

                        sb.AppendLine($"".PadLeft(linesw, '-'));
                        sb.AppendLine($"{"JF RENDER TIME",cw} | {"LAYOUT FRAMES",cw} | {"DRAW FRAMES",cw} | {"GPU FRAMES",cw}");


                        var jf =
                            $"{es.Report[EventSummary.RENDER_JUNKY_FRAMES_AVG]} - [{es.Report[EventSummary.RENDER_JUNKY_FRAMES_MIN]} - {es.Report[EventSummary.RENDER_JUNKY_FRAMES_MAX]}]";

                        var lf =
                            $"{es.Report[EventSummary.LAYOUT_FRAMES_AVG]} - [{es.Report[EventSummary.LAYOUT_FRAMES_MIN]} - {es.Report[EventSummary.LAYOUT_FRAMES_MAX]}]";

                        var df =
                            $"{es.Report[EventSummary.DRAW_FRAMES_MIN]} - [{es.Report[EventSummary.DRAW_FRAMES_MIN]} - {es.Report[EventSummary.DRAW_FRAMES_MIN]}], ";

                        var gf =
                            $"{es.Report[EventSummary.GPU_FRAMES_AVG]} - [{es.Report[EventSummary.GPU_FRAMES_MIN]} - {es.Report[EventSummary.GPU_FRAMES_MAX]}]";

                        sb.AppendLine($"{jf,cw} | {lf,cw} | {df,cw} | {gf,cw}");

                        sb.AppendLine($"".PadLeft(linesw, '-'));
                        sb.AppendLine("");

                    }

                    //sb.AppendLine("======= FPS =======");
                    //foreach (var fps in fpsTimeline)
                    //{
                    //    sb.AppendLine($"Junky frame at +{fps.Offset:g} {fps.InformationAboutFrame()}");
                    //}

                    t.SetResult(sb.ToString());
                }
                catch (Exception e)
                {
                    throw;
                }
            });

            return t.Task;
        }

        private class EventSummary
        {
            public EventSummary(TraceEvent @event)
            {
                Event = @event;

                _report = new Dictionary<string, string>();
            }

            public EventSummary FillFramesFromSource(IList<FrameMetricsData> frameSource)
            {
                var start = Event.StartedAt;
                var end = Event.FinishedAt;

                if (Event.EventPeriod == TraceEventPeriod.Single)
                {
                    end = end.Add(TimeSpan.FromMilliseconds(60));
                }

                Frames = frameSource.Where(x => x.Offset >= start && x.Offset <= end).ToArray();

                return this;
            }

            public const string JUNKY_FRAMES_TOTAL = "JUNKY_FRAMES_TOTAL";
            public const string JUNKY_FRAMES_PERCENTAGE_AVG = "JUNKY_FRAMES_PERCENTAGE_AVG";
            public const string RENDER_JUNKY_FRAMES_MIN = "RENDER_JUNKY_FRAMES_MIN";
            public const string RENDER_JUNKY_FRAMES_MAX = "RENDER_JUNKY_FRAMES_MAX";
            public const string RENDER_JUNKY_FRAMES_AVG = "RENDER_JUNKY_FRAMES_AVG";


            public const string LAYOUT_FRAMES_MIN = "LAYOUT-MEASURE.FRAME.MIN";
            public const string LAYOUT_FRAMES_MAX = "LAYOUT-MEASURE.FRAME.MAX";
            public const string LAYOUT_FRAMES_AVG = "LAYOUT-MEASURE.FRAME.AVG";


            public const string DRAW_FRAMES_MIN = "DRAW-MEASURE.FRAME.MIN";
            public const string DRAW_FRAMES_MAX = "DRAW-MEASURE.FRAME.MAX";
            public const string DRAW_FRAMES_AVG = "DRAW-MEASURE.FRAME.AVG";

            public const string GPU_FRAMES_MIN = "GPU-MEASURE.FRAME.MIN";
            public const string GPU_FRAMES_MAX = "GPU-MEASURE.FRAME.MAX";
            public const string GPU_FRAMES_AVG = "GPU-MEASURE.FRAME.AVG";



            //public string InformationAboutFrame()
            //{
            //    float layoutMeasureDurationMs = ToMs(LayoutMeasureDuration);
            //    float drawDurationMs = ToMs(DrawDuration);
            //    float gpuCommandMs = ToMs(CommandIssueDuration);
            //    var totalDurationMs = ToMs(TotalDuration);
            //    var allFrames = FrameNumber;
            //    var jankyFrames = JunkyFrameNumber;

            //    float othersMs = totalDurationMs - layoutMeasureDurationMs - drawDurationMs - gpuCommandMs;
            //    float jankyPercent = (float)jankyFrames / allFrames * 100;

            //    var msg = $"Janky frame detected on Activity with total duration: {totalDurationMs}\n";
            //    msg += $"Layout/measure: {layoutMeasureDurationMs}ms, draw:{drawDurationMs}ms, gpuCommand:{gpuCommandMs}ms others:{othersMs}ms\n";
            //    msg += "Janky frames: " + jankyFrames + "/" + allFrames + "(" + jankyPercent + "%)";
            //    return msg;

            //    //Log.Warn("FrameMetricsDataData", msg);
            //}

            public void Calculate()
            {
                if (!this.Frames.Any())
                    return;

                _report.Add(JUNKY_FRAMES_TOTAL, this.Frames.Length.ToString());


                var jfp = this.Frames.Select(x => ((float)x.JunkyFrameNumber / x.FrameNumber * 100)).Sum() / this.Frames.Length;

                _report.Add(JUNKY_FRAMES_PERCENTAGE_AVG, jfp.ToString("F2"));


                _report.Add(RENDER_JUNKY_FRAMES_MIN, FrameMetricsData.ToMs(this.Frames.Min(x => x.TotalDuration)).ToString("F2"));
                _report.Add(RENDER_JUNKY_FRAMES_MAX, FrameMetricsData.ToMs(this.Frames.Max(x => x.TotalDuration)).ToString("F2"));
                _report.Add(RENDER_JUNKY_FRAMES_AVG, FrameMetricsData.ToMs(this.Frames.Sum(x => x.TotalDuration) / this.Frames.Length).ToString("F2"));


                _report.Add(LAYOUT_FRAMES_MIN, FrameMetricsData.ToMs(this.Frames.Min(x => x.LayoutMeasureDuration)).ToString("F2"));
                _report.Add(LAYOUT_FRAMES_MAX, FrameMetricsData.ToMs(this.Frames.Max(x => x.LayoutMeasureDuration)).ToString("F2"));
                _report.Add(LAYOUT_FRAMES_AVG, FrameMetricsData.ToMs(this.Frames.Sum(x => x.LayoutMeasureDuration) / this.Frames.Length).ToString("F2"));

                _report.Add(DRAW_FRAMES_MIN, FrameMetricsData.ToMs(this.Frames.Min(x => x.DrawDuration)).ToString("F2"));
                _report.Add(DRAW_FRAMES_MAX, FrameMetricsData.ToMs(this.Frames.Max(x => x.DrawDuration)).ToString("F2"));
                _report.Add(DRAW_FRAMES_AVG, FrameMetricsData.ToMs(this.Frames.Sum(x => x.DrawDuration) / this.Frames.Length).ToString("F2"));

                _report.Add(GPU_FRAMES_MIN, FrameMetricsData.ToMs(this.Frames.Min(x => x.CommandIssueDuration)).ToString("F2"));
                _report.Add(GPU_FRAMES_MAX, FrameMetricsData.ToMs(this.Frames.Max(x => x.CommandIssueDuration)).ToString("F2"));
                _report.Add(GPU_FRAMES_AVG, FrameMetricsData.ToMs(this.Frames.Sum(x => x.CommandIssueDuration) / this.Frames.Length).ToString("F2"));
            }

            private readonly Dictionary<string, string> _report;

            public IReadOnlyDictionary<string, string> Report => this._report;


            public TraceEvent Event { get; private set; }
            public FrameMetricsData[] Frames { get; private set; }
        }
    }
}
