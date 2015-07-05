using System;
using Xwt;
using XwtPlus.TextEditor;

namespace Sample
{
	public class MainWindow : Window
	{
		TextEditor textEditor;

		public MainWindow()
		{
			Title = "Hello, World!";
			InitialLocation = WindowLocation.CenterScreen;
			Width = 500;
			Height = 400;

            Content = textEditor = new TextEditor();
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
