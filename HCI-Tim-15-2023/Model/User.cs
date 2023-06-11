using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Tim_15_2023.Model;

public enum roles { CLIENT, ADMIN}

public class User
{
    public string username, password, id;
    public roles roles;
    

    public User(string id, string username, string password, roles roles)
    {
        this.roles = roles;
        this.id = id;
        this.username = username;
        this.password = password;
    }
}
