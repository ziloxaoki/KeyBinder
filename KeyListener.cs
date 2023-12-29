using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;

namespace KeyBinder
{
    class KeyListener
    {
        private static HashSet<Key> pressedKeys = new HashSet<Key>();
        private static bool isEnabled = true;
        private static readonly Object lockIt = new object();
        HotkeyProcessor processor;
        CancellationToken token;

        public KeyListener(CancellationToken token, ViewModel viewModel)
        {
            processor = new HotkeyProcessor(token, viewModel);
            this.token = token;
        }

        public static void disableKeyListener()
        {
            lock (lockIt)
            {
                isEnabled = false;
            }
        }

        public static void enableKeyListener()
        {
            lock (lockIt)
            {
                isEnabled = true;
            }
        }

        /// <summary>
        /// Starts infinite run of the keylogger
        /// </summary>
        public async void StartMonitorAsync()
        {
            Console.WriteLine("Initializing monitor...");

            DataSource dataSource = new DataSource();
            Task.Run(() => processor.StartProcessorAsync());

            while (true && !token.IsCancellationRequested)
            {
                if (isEnabled)
                {
                    try
                    {
                        pressedKeys = dataSource.GetPressedKeys();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Closing thread KeyListener...");
                        return;
                    }

                    if (pressedKeys.Any())
                    {
                        processKeys(pressedKeys);
                    }
                }

                Thread.Sleep(50);
            }

            Console.WriteLine("Closing thread KeyListener...");

        }

        public void processKeys(HashSet<Key> pressedKeys)
        {
            if (pressedKeys.Count > 1)
            {
                processor.addHotkey(Utils.converKeysToHotkeyString(pressedKeys));                
            }
        }
    }
}
