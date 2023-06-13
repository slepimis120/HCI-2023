using HCI_Tim_15_2023.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI;
public partial class LogInPage : Page
{
    public LogInPage()
    {
        InitializeComponent();
    }

    private List<User> GetUsersFromDB()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "hci";
        string collectionName = "users";

        var client = new MongoClient(connectionString);

        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<User>(collectionName);
        var filter = Builders<User>.Filter.Empty;
        var users = collection.Find(filter).ToList();

        return users;
    }

    private void Login(object sender, RoutedEventArgs e)
    {
        List<User> users = GetUsersFromDB();
        bool exists = false;
        if (Username.Text == "" || Password.Password.ToString() == "")
        {
            exists = true;
            MessageBox.Show("Fill in the blanks!");
        }
        else
        {
            for (int i = 0; i < users.Count; i++){ 
                if (Username.Text == users[i].username && Password.Password.ToString() == users[i].password && users[i].roles == roles.CLIENT)
                {
                    exists = true;
                    var window = (MainWindow)Application.Current.MainWindow;
                    window.loggedUser = users[i];
                    this.NavigationService.Navigate(new ClientHomePage());
                }
                else if (Username.Text == users[i].username && Password.Password.ToString() == users[i].password && users[i].roles == roles.ADMIN)
                {
                    exists = true;
                    var window = (MainWindow)Application.Current.MainWindow;
                    window.loggedUser = users[i];
                    this.NavigationService.Navigate(new AgentHomePage());
                    
                }
            }
        }
        if(exists == false)
        {
            MessageBox.Show("User does not exist. Try again!");
        }
    }

    private void RegisterClient(object sender, RoutedEventArgs e)
    {
        this.NavigationService.Navigate(new RegisterPage());
    }
}
