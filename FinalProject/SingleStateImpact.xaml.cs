using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;

using LiveCharts;
using LiveCharts.Wpf;

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for SingleStateImpact.xaml
    /// </summary>
    public partial class SingleStateImpact : Page
    {
        public SingleStateImpact()
        {
            InitializeComponent();
        }

        private void update_graph()
        {
            DataContext = null;

            var db = new DatabaseConnection();

            MenuItem state = stateSelector.Items.GetItemAt(0) as MenuItem;

            int stateId = -1;
            string stateName = "";

            foreach (MenuItem item in state.Items)
            {
                if (item.IsChecked)
                {
                    stateName = item.Header as string;
                    stateId = db.getStateId(stateName);
                    break;
                }
            }

            MenuItem covidType = casesOrDeaths.Items.GetItemAt(0) as MenuItem;

            bool getDeaths = false;

            foreach (MenuItem item in covidType.Items)
            {
                if (item.IsChecked && (item.Header as string) == "Deaths")
                {
                    getDeaths = true;
                }
            }

            string casesDeathsName = "Cases";
            if (getDeaths) casesDeathsName = "Deaths";

            IEnumerable<DatabaseConnection.TripsCovidStr> response = db.getTripsandCovid(stateId, getDeaths);

            List<int> totalTrips = new List<int>();
            List<int> totalCases = new List<int>();

            Trace.WriteLine(response.Count());

            Trace.WriteLine(response.ElementAt(89).TRIPSTOTAL);
            Trace.WriteLine(response.ElementAt(89).SUMCASES);

            foreach (DatabaseConnection.TripsCovidStr pair in response)
            {
                totalTrips.Add(pair.TRIPSTOTAL);
                totalCases.Add(pair.SUMCASES);
            }

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = $"{stateName} COVID {casesDeathsName}",
                    Values = new ChartValues<int>(totalCases),
                    LineSmoothness = 0,
                    PointGeometrySize = 7,
                    PointGeometry = DefaultGeometries.Circle
                }
            };

            SeriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = $"{stateName} Trips",
                    Values = new ChartValues<int>(totalTrips),
                    LineSmoothness = 0,
                    PointGeometrySize = 7,
                    PointGeometry = DefaultGeometries.Circle
                }
            };

            string firstDate = db.getStartTimeTrips();
            string secondDate = db.getEndTimeTrips();

            Labels = new[] { "" };
            YFormatter = value => value.ToString("");
            TitleFormatter = $"Number of {casesDeathsName}";
            XTitleFormatter = XTitleFormatter = $"Start Date: {firstDate} - End Date: {secondDate}";

            Labels2 = new[] { "" };
            YFormatter2 = value => value.ToString("");
            TitleFormatter2 = $"Number of Trips";
            XTitleFormatter2 = XTitleFormatter = $"Start Date: {firstDate} - End Date: {secondDate}";

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public string TitleFormatter { get; set; }
        public string XTitleFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public SeriesCollection SeriesCollection2 { get; set; }
        public string[] Labels2 { get; set; }
        public string TitleFormatter2 { get; set; }
        public string XTitleFormatter2 { get; set; }
        public Func<double, string> YFormatter2 { get; set; }

        public void Visualize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            update_graph();
        }

        public void Return_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartPage page = new StartPage();
            this.NavigationService.Navigate(page);
        }

        public void Display_Query(object sender, RoutedEventArgs e)
        {
            queryResponse.Text = "SELECT TRIPSTOTAL, SUMCASES from TRIPS inner join (SELECT REPORTDATE, SUM(DEATHS) AS SUMCASES FROM COVID INNER JOIN COUNTY\nON COVID.FIPS = COUNTY.FIPS WHERE COUNTY.STATEID = {stateID} GROUP BY REPORTDATE) STATECOVID \nON (TRIPS.REPORTDATE + INTERVAL '3' DAY) = STATECOVID.REPORTDATE WHERE TRIPS.STATEID = {stateID} AND TRIPS.REPORTDATE >= '01-JAN-20' \nAND TRIPS.REPORTDATE < '01-APR-22' ORDER BY TRIPS.REPORTDATE ASC";

        }

        public void State_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            Trace.WriteLine(menuItem.Header);

            MenuItem parent = menuItem.Parent as MenuItem;

            Trace.WriteLine(parent.Header);
            Trace.WriteLine(parent.Items.Count);

            foreach (MenuItem item in parent.Items)
            {
                if (item.IsChecked && item.Header != menuItem.Header)
                {
                    item.IsChecked = false;
                }
            }

        }

        public void State_UnChecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
