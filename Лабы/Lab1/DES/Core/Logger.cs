using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core
{
    public static class Logger
    {
        private static TextBox TextBox { set; get; }

        public static void Initialize(TextBox textBox)
        {
            Logger.TextBox = textBox;
        }

        public static void Log(string text)
        {
            Logger.TextBox.Text += "\n\n-----------------------------------------------------------------------\n";
            Logger.TextBox.Text += text;
        }
    }
}
