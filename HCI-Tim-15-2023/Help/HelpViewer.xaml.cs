using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace HCI_Tim_15_2023.Help;

public partial class HelpViewer : Window
{
    private JavaScriptControlHelper ch;

    public HelpViewer(string key, MainWindow originator)
    {
        InitializeComponent();
        string curDir = Directory.GetCurrentDirectory();
        string parentDir = Directory.GetParent(Directory.GetParent(Directory.GetParent(curDir).FullName).FullName).FullName;
        string fullPath = Path.Combine(parentDir, "Help", $"{key}.htm");

        if (!File.Exists(fullPath))
        {
            key = "error";
            fullPath = Path.Combine(parentDir, "Help", "error.htm");
        }

        Uri uri = new Uri(fullPath);
        ch = new JavaScriptControlHelper(originator);
        wbHelp.ObjectForScripting = ch;
        wbHelp.Navigate(uri);
    }


    private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((wbHelp != null) && (wbHelp.CanGoBack));
    }

    private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        wbHelp.GoBack();
    }

    private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ((wbHelp != null) && (wbHelp.CanGoForward));
    }

    private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        wbHelp.GoForward();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
    }

    private void wbHelp_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
    {
    }
}