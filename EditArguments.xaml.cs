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
using System.Windows.Shapes;

namespace KeyBinder
{
    /// <summary>
    /// Interaction logic for EditArguments.xaml
    /// </summary>
    public partial class EditArguments : Window
    {
        private DataSource dataSource;
        private string oldValue;

        public EditArguments(Hotkey selectedHotkey)
        {
            InitializeComponent();
            dataSource = new DataSource();
            oldValue = selectedHotkey.arguments;
            this.DataContext = selectedHotkey;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(argumentsTextBox.Text))
            {
                oldValue = argumentsTextBox.Text;
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            argumentsTextBox.Text = oldValue;
        }
    }
}
