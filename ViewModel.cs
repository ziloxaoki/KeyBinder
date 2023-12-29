using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyBinder
{
    public class ViewModel
    {
        public ObservableCollection<Hotkey> DataGridItems { get; set; }
        public Hotkey selectedHotkey {
            get; set; }
    }
}
