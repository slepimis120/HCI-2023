using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace HCI_Tim_15_2023.Help;

[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
[ComVisible(true)]
public class JavaScriptControlHelper
{
    MainWindow prozor;
    public JavaScriptControlHelper(MainWindow w)
    {
        prozor = w;
    }

    public void RunFromJavascript(string param)
    {
        //prozor.doThings(param);
    }
}