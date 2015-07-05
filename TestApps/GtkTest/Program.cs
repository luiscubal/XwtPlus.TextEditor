using System;
using Xwt;

using Sample;

namespace GtkTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(ToolkitType.Gtk);

            MainWindow window = new MainWindow();
            window.Show();

            Application.Run();
        }
    }
}

