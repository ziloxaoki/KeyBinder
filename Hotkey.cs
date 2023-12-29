using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyBinder
{
    public class Hotkey
    {
        private const string SEPARATOR = ",";
        public string hotkey { get; set; } = "";


        public string file { get; set; } = "";

        public string arguments { get; set; } = "";

        public string description { get; set; } = "";
        public string[] keys { get => hotkey.Split('+'); set => keys = value; }

        public string toCSV()
        {
            return hotkey + SEPARATOR + file + SEPARATOR + arguments + SEPARATOR + description;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
