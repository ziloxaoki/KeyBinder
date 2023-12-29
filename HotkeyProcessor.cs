﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyBinder
{
    class HotkeyProcessor
    {
        private ObservableCollection<Hotkey> listOfConfiguredHotkeys;
        private static HashSet<string> processes = new HashSet<string>();        
        private readonly Object lockIt = new Object();        
        CancellationToken token;


        public HotkeyProcessor(CancellationToken token, ViewModel viewModel)
        {
            this.token = token;
            listOfConfiguredHotkeys = new ObservableCollection<Hotkey>(
                viewModel.DataGridItems.Select(
                    entry => new Hotkey {
                        hotkey = entry.hotkey,
                        file = entry.file,
                        arguments = entry.arguments,
                        description = entry.description,
                    }).ToList());
        }        

        public async void StartProcessorAsync()
        {
            Console.WriteLine("Initializing hotkey processor...");

            while (true && !token.IsCancellationRequested)
            {                
                foreach (string hotkeyStr in processes.ToList())
                {
                    IEnumerable<Hotkey> hotkeyIE = listOfConfiguredHotkeys.Where(entry => entry.hotkey.Equals(hotkeyStr));
                    if (hotkeyIE.Any())
                    {
                        Hotkey hotkeyObj = hotkeyIE.ElementAt(0);

                        using (Process p = new Process())
                        {
                            p.StartInfo.FileName = hotkeyObj.file;

                            // Stop the process from opening a new window
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.CreateNoWindow = true;

                            if (!String.IsNullOrEmpty(hotkeyObj.arguments))
                            {
                                p.StartInfo.Arguments = hotkeyObj.arguments;
                            }

                            // Go
                            p.Start();
                        }

                        //sleep for 3s to avoid user triggering same shortcut several times
                        Thread.Sleep(3000);

                        lock (lockIt)
                        {
                            processes.Remove(hotkeyObj.hotkey);
                        }
                    }
                }
                Thread.Sleep(200);
            }
            Console.WriteLine("Closing thread HotkeyProcessor...");
        }

        public void addHotkey(string hotkey)
        {
            lock (lockIt)
            {
                if (!processes.Contains(hotkey))
                {
                    try
                    {
                        processes.Add(hotkey);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}