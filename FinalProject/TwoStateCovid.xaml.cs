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
    /// Interaction logic for SingleStateCovid.xaml
    /// </summary>
    public partial class TwoStateCovid : Page
    {
        public TwoStateCovid()
        {
            InitializeComponent();
        }

        private void update_graph()
        {
            DataContext = null;
            
            var db = new DatabaseConnection();

            MenuItem covidType = casesOrDeaths.Items.GetItemAt(0) as MenuItem;

            int getDeaths = 0;

            foreach (MenuItem item in covidType.Items)
            {
                if (item.IsChecked && (item.Header as string) == "Deaths")
                {
                    getDeaths = 1;
                }
                if (item.IsChecked && (item.Header as string) == "Cases per Death")
                {
                    getDeaths = 2;
                }
            }

            string casesDeathsName = "Cases";
            if (getDeaths == 1) casesDeathsName = "Deaths";
            if (getDeaths == 2) casesDeathsName = "Cases per Death";

            MenuItem state1 = stateSelector1.Items.GetItemAt(0) as MenuItem;
            
            int stateId = -1;
            string stateName1 = "";

            foreach (MenuItem item in state1.Items)
            {
                if (item.IsChecked)
                {
                    stateName1 = item.Header as string;
                    stateId = db.getStateId(stateName1);
                    break;
                }
            }

            IEnumerable<int> data1 = db.complexTotalCovid(stateId, getDeaths);

            MenuItem state2 = stateSelector2.Items.GetItemAt(0) as MenuItem;

            string stateName2 = "";

            foreach (MenuItem item in state2.Items)
            {
                if (item.IsChecked)
                {
                    stateName2 = item.Header as string;
                    stateId = db.getStateId(stateName2);
                    break;
                }
            }

            IEnumerable<int> data2 = db.complexTotalCovid(stateId, getDeaths);

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = $"{stateName1} COVID {casesDeathsName}",
                    Values = new ChartValues<int>(data1),
                    LineSmoothness = 0,
                    PointGeometrySize = 7,
                    PointGeometry = DefaultGeometries.Circle
                },
                new LineSeries
                {
                    Title = $"{stateName2} COVID {casesDeathsName}",
                    Values = new ChartValues<int>(data2),
                    LineSmoothness = 0,
                    PointGeometrySize = 7,
                    PointGeometry = DefaultGeometries.Circle
                },
            };

            string firstDate = db.getStartTime();
            string secondDate = db.getEndTime();

            Labels = new[] { "" };
            YFormatter = value => value.ToString("");
            TitleFormatter = $"Number of {casesDeathsName}";
            XTitleFormatter = $"Start Date: {firstDate} - End Date: {secondDate}";

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public string TitleFormatter { get; set; }
        public string XTitleFormatter { get; set; }
        public Func<double, string> YFormatter { get; set; }

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
            queryResponse.Text = "SELECT SUM({type}) FROM COVID INNER JOIN COUNTY ON COVID.FIPS = COUNTY.FIPS\nWHERE COUNTY.STATEID = {stateId} GROUP BY REPORTDATE ORDER BY REPORTDATE ASC";
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
