using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Graphics;

namespace GraphEditor.Tools
{
    /// <summary>
    /// Text tool
    /// </summary>
    class ToolText : ToolRectangleBase
    {
        TextBox textBox = null;
        string oldText;
        GraphicsBase cur_graphics;
        GraphCanvas drawingCanvas;

        public ToolText(GraphCanvas drawingCanvas)
        {
            this.drawingCanvas = drawingCanvas;
			MemoryStream stream = new MemoryStream(GraphEditor.Properties.Resources.Text);
            ToolCursor = new Cursor(stream);
        }

        /// <summary>
        /// Textbox is exposed for DrawingCanvas Visual Children Overrides.
        /// If it is not null, overrides should include this textbox.
        /// </summary>
        public TextBox TextBox
        {
            get { return textBox; }
            set { textBox = value; }
        }

        public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(drawingCanvas);
            GraphicsBase o;
            cur_graphics = null;

            for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
            {
                o = drawingCanvas[i];
                if (o.MakeHitTest(point) == 0)
                {
                    cur_graphics = o;
                    HelperFunctions.UnselectAll(drawingCanvas);
                    cur_graphics.IsSelected = true;
                    break;
                }
            }

            if (cur_graphics == null)
            {
                HelperFunctions.UnselectAll(drawingCanvas);
            }
            drawingCanvas.CaptureMouse();
        }

        public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
        {
            drawingCanvas.ReleaseMouseCapture();
            if (cur_graphics != null && textBox == null)
                CreateTextBox(cur_graphics, drawingCanvas);
        }

        /// <summary>
        /// Create textbox for in-place editing
        /// </summary>
        public void CreateTextBox(GraphicsBase graphics, GraphCanvas drawingCanvas)
        {
			// Set graphics if not set
			if (!ReferenceEquals(cur_graphics, graphics))
			{
				cur_graphics = graphics;
			}

            oldText = graphics.Label;
            textBox = new TextBox();
            
            textBox.Width = graphics.FormatedLabel.Width;
            textBox.Height = graphics.FormatedLabel.Height;
            textBox.FontSize = 16;
            textBox.FontFamily = new FontFamily("Verdana");
            textBox.Text = graphics.Label;

            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;

            drawingCanvas.Children.Add(textBox);

            if (graphics is GraphicsEdge)
            {
                Canvas.SetLeft(textBox, ((GraphicsEdge)graphics).LabelPosition.X);
                Canvas.SetTop(textBox, ((GraphicsEdge)graphics).LabelPosition.Y);
            }
            else if(graphics is GraphicsVertex)
            {
                Canvas.SetLeft(textBox, ((GraphicsVertex)graphics).Center.X - graphics.FormatedLabel.Width / 2);
                Canvas.SetTop(textBox, ((GraphicsVertex)graphics).Center.Y - graphics.FormatedLabel.Height / 2);
            }
            
            textBox.Focus();
            textBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
            textBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(textBox_LostKeyboardFocus);
            textBox.PreviewKeyDown += new KeyEventHandler(textBox_PreviewKeyDown);
            textBox.KeyDown += new KeyEventHandler(textBox_KeyDown);
            textBox.ContextMenu = null;
            // Initially textbox is set to the same rectangle as graphicsText.
            // After textbox loading its template is available, and we can
            // correct textbox position - see details in the textBox_Loaded function.
            textBox.Loaded += new RoutedEventHandler(textBox_Loaded);
        }


        /// <summary>
        /// Correct textbox position.
        /// Without this correction text shown in a textbox appears with some
        /// horizontal and vertical offset relatively to textbox bounding rectangle.
        /// We need to apply this correction, moving textbox in left-up direction.
        /// 
        /// Visually, text should not "jump" on the screen, when in-place editing
        /// textbox is open and closed.
        /// </summary>
        void textBox_Loaded(object sender, RoutedEventArgs e)
        {
            double xOffset, yOffset;

            ComputeTextOffset(textBox, out xOffset, out yOffset);

            Canvas.SetLeft(textBox, Canvas.GetLeft(textBox) - xOffset);
            Canvas.SetTop(textBox, Canvas.GetTop(textBox) - yOffset);
            textBox.Width = textBox.Width + xOffset + xOffset;
            textBox.Height = textBox.Height + yOffset + yOffset;
        }


        /// <summary>
        /// Compute distance between textbox top-left point and actual
        /// text top-left point inside of textbox.
        /// 
        /// Thanks to Nick Holmes for showing this code in MSDN WPF Forum.
        /// </summary>
        static void ComputeTextOffset(TextBox tb, out double xOffset, out double yOffset)
        {
            // Set hard-coded values initially
            xOffset = 5;
            yOffset = 3;

            try
            {
                ContentControl cc = (ContentControl)tb.Template.FindName("PART_ContentHost", tb);
                GeneralTransform tf = ((Visual)cc.Content).TransformToAncestor(tb);
                Point offset = tf.Transform(new Point(0, 0));
                xOffset = offset.X;
                yOffset = offset.Y;
            }
            catch(Exception){}

        }

        void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                textBox.Text = oldText;
                drawingCanvas.HideTextbox(cur_graphics);
                e.Handled = true;
                return;
            }

            // Enter without modifiers - Shift+Enter should be available.
            if (e.Key == Key.Return && Keyboard.Modifiers == ModifierKeys.None)
            {
                drawingCanvas.HideTextbox(cur_graphics);
                e.Handled = true;
                return;
            }
        }
        void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (cur_graphics.Label.Length < textBox.Text.Length)
            {
                double xOffset, yOffset;
                ComputeTextOffset(textBox, out xOffset, out yOffset);
                Canvas.SetLeft(textBox, Canvas.GetLeft(textBox) - xOffset);
                textBox.Width = textBox.Width + xOffset + xOffset;
            }
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            drawingCanvas.HideTextbox(cur_graphics);
        }

        void textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            drawingCanvas.HideTextbox(cur_graphics);
        }

        public string OldText
        {
            get { return oldText; }
        }
    }
}
