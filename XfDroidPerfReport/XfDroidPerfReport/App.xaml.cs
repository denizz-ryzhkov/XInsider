using System;
using PerformanceTracker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XfDroidPerfReport.Services;
using XfDroidPerfReport.Views;

namespace XfDroidPerfReport
{
    public partial class App : Application
    {
        //public static CombinedMetricRecorder MetricRecorder = new CombinedMetricRecorder();
        public App()
        {
            TraceEventsHandler.Current.Checkpoint("App .ctor");
            InitializeComponent();
            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
