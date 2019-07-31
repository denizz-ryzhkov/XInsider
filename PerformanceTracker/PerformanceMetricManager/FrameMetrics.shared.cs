using System;
using System.Collections.Generic;

namespace PerformanceTracker
{
    public class FrameMetricsData
    {
        public uint FrameNumber { get; set; }
        public uint JunkyFrameNumber { get; set; }
        public TimeSpan Offset { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.UnknownDelayDuration", JniField = "android/view/FrameMetrics.UNKNOWN_DELAY_DURATION")]
        public long UnknownDelayDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.InputHandlingDuration", JniField = "android/view/FrameMetrics.INPUT_HANDLING_DURATION")]
        public long InputHandlingDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.AnimationDuration", JniField = "android/view/FrameMetrics.ANIMATION_DURATION")]
        public long AnimationDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.LayoutMeasureDuration", JniField = "android/view/FrameMetrics.LAYOUT_MEASURE_DURATION")]
        public long LayoutMeasureDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.DrawDuration", JniField = "android/view/FrameMetrics.DRAW_DURATION")]
        public long DrawDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.SyncDuration", JniField = "android/view/FrameMetrics.SYNC_DURATION")]
        public long SyncDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.CommandIssueDuration", JniField = "android/view/FrameMetrics.COMMAND_ISSUE_DURATION")]
        public long CommandIssueDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.SwapBuffersDuration", JniField = "android/view/FrameMetrics.SWAP_BUFFERS_DURATION")]
        public long SwapBuffersDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.TotalDuration", JniField = "android/view/FrameMetrics.TOTAL_DURATION")]
        public long TotalDuration { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.FirstDrawFrame", JniField = "android/view/FrameMetrics.FIRST_DRAW_FRAME")]
        public long FirstDrawFrame { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.IntendedVsyncTimestamp", JniField = "android/view/FrameMetrics.INTENDED_VSYNC_TIMESTAMP")]
        public long IntendedVsyncTimestamp { get; set; }
        //[IntDefinition("Android.Views.FrameMetrics.VsyncTimestamp", JniField = "android/view/FrameMetrics.VSYNC_TIMESTAMP")]
        public long VsyncTimestamp { get; set; }


        public FrameMetricsData()
        {

        }
        public string InformationAboutFrame()
        {
            float layoutMeasureDurationMs = ToMs(LayoutMeasureDuration);
            float drawDurationMs = ToMs(DrawDuration);
            float gpuCommandMs = ToMs(CommandIssueDuration);
            var totalDurationMs = ToMs(TotalDuration);
            var allFrames = FrameNumber;
            var jankyFrames = JunkyFrameNumber;

            float othersMs = totalDurationMs - layoutMeasureDurationMs - drawDurationMs - gpuCommandMs;
            float jankyPercent = (float)jankyFrames / allFrames * 100;

            var msg = $"Janky frame detected on Activity with total duration: {totalDurationMs}\n";
            msg += $"Layout/measure: {layoutMeasureDurationMs}ms, draw:{drawDurationMs}ms, gpuCommand:{gpuCommandMs}ms others:{othersMs}ms\n";
            msg += "Janky frames: " + jankyFrames + "/" + allFrames + "(" + jankyPercent + "%)";
            return msg;

            //Log.Warn("FrameMetricsDataData", msg);
        }

        public static float ToMs(long lng)
        {
            return (float)(0.000001 * lng);
        }
    }
}