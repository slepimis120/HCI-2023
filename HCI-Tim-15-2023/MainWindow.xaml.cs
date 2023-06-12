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
using HCI_Tim_15_2023.GUI;
using HCI_Tim_15_2023.Help;
using HCI_Tim_15_2023.Model;
using MongoDB.Driver;

namespace HCI_Tim_15_2023
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public static NavigationService NavigationService =>
            ((MainWindow) Application.Current.MainWindow).MainFrame.NavigationService;

        public MainWindow()
            //TODO add app icon
        {
            InitializeComponent();
            MainFrame.NavigationService.Navigate(new LogInPage());
            
            Random rnd = new Random();
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("hci");
            var collection = database.GetCollection<User>("users");
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IInputElement focusedControl = FocusManager.GetFocusedElement(Application.Current.Windows[0]);
            Page currentPage = MainFrame.Content as Page;
            if (currentPage != null)
            {
                string currentPageTitle = currentPage.Title;
                Console.WriteLine(currentPageTitle);
            }
            HelpProvider.ShowHelp("index", this);
        }

        public void doThings(string param)
        {
            Title = param;
        }
    }
}