using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleMonitor
{
    public static class Utils
    {
        public static void AddListViewEntry(ListView Lv, string Param, string Value, string Description, ListViewGroup Lvg)
        {
            ListViewItem lvi = Lv.Items.Add(new ListViewItem(new string[] { Param, Value, Description }));
            lvi.Group = Lvg;
        }
    }
}
