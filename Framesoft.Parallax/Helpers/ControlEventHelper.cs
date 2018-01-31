using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Framesoft.Parallax.Helpers
{
    public delegate void ListViewScrollEventArgs(ListView listView, int firstItemPosition, float deviceScale, float x, float y);

    public static class ControlEventHelper
    {
        public static Action<ListView, int, int> SetScrollAction { get; set; }
        public static Func<ListView, Tuple<int, int, int>> GetScrollFunction { get; set; }
        public static bool IsFirstScrollAfterListViewChanged { get; set; }
        static Dictionary<ListView, ListViewScrollEventArgs> ScrollItems { get; set; } = new Dictionary<ListView, ListViewScrollEventArgs>();

        public static void AddListViewScrollEvent(ListView listView, ListViewScrollEventArgs action)
        {
            ScrollItems[listView] = action;
        }

        public static ListViewScrollEventArgs GetListViewScrollAction(ListView listView)
        {
            if (!ScrollItems.ContainsKey(listView))
                return null;
            return ScrollItems[listView];
        }

    }
}
