using ZstdSharp.Unsafe;

namespace HCI_Tim_15_2023.Model;

public class Location
{


    public string id { get; set; }
    public double lon { get; set; }
    public double lat { get; set; }
    public string address { get; set; }
    public string name { get; set; }
    public int cost {get;set;}

}