using Mono.TextEditor.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt.Drawing;

namespace XwtPlus.TextEditor
{
    public class TextEditorOptions
    {
        public Font EditorFont = Font.FromName("Consolas 13");
        public IndentStyle IndentStyle = IndentStyle.Auto;
        public int TabSize = 4;
        public Color Background = Colors.White;
        public ColorScheme ColorScheme = SyntaxModeService.DefaultColorStyle;
        public bool CurrentLineNumberBold = true;
    }
}
