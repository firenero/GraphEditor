using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Graphics;
using GraphEditor.Properties;

namespace GraphEditor.Tools
{
	/// <summary>
	///     Text tool
	/// </summary>
	internal class ToolText : ToolRectangleBase
	{
		private GraphicsBase curGraphics;
		private GraphCanvas drawingCanvas;
		private string oldText;
		private TextBox textBox;

		public ToolText(GraphCanvas drawingCanvas)
		{
			this.drawingCanvas = drawingCanvas;
			var stream = new MemoryStream(Resources.Text);
			ToolCursor = new Cursor(stream);
		}

		/// <summary>
		///     Textbox is exposed for DrawingCanvas Visual Children Overrides.
		///     If it is not null, overrides should include this textbox.
		/// </summary>
		public TextBox TextBox
		{
			get { return textBox; }
			set { textBox = value; }
		}

		public string OldText
		{
			get { return oldText; }
		}

		public override void OnMouseDown(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			var point = e.GetPosition(drawingCanvas);
			curGraphics = null;

			for (int i = drawingCanvas.GraphicsList.Count - 1; i >= 0; i--)
			{
				var o = drawingCanvas[i];
				if (o.MakeHitTest(point) == 0)
				{
					curGraphics = o;
					HelperFunctions.UnselectAll(drawingCanvas);
					curGraphics.IsSelected = true;
					break;
				}
			}

			if (curGraphics == null)
			{
				HelperFunctions.UnselectAll(drawingCanvas);
			}
			drawingCanvas.CaptureMouse();
		}

		public override void OnMouseUp(GraphCanvas drawingCanvas, MouseButtonEventArgs e)
		{
			drawingCanvas.ReleaseMouseCapture();
			if (curGraphics != null && textBox == null)
				CreateTextBox(curGraphics, drawingCanvas);
		}

		/// <summary>
		///     Create textbox for in-place editing
		/// </summary>
		public void CreateTextBox(GraphicsBase graphics, GraphCanvas drawingCanvas)
		{
			// Set graphics if not set
			if (!ReferenceEquals(curGraphics, graphics))
			{
				curGraphics = graphics;
			}

			oldText = graphics.Label;
			textBox = new TextBox
			          {
				          Width = graphics.FormatedLabel.Width,
				          Height = graphics.FormatedLabel.Height,
				          FontSize = 16,
				          FontFamily = new FontFamily("Verdana"),
				          Text = graphics.Label,
				          AcceptsReturn = true,
				          TextWrapping = TextWrapping.Wrap
			          };

			drawingCanvas.Children.Add(textBox);

			if (graphics is GraphicsEdge)
			{
				Canvas.SetLeft(textBox, (graphics as GraphicsEdge).LabelPosition.X);
				Canvas.SetTop(textBox, (graphics as GraphicsEdge).LabelPosition.Y);
			}
			else if (graphics is GraphicsVertex)
			{
				Canvas.SetLeft(textBox, (graphics as GraphicsVertex).Center.X - graphics.FormatedLabel.Width / 2);
				Canvas.SetTop(textBox, (graphics as GraphicsVertex).Center.Y - graphics.FormatedLabel.Height / 2);
			}

			textBox.Focus();
			textBox.LostFocus += textBox_LostFocus;
			textBox.LostKeyboardFocus += textBox_LostKeyboardFocus;
			textBox.PreviewKeyDown += textBox_PreviewKeyDown;
			textBox.KeyDown += textBox_KeyDown;
			textBox.ContextMenu = null;
			// Initially textbox is set to the same rectangle as graphicsText.
			// After textbox loading its template is available, and we can
			// correct textbox position - see details in the textBox_Loaded function.
			textBox.Loaded += textBox_Loaded;
		}


		/// <summary>
		///     Correct textbox position.
		///     Without this correction text shown in a textbox appears with some
		///     horizontal and vertical offset relatively to textbox bounding rectangle.
		///     We need to apply this correction, moving textbox in left-up direction.
		///     Visually, text should not "jump" on the screen, when in-place editing
		///     textbox is open and closed.
		/// </summary>
		private void textBox_Loaded(object sender, RoutedEventArgs e)
		{
			double xOffset, yOffset;

			ComputeTextOffset(textBox, out xOffset, out yOffset);

			Canvas.SetLeft(textBox, Canvas.GetLeft(textBox) - xOffset);
			Canvas.SetTop(textBox, Canvas.GetTop(textBox) - yOffset);
			textBox.Width = textBox.Width + xOffset + xOffset;
			textBox.Height = textBox.Height + yOffset + yOffset;
		}


		/// <summary>
		///     Compute distance between textbox top-left point and actual
		///     text top-left point inside of textbox.
		///     Thanks to Nick Holmes for showing this code in MSDN WPF Forum.
		/// </summary>
		private static void ComputeTextOffset(TextBox tb, out double xOffset, out double yOffset)
		{
			// Set hard-coded values initially
			xOffset = 5;
			yOffset = 3;

			try
			{
				var cc = (ContentControl) tb.Template.FindName("PART_ContentHost", tb);
				var tf = ((Visual) cc.Content).TransformToAncestor(tb);
				var offset = tf.Transform(new Point(0, 0));
				xOffset = offset.X;
				yOffset = offset.Y;
			}
			catch (Exception)
			{
			}
		}

		private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				textBox.Text = oldText;
				drawingCanvas.HideTextbox(curGraphics);
				e.Handled = true;
				return;
			}

			// Enter without modifiers - Shift+Enter should be available.
			if (e.Key == Key.Return && Keyboard.Modifiers == ModifierKeys.None)
			{
				drawingCanvas.HideTextbox(curGraphics);
				e.Handled = true;
			}
		}

		private void textBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (curGraphics.Label.Length < textBox.Text.Length)
			{
				double xOffset, yOffset;
				ComputeTextOffset(textBox, out xOffset, out yOffset);
				Canvas.SetLeft(textBox, Canvas.GetLeft(textBox) - xOffset);
				textBox.Width = textBox.Width + xOffset + xOffset;
			}
		}

		private void textBox_LostFocus(object sender, RoutedEventArgs e)
		{
			drawingCanvas.HideTextbox(curGraphics);
		}

		private void textBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			//drawingCanvas.HideTextbox(cur_graphics);
		}
	}
}