using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Tim_15_2023.Model;

public class User
{
    public string username, password;

    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}
