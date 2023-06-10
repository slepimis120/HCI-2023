using System.Windows;
using System.Windows.Controls;

namespace HCI_Tim_15_2023.GUI.CRUD
{
    public partial class CreateLocationDialog : Window
    {
        public CreateLocationDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        
        public TextBox NameTextBox { get { return nameTextBox; } }
        public TextBox AddressTextBox { get { return addressTextBox; } }
        public TextBox CostTextBox { get { return costTextBox; } }
    }
}