using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace XwtPlus.TextEditor.Test
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
            Application.Initialize(ToolkitType.Gtk3);

			using (var window = new MainWindow())
			{
				window.Show();
				Application.Run();
			}

			Application.Dispose();
		}
	}
}
