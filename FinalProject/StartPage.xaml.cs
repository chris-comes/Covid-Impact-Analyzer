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

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        public void State_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SingleStateCovid page = new SingleStateCovid();
            this.NavigationService.Navigate(page);
        }

        public void State_Comp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            TwoStateCovid page = new TwoStateCovid();
            this.NavigationService.Navigate(page);
        }

        public void State_Impact_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SingleStateImpact page = new SingleStateImpact();
            this.NavigationService.Navigate(page);
        }

        public void State_Impact_Comp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var db = new DatabaseConnection();
            db.testConnection();
        }

        public void Database_Size(object sender, System.Windows.RoutedEventArgs e)
        {
            var db = new DatabaseConnection();
            int size = db.getSize();

            sizeBox.Text = $"Covid table contains: {size.ToString()} tuples";
        }
    }
}
