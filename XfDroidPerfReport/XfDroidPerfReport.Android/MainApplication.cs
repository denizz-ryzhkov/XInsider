﻿using System;
using Android.App;
using Android.Runtime;
using Android.Widget;
using PerformanceTracker;

namespace XfDroidPerfReport.Droid
{
    [Application]
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
            : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            RenderingMetricsRecorder.Current.Attach(this);
            //CrossCurrentActivity.Current.Init(this);
            //CrossCurrentActivity.Current.ActivityStateChanged += Current_ActivityStateChanged;
        }

        //private void Current_ActivityStateChanged(object sender, ActivityEventArgs e)
        //{
        //    Toast.MakeText(Application.Context, $"Activity Changed: {e.Activity.LocalClassName} -  {e.TraceEvent}", ToastLength.Short).Show();

        //}
    }
}