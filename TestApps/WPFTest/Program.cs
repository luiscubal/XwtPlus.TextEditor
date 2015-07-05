using System;
using Xwt;

using Sample;

namespace WPFTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Initialize(ToolkitType.Wpf);

            MainWindow window = new MainWindow();
            window.Show();

            Application.Run();
        }
    }
}

