using System.Windows;
using System.Threading;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace KeyBinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;                
        private readonly ViewModel viewModel;

        protected CancellationTokenSource tokenSource = null;

        public MainWindow()
        {            
            tokenSource = new CancellationTokenSource();

            List<string[]> fileContent = FileManager.getFileContent();

            this.viewModel = new ViewModel
            {
                DataGridItems = new ObservableCollection<Hotkey>(fileContent.Select(entry => new Hotkey
                {
                    hotkey = entry[0],
                    file = entry[1],
                    arguments = entry[2],
                    description = entry[3]
                }))
            };

            this.DataContext = this.viewModel;

            InitializeComponent();

            startThreads(tokenSource.Token);

            setSystemTrayProperties();

            minimizeOnStartUp();
        }

        private void minimizeOnStartUp()
        {
            this.Hide();
            changeWindowStateDelegate(WindowState.Minimized);
            this.ShowInTaskbar = false;
            _notifyIcon.Visible = true;            
        }

        private void changeWindowStateDelegate(WindowState state)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(delegate ()
                {
                    this.WindowState = state;
                })
            );
        }

        private void setSystemTrayProperties()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.Icon = new System.Drawing.Icon("Resources/keyboard-24x24.ico");            
            _notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(NotifyIcon_Click);
            //m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            _notifyIcon.BalloonTipTitle = "KeyBinder";
            _notifyIcon.Text = "KeyBinder";
            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", System.Drawing.Image.FromFile("Resources/keyboard-24x24.ico"), OnExitClicked);
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {            
            if (((System.Windows.Forms.MouseEventArgs)e).Button == System.Windows.Forms.MouseButtons.Left)
            {
                changeWindowStateDelegate(WindowState.Normal);
                this.Activate();
                this.Show();
            }
        }

        private async void startThreads(CancellationToken token)
        {
            KeyListener keyListener = new KeyListener(token, viewModel);
            await Task.Run(() => keyListener.StartMonitorAsync());
        }

        void OnClose(object sender, CancelEventArgs args)
        {
            _notifyIcon.Dispose();
            _notifyIcon = null;
            tokenSource.Cancel();
        }

        void OnStateChanged(object sender, EventArgs args)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                _notifyIcon.Visible = true;
                //if (m_notifyIcon != null)
                //    m_notifyIcon.ShowBalloonTip(2000);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                _notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            }

        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            changeWindowStateDelegate(WindowState.Normal);
            this.Activate();
        }

        void ShowTrayIcon(bool show)
        {
            if (_notifyIcon != null)
                _notifyIcon.Visible = show;
        }

        private void DataGridCell_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            KeyListener.disableKeyListener();
            DataGridRow selectedRow = DataGridRow.GetRowContainingElement(sender as DataGridCell);
            int selectedRowIndex = selectedRow.GetIndex();

            if (selectedRowIndex >= viewModel.DataGridItems.Count())
            {
                addNewItem();
            }
            EditHotkey editWindow = new EditHotkey(this.viewModel);
            editWindow.ShowDialog();
        }
        
        protected void updateHotKey(string value)
        {
            this.viewModel.selectedHotkey.hotkey = value;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Utils.isHotkeyDuplicatted(viewModel))
            {
                System.Windows.Forms.MessageBox.Show("Please remove duplicated hotkeys before saving.", 
                    "Error Saving!!!", 
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("Program will have to be restarted. Do you wish to continue?", 
                "", 
                System.Windows.Forms.MessageBoxButtons.YesNo, 
                System.Windows.Forms.MessageBoxIcon.Warning);

            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                FileManager.saveDataToFile(viewModel);
                System.Windows.Forms.MessageBox.Show("Changes saved to file");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }            
        }

        private Hotkey addNewItem()
        {
            Hotkey newItem = new Hotkey();
            viewModel.DataGridItems.Add(newItem);
            this.viewModel.selectedHotkey = newItem;
            dataGrid.SelectedIndex = dataGrid.Items.Count;
            return newItem;
        }

        private void cleanEmptyRows()
        {
            List<int> indexes = new List<int>();
            int index = 0;
            foreach (Hotkey hotkey in viewModel.DataGridItems) {
                if (string.IsNullOrEmpty(hotkey.hotkey))
                {
                    indexes.Add(index);
                }
                index++;
            }

            foreach (int i in indexes.Reverse<int>())
            {
                viewModel.DataGridItems.RemoveAt(i);
            }
        }
    }
}
