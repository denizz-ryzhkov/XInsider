using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerformanceTracker;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using XfDroidPerfReport.Models;
using XfDroidPerfReport.Views;
using XfDroidPerfReport.ViewModels;

namespace XfDroidPerfReport.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var ts = new TimelineSummary();
            var res = await ts.GetTextSummary();
            Debug.WriteLine(res);


            //ItemsListView.ScrollTo(viewModel.Items.Last(), ScrollToPosition.Start, true);

            //var item = args.SelectedItem as Item;
            //if (item == null)
            //    return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //// Manually deselect item.
            //ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TraceEventsHandler.Current.MakeEvent(TimeSpan.FromMilliseconds(2000), "Adding Elements", null, null);

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }

    //public class SpeedTester
    //{
    //    public event EventHandler<int> OnDataAvailable;

    //    public void StartDelegate()
    //    {

    //    }

    //    protected virtual void DataAvailable(int e)
    //    {
    //        OnDataAvailable?.Invoke(this, e);
    //    }
    //}
}