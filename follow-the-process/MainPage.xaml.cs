using HubApp1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Util;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace follow_the_process
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Dictionary<string, object> ViewModel { get; set; }
        public ObservableCollection<Ann> PrevTradingDayAnns { get; set; }
        private readonly string _googleUrl = @"https://www.google.com.au/search?q=";

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.Loaded += PageLoaded;

            ConfirmInternet();

            ViewModel = new Dictionary<string, object>();
            SetTraderPersonalities();
            SetExistingPosCharts();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            // Disabled as feature superseeded by "gitlab/company-descs"
            //SetPrevTradingDayAnns();
        }

        private async void ConfirmInternet()
        {
            bool connectedToInternet = NetworkInterface.GetIsNetworkAvailable();
            if (!connectedToInternet)
            {
                await new MessageDialog("(╯°□°)╯︵ ┻━┻ need internet").ShowAsync();
            }
        }

        private void SetTraderPersonalities()
        {
            ViewModel["traderPersonalities"] = new List<TraderPersonality>()
            {
                new TraderPersonality("msm ^?"),
                new TraderPersonality("fbr ^?"),
                new TraderPersonality("utr >?")
            };
        }

        private void SetExistingPosCharts()
        {
            ViewModel["existingPositions"] = new List<Chart>()
            {
                new Chart("bub"),
                new Chart("msm"),
                new Chart("utr"),
                new Chart("brn"),
                new Chart("---"),
                new Chart("esh"),
                new Chart("fbr"),
                new Chart("---"),
                new Chart("nme"),
                new Chart("sxx"),
                new Chart("rbx"),
                new Chart("cdt"),
                new Chart("cbq"),
                new Chart("mgu"),
                new Chart("chz")
            };
        }

        /* private async void SetPrevTradingDayAnns()
        {
            List<Ann> anns = await Asx.PriceSensAnns();
            ListView annsListView = FindDescendant<ListView>(AnnsHubSection);
            if (annsListView != null)
            {
                annsListView.ItemsSource = anns;
            }
            else
            {
                throw new NullReferenceException("Failed to show announcements, because cant find ListView inside the hub section: AnnsHubSection");
            }
        }
        */

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            string company = ((CheckBox)sender).Tag.ToString();
            await Launcher.LaunchUriAsync(GoogleTkrUri(company));
        }

        private Uri GoogleTkrUri(string company)
        {
            string query = company.Replace(' ', '+');
            return new Uri(_googleUrl + query);
        }

        private async void ExistingPosChart_DoubleTapped(object sender, RoutedEventArgs e)
        {
            string tkr = ((Image)sender).Tag.ToString().ToUpperInvariant();
            await new MessageDialog(tkr).ShowAsync();
        }

        //dm
        public static T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {
            // Check if this object is the specified type
            if (obj is T)
                return obj as T;

            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;

            // First check all the children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }

            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
        }

    }
}
