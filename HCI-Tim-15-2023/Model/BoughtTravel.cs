using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCI_Tim_15_2023.Model;

public class BoughtTravel
{
    public string id;
    public User user;
    public Travel travel;
    public DateTime time;

    public BoughtTravel(string id, User user, Travel travel, DateTime time)
    {
        this.id = id;
        this.user = user;
        this.travel = travel;
        this.time = time;
    }

    public bool IsDone()
    {
        if(time < new DateTime())
            return true;
        else
            return false;
    }
}
