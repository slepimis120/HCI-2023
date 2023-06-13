using HCI_Tim_15_2023.GUI.Pregledi;
using HCI_Tim_15_2023.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace HCI_Tim_15_2023.GUI.CRUD
{
    /// <summary>
    /// Interaction logic for ReservationCreatePage.xaml
    /// </summary>
    public partial class ReservationCreatePage : Page
    {

        List<Travel> travels;

        public ReservationCreatePage()
        {
            InitializeComponent();
            AddTravels();
        }

        public ReservationCreatePage(Travel travel)
        {
            InitializeComponent();
            AddTravels();
            SelectTravel(travel);
        }

        public void AddTravels()
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "hci";
            string collectionName = "travels";

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(databaseName);
            var collection = database.GetCollection<Travel>(collectionName);
            var filter = Builders<Travel>.Filter.Empty;

            this.travels = collection.Find(filter).ToList();

            for (int i = 0; i < this.travels.Count; i++)
            {
                Travels.Items.Add(this.travels[i].name);
            }
        }

        public void SelectTravel(Travel travel)
        {
            Travels.SelectedIndex = Travels.Items.IndexOf(travel.name);
        }

        public void ReserveTrip(object sender, RoutedEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            User user = window.loggedUser;
            Travel travel = new Travel();
            bool exists = false;
            string value = Travels.Text;

            for (int i = 0; i < this.travels.Count; i++)
            {
                if (this.travels[i].name == value)
                {
                    travel = this.travels[i];
                    exists = true;
                    break;
                }
            }

            

            if(DatePick.SelectedDate == null || user == null || exists == false)
            {
                MessageBox.Show("There was an error. Try again!");
            }
            else
            {
                DateTime date = DatePick.SelectedDate.Value.Date;

                string connectionString = "mongodb://localhost:27017";
                string databaseName = "hci";
                string collectionName = "reservations";

                var client = new MongoClient(connectionString);

                var database = client.GetDatabase(databaseName);
                var collection = database.GetCollection<BoughtTravel>(collectionName);

                BoughtTravel boughtTravel = new BoughtTravel(GenerateUniqueID(collection), user, travel, date);

                collection.InsertOne(boughtTravel);

                MessageBox.Show("Travel successfully reserved!");

                this.NavigationService.Navigate(new ClientHomePage());
            }

        }

        private string GenerateUniqueID(IMongoCollection<BoughtTravel> collection)
        {
            string newId;
            bool idExists;

            do
            {
                int randomNumber = new Random().Next(100000, 999999);
                newId = randomNumber.ToString();

                var idFilter = Builders<BoughtTravel>.Filter.Eq(r => r.id, newId);
                idExists = collection.Find(idFilter).Any();
            } while (idExists);

            return newId;
        }
    }
}
