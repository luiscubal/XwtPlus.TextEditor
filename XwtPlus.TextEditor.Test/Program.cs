using System;
using Xwt;

namespace XwtPlus.TextEditor.Test
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
            Application.Initialize(ToolkitType.Gtk);

			using (var window = new MainWindow())
			{
				window.Show();
				Application.Run();
			}

			Application.Dispose();
		}
	}
}
