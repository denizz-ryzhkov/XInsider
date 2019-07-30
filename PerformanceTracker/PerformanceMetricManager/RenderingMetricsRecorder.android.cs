using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using Android.App;
using Android.OS;
using Android.Service.Notification;
using Android.Util;
using Android.Views;

namespace PerformanceTracker
{
    public partial class RenderingMetricsRecorder
    {
        internal void PStart()
        {

        }

        internal void PStop()
        {

        }

        internal IList<FrameMetricsData> PGetFrames()
        {
            return this._storage.ToArray();
        }

        public void Attach(Application application)
        {
            var afm = new ActivityFrameMetrics();
            afm.SetTarget(this);
            application.RegisterActivityLifecycleCallbacks(afm);
        }

        internal void PushNewFrame(FrameMetricsData frameData)
        {
            this._storage.Add(frameData);
            Log.Warn("FrameMetricsDataData", frameData.InformationAboutFrame());
        }
    }

    public class ActivityFrameMetrics : Java.Lang.Object, Application.IActivityLifecycleCallbacks
    {
        //public void Dispose()
        //{
        //}

        //public IntPtr Handle { get; }
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
            this.StopFrameMetrics(activity);
        }

        public void OnActivityResumed(Activity activity)
        {
            this.StartFrameMetrics(activity);
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
            StopFrameMetrics(activity);
        }

        private OnFrameMetricsAvailableListener _listener;
        private WeakReference<Activity> _activity;
        private RenderingMetricsRecorder _renderingMetricsRecorder;

        public void StartFrameMetrics(Activity activity)
        {
            _activity = new WeakReference<Activity>(activity);
            var activityName = activity.Class.SimpleName;
            _listener = new OnFrameMetricsAvailableListener() { _activityName = activityName };
            _listener.SetTarget(_renderingMetricsRecorder);
            // activity.getWindow().addOnFrameMetricsAvailableListener(listener, new Handler());
            activity.Window.AddOnFrameMetricsAvailableListener(_listener, new Handler());
        }

        public void StopFrameMetrics(Activity activity)
        {
            try
            {
                if (_listener != null)
                {
                    //activity.Window.RemoveOnFrameMetricsAvailableListener(_listener);
                    _listener = null;
                }
            }
            catch (Exception e)
            {
            }
        }

        public bool TryToStopFrameMetrics()
        {
            if (_activity.TryGetTarget(out var acv))
            {
                StopFrameMetrics(acv);
                return true;
            }

            return false;
        }

        public bool TryToRunFrameMetrics()
        {
            if (_activity.TryGetTarget(out var acv))
            {
                StartFrameMetrics(acv);
                return true;
            }

            return false;
        }

        public class OnFrameMetricsAvailableListener : Java.Lang.Object, Window.IOnFrameMetricsAvailableListener
        {
            public string _activityName;
            private uint allFrames = 0;
            private uint jankyFrames = 0;

            private static float DEFAULT_WARNING_LEVEL_MS = 17.0f;
            private static float DEFAULT_ERROR_LEVEL_MS = 34.0f;

            private float warningLevelMs = DEFAULT_WARNING_LEVEL_MS;
            private float errorLevelMs = DEFAULT_ERROR_LEVEL_MS;
            private bool showWarning = true;
            private bool showError = true;
            private RenderingMetricsRecorder _renderingMetricsRecorder;

            //private readonly int TotalDuration = (int)FrameMetricsId.TotalDuration;

            //public OnFrameMetricsAvailableListener(string activityName)
            //{
            //    _activityName = activityName;
            //}

            //public void Dispose()
            //{
            //}

            //public IntPtr Handle { get; }
            public void OnFrameMetricsAvailable(Window window, FrameMetrics frameMetrics, int dropCountSinceLastInvocation)
            {
                //if (Looper.MainLooper == Looper.MyLooper())
                //{
                //    Log.Error("THIS IS UI THREAD", "UI THREAD");
                //}

                var frameMetricsCopy = new FrameMetrics(frameMetrics);
                //frameMetrics.GetMetric((int)FrameMetricsId.AnimationDuration);

                allFrames++;
                var totalDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.TotalDuration);
                var totalDurationMs = (float)(0.000001 * totalDuration);
                if (totalDurationMs > warningLevelMs)
                {
                    jankyFrames++;
                    //var msg = $"Janky frame detected on {_activityName} with total duration: {totalDurationMs}\n";

                    var d = new FrameMetricsData()
                    {
                        Offset = PTrackerTimeProvider.Source.Elapsed,
                        FrameNumber = allFrames,
                        TotalDuration = totalDuration,
                        LayoutMeasureDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.LayoutMeasureDuration),
                        DrawDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.DrawDuration),
                        CommandIssueDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.CommandIssueDuration),
                        JunkyFrameNumber = jankyFrames
                        //AnimationDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.AnimationDuration),
                        //FirstDrawFrame = frameMetricsCopy.GetMetric((int)FrameMetricsId.FirstDrawFrame),
                        //InputHandlingDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.InputHandlingDuration),
                        //IntendedVsyncTimestamp = frameMetricsCopy.GetMetric((int)FrameMetricsId.IntendedVsyncTimestamp),
                        //SwapBuffersDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.SwapBuffersDuration),
                        //SyncDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.SyncDuration),
                        //UnknownDelayDuration = frameMetricsCopy.GetMetric((int)FrameMetricsId.UnknownDelayDuration),
                        //VsyncTimestamp = frameMetricsCopy.GetMetric((int)FrameMetricsId.VsyncTimestamp),
                    };

                    _renderingMetricsRecorder.PushNewFrame(d);

                    //float layoutMeasureDurationMs = (float)(0.000001 * frameMetricsCopy.GetMetric((int)FrameMetricsId.LayoutMeasureDuration));
                    //float drawDurationMs = (float)(0.000001 * frameMetricsCopy.GetMetric((int)FrameMetricsId.DrawDuration));
                    //float gpuCommandMs = (float)(0.000001 * frameMetricsCopy.GetMetric((int)FrameMetricsId.CommandIssueDuration));
                    //float othersMs = totalDurationMs - layoutMeasureDurationMs - drawDurationMs - gpuCommandMs;
                    //float jankyPercent = (float)jankyFrames / allFrames * 100;
                    //msg += $"Layout/measure: {layoutMeasureDurationMs}ms, draw:{drawDurationMs}ms, gpuCommand:{gpuCommandMs}ms others:{othersMs}ms\n";
                    //msg += "Janky frames: " + jankyFrames + "/" + allFrames + "(" + jankyPercent + "%)";
                    //if (showWarning && totalDurationMs > errorLevelMs)
                    //{
                    //    Log.Error("FrameMetricsDataData", msg);
                    //}
                    //else if (showError)
                    //{
                    //    Log.Warn("FrameMetricsDataData", msg);
                    //}

                    //var d = new FrameMetricsData()
                    //{
                    //    FrameNumber = allFrames,
                    //    LayoutMeasureDuration = layoutMeasureDurationMs,
                    //    DrawDuration = drawDurationMs,
                    //    CommandIssueDuration = gpuCommandMs,

                    //};
                }
            }


            public void SetTarget(RenderingMetricsRecorder renderingMetricsRecorder)
            {
                this._renderingMetricsRecorder = renderingMetricsRecorder;
            }
        }



        public void SetTarget(RenderingMetricsRecorder renderingMetricsRecorder)
        {
            this._renderingMetricsRecorder = renderingMetricsRecorder;
        }
    }
}