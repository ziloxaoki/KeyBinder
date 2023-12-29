using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KeyBinder
{
    class Utils
    {
        /// <summary>
        /// Gets all values of given enum
        /// </summary>
        /// <typeparam name="T">Enum whose values we want</typeparam>
        /// <returns>Values of the enum</returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static bool CompareLists<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();

            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        public static string converKeysToHotkeyString(HashSet<Key> keys)
        {
            return String.Join(" + ", keys.OrderBy(x => x));
        }

        public static bool isHotkeyDuplicatted(ViewModel viewModel)
        {
            HashSet<String> hotkeys = new HashSet<string>(viewModel.DataGridItems.Select(item => item.hotkey));

            return viewModel.DataGridItems.Count() > hotkeys.Count;
        }

        public static bool isHotkeyDuplicatted(string hotkey, ViewModel viewModel)
        {
            return viewModel.DataGridItems.Where(entry => entry.hotkey.Equals(hotkey)).Count() > 0;
        }
    }
}
