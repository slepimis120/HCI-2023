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
using MongoDB.Driver;

namespace HCI_Tim_15_2023
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Random rnd = new Random();
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("hci");
            var collection = database.GetCollection<User>("users");
            var user = new User {Id = rnd.Next(1, 100000), Name = "Bob", Age = 30};
            
            collection.InsertOne(user);
            var users = collection.Find(Builders<User>.Filter.Empty).ToList();
            
            Console.WriteLine(users);
        }

        private void LoginAgent(object sender, RoutedEventArgs e)
        {
            AgentHomeWindow agentHomeWindow = new AgentHomeWindow();
            agentHomeWindow.Show();
            Close();
        }

        private void LoginClient(object sender, RoutedEventArgs e)
        {
            ClientHomeWindow clientHomeWindow = new ClientHomeWindow();
            clientHomeWindow.Show();
            Close();
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
