using System;
using Xwt;
using XwtPlus.TextEditor;

namespace XwtPlus.TextEditor.Test
{
	class MainWindow : Window
	{
        ScrollView scrollView1;
		TextEditor textEditor;

		public MainWindow()
		{
			Title = "Hello, World!";
			InitialLocation = WindowLocation.CenterScreen;
			Width = 500;
			Height = 400;

            scrollView1 = new ScrollView(textEditor = new TextEditor());
            Content = scrollView1;
			Padding = new WidgetSpacing();

			Closed += (sender, e) => Application.Exit();

			OpenFile();
		}

		private void OpenFile()
		{
			var text = "using System;\nclass Foo {\n}\n";
			textEditor.Document.Text = text;
			textEditor.Document.MimeType = "text/x-csharp";
		}
	}
}
