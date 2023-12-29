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
    /// Interaction logic for EditHotkey.xaml
    /// </summary>
    /// 

    public partial class EditHotkey : Window
    {
        private DataSource dataSource;
        private Hotkey oldValue = new Hotkey();
        private ViewModel viewModel;

        public EditHotkey(ViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            Hotkey selectedHotkey = viewModel.selectedHotkey;
            dataSource = new DataSource();
            oldValue.hotkey = selectedHotkey.hotkey;
            oldValue.file = selectedHotkey.file;
            oldValue.arguments = selectedHotkey.arguments;
            oldValue.description = selectedHotkey.description;
            this.DataContext = selectedHotkey;            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.isHotkeyDuplicatted(viewModel))
            {
                MessageBox.Show("Please remove duplicated hotkeys before updating.", "Error updating!!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (hotkeyTextBox.Text.Split('+').Count() > 1 && !string.IsNullOrEmpty(fileTextBox.Text))
            {
                oldValue.hotkey = hotkeyTextBox.Text;
                oldValue.file = fileTextBox.Text;
                oldValue.arguments = argumentsTextBox.Text;
                oldValue.description = descriptionTextBox.Text;
            } else
            {
                MessageBox.Show("Hotkey is invalid.", "Error Saving!!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Close();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            oldValue.hotkey = "";
            oldValue.file = "";
            oldValue.arguments = "";
            oldValue.description = "";
            this.Close();
        }

        private void HotkeyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            HashSet<Key> pressedKeys = new HashSet<Key>();
            try
            {
                pressedKeys = dataSource.GetPressedKeys();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (pressedKeys.Any())
            {
                hotkeyTextBox.Text = Utils.converKeysToHotkeyString(pressedKeys);
            }            
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    if (file != null)
                    {
                        fileTextBox.Text = file;
                        fileTextBox.ToolTip = file;
                    }
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }

        private void EditHotkeyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            hotkeyTextBox.Text = oldValue.hotkey;
            fileTextBox.Text = oldValue.file;
            argumentsTextBox.Text = oldValue.arguments;
            descriptionTextBox.Text = oldValue.description;
            KeyListener.enableKeyListener();
        }        
    }
}
