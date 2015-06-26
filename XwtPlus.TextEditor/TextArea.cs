using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;
using XwtPlus.TextEditor.Margins;

namespace XwtPlus.TextEditor
{
    class TextArea : Canvas
    {
        const int StartOffset = 4;

        TextEditor editor;

        List<Margin> margins = new List<Margin>();
        TextViewMargin textViewMargin;

        public TextArea(TextEditor editor)
        {
            this.editor = editor;

            CanGetFocus = true;

            textViewMargin = new TextViewMargin(editor);

            margins.Add(new LineNumberMargin(editor));
            margins.Add(new PaddingMargin(5));
            margins.Add(textViewMargin);
        }

        public double ComputedWidth
        {
            get { return margins.Select(margin => margin.ComputedWidth).Sum(); }
        }

        protected override Size OnGetPreferredSize(SizeConstraint widthConstraint, SizeConstraint heightConstraint)
        {
            return new Size(ComputedWidth, textViewMargin.LineHeight * editor.Document.LineCount);
        }

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            ctx.Save();
            ctx.SetColor(editor.Options.Background);
            ctx.Rectangle(dirtyRect);
            ctx.Fill();
            ctx.Restore();

            UpdateMarginXOffsets();
            RenderMargins(ctx, dirtyRect);
        }

        void UpdateMarginXOffsets()
        {
            double currentX = 0;
            foreach (Margin margin in margins.Where(margin => margin.IsVisible))
            {
                margin.XOffset = currentX;
                currentX += margin.Width;
            }
        }

        public int YToLine(double yPos)
        {
            return textViewMargin.YToLine(yPos);
        }

        public double LineToY(int logicalLine)
        {
            return textViewMargin.LineToY(logicalLine);
        }

        public double GetLineHeight(DocumentLine line)
        {
            return textViewMargin.GetLineHeight(line);
        }

        void RenderMargins(Context ctx, Rectangle dirtyRect)
        {
            int startLine = YToLine(dirtyRect.Y);
            double startY = LineToY(startLine);
            double currentY = startY;

            for (int lineNumber = startLine; ; lineNumber++)
            {
                var line = editor.Document.GetLine(lineNumber);

                double lineHeight = GetLineHeight(line);
                foreach (var margin in this.margins.Where(margin => margin.IsVisible))
                {
                    margin.DrawBackground(ctx, dirtyRect, line, lineNumber, margin.XOffset, currentY, lineHeight);
                    margin.Draw(ctx, dirtyRect, line, lineNumber, margin.XOffset, currentY, lineHeight);
                }

                currentY += lineHeight;
                if (currentY > dirtyRect.Bottom)
                    break;
            }
        }

        Margin GetMarginAtX(double x, out double startingPos)
        {
            double currentX = 0;
            foreach (Margin margin in margins.Where(margin => margin.IsVisible))
            {
                if (currentX <= x && (x <= currentX + margin.Width || margin.Width < 0))
                {
                    startingPos = currentX;
                    return margin;
                }
                currentX += margin.Width;
            }
            startingPos = -1;
            return null;
        }

        internal void RedrawLine(int lineNumber)
        {
            var line = editor.Document.GetLine(lineNumber);
            var dirtyRect = new Rectangle(0, LineToY(lineNumber), ComputedWidth, GetLineHeight(line));
            QueueDraw(dirtyRect);
        }

        internal void RedrawLines(int start, int end)
        {
            var line = editor.Document.GetLine(start);
            int lineCount = end - start;
            var dirtyRect = new Rectangle(0, LineToY(start), ComputedWidth, GetLineHeight(line) * lineCount);
            QueueDraw(dirtyRect);
        }

        internal void RedrawPosition(int line, int column)
        {
            //STUB
            QueueDraw();
        }

        double pressPositionX, pressPositionY;
        protected override void OnButtonPressed(ButtonEventArgs args)
        {
            base.OnButtonPressed(args);

            pressPositionX = args.X;
            pressPositionY = args.Y;

            double startPos;
            Margin margin = GetMarginAtX(pressPositionX, out startPos);
            if (margin != null)
            {
                margin.MousePressed(new MarginMouseEventArgs(editor, args.Button, args.X, args.Y, args.MultiplePress));
            }

            editor.SetFocus();
        }

        internal void HandleKeyPressed(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Home:
                    editor.Caret.Column = 1;
                    Deselect();
                    break;
                case Key.Up:
                    editor.Caret.Line--;
                    Deselect();
                    break;
                case Key.Down:
                    editor.Caret.Line++;
                    Deselect();
                    break;
                case Key.Left:
                    {
                        if (editor.Selection.IsEmpty)
                        {
                            if (editor.Caret.Column == 1)
                            {
                                var line = editor.Document.GetLine(editor.Caret.Line - 1);
                                editor.Caret.Location = new DocumentLocation(editor.Caret.Line - 1, line.Length + 1);
                            }
                            else
                            {
                                editor.Caret.Column--;
                            }
                        }
                        else
                        {
                            editor.Caret.Offset = editor.Selection.Offset;
                        }
                        Deselect();
                        break;
                    }
                case Key.Right:
                    {
                        if (editor.Selection.IsEmpty)
                        {
                            var line = editor.Document.GetLine(editor.Caret.Line);
                            if (editor.Caret.Column > line.Length)
                            {
                                editor.Caret.Column = 1;
                                editor.Caret.Line++;
                            }
                            else
                            {
                                editor.Caret.Column++;
                            }
                        }
                        else
                        {
                            editor.Caret.Offset = editor.Selection.EndOffset;
                        }
                        Deselect();
                        break;
                    }
                case Key.Delete:
                    {
                        if (editor.Selection.IsEmpty)
                            editor.Document.Remove(editor.Document.GetOffset(editor.Caret.Location), 1);
                        else
                        {
                            editor.Document.Remove(editor.Selection);
                            Deselect();
                        }
                        QueueDraw();
                        break;
                    }
                case Key.BackSpace:
                    {
                        if (editor.Selection.IsEmpty)
                        {
                            editor.Document.Remove(editor.Document.GetOffset(editor.Caret.Location) - 1, 1);

                            if (editor.Caret.Column == 1)
                            {
                                var line = editor.Document.GetLine(editor.Caret.Line - 1);
                                editor.Caret.Location = new DocumentLocation(editor.Caret.Line - 1, line.Length + 1);
                            }
                            else
                            {
                                editor.Caret.Column--;
                            }
                        }
                        else
                        {
                            editor.Document.Remove(editor.Selection);
                            Deselect();
                        }
                        QueueDraw();
                        break;
                    }
                case Key.Tab:
                    InsertText("\t");
                    break;
                default:
                    e.Handled = false;
                    break;
            }

            if (e.Handled)
            {
                editor.ResetCaretState();
            }
        }

        void Deselect()
        {
            editor.Selection = new TextSegment();
        }

        internal void HandleTextInput(object sender, TextInputEventArgs args)
        {
            base.OnTextInput(args);

            InsertText(args.Text);

            editor.ResetCaretState();

            args.Handled = true;
        }

        void InsertText(string text)
        {
            if (text == "\b")
            {
                if (editor.Selection.IsEmpty)
                {
                    if (editor.Caret.Column == 1)
                    {
                        int newLine = --editor.Caret.Line;
                        editor.Caret.Location = new DocumentLocation(newLine, editor.Document.GetLine(newLine).Length + 1);

                        editor.Document.Remove(editor.Document.GetOffset(editor.Caret.Location), 1);
                    }
                    else
                    {
                        editor.Caret.Column--;
                        var tl = new DocumentLocation(editor.Caret.Line, editor.Caret.Column);
                        var offset = editor.Document.GetOffset(tl);
                        editor.Document.Remove(offset, 1);
                    }
                }
                else
                {
                    editor.Document.Remove(editor.Selection);
                    Deselect();
                }
            }
            else
            {
                if (!editor.Selection.IsEmpty)
                {
                    editor.Document.Remove(editor.Selection);
                    editor.Caret.Offset = editor.Selection.Offset;
                    Deselect();
                }

                if (text == "\r" || text == "\n")
                {
                    int offset = editor.Document.GetOffset(editor.Caret.Location);
                    string tabText = "";
                    if (editor.Options.IndentStyle == IndentStyle.Auto)
                    {
                        tabText = editor.Document.GetLine(editor.Caret.Line).GetIndentation(editor.Document);
                    }
                    editor.Document.Insert(offset, text + tabText);
                    editor.Caret.Location = new DocumentLocation(editor.Caret.Line + 1, tabText.Length + 1);
                }
                else
                {
                    int offset = editor.Document.GetOffset(editor.Caret.Location);
                    editor.Document.Insert(offset, text);
                    editor.Caret.Column += text.Length;
                }
            }

            QueueDraw();
        }

        List<Tuple<PointerButton, Action<double, double>>> mouseMotionTrackers = new List<Tuple<PointerButton, Action<double, double>>>();

        internal void RegisterMouseMotionTracker(PointerButton releaseButton, Action<double, double> callback)
        {
            mouseMotionTrackers.Add(Tuple.Create(releaseButton, callback));
        }

        protected override void OnMouseMoved(MouseMovedEventArgs args)
        {
            base.OnMouseMoved(args);

            NotifyTrackers(args.X, args.Y);
        }

        protected override void OnButtonReleased(ButtonEventArgs args)
        {
            base.OnButtonReleased(args);

            NotifyTrackers(args.X, args.Y);

            mouseMotionTrackers.RemoveAll(tracker => tracker.Item1 == args.Button);
        }

        void NotifyTrackers(double x, double y)
        {
            foreach (var mouseMotionTracker in mouseMotionTrackers)
            {
                mouseMotionTracker.Item2(x, y);
            }
        }
    }
}
