using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphEditor.Commands;
using GraphEditor.Graphics;
using GraphEditor.GraphStruct;
using GraphEditor.Tools;

namespace GraphEditor
{
	public class GraphCanvas : Canvas
	{
		public static readonly DependencyProperty TrackerProperty;
		public static readonly DependencyProperty ToolProperty;
		public static readonly DependencyProperty LineWidthProperty;
		public static readonly DependencyProperty ObjectColorProperty;
		public static readonly DependencyProperty SelectedColorProperty;
		public static readonly DependencyProperty TextColorProperty;
		public static readonly DependencyProperty ActualScaleProperty;
		public static readonly DependencyProperty IsDirtyProperty;
		public static readonly DependencyProperty CanUndoProperty;
		public static readonly DependencyProperty CanRedoProperty;
		public static readonly DependencyProperty CanSelectAllProperty;
		public static readonly DependencyProperty CanUnselectAllProperty;
		public static readonly DependencyProperty CanDeleteProperty;
		public static readonly DependencyProperty CanDeleteAllProperty;
		public static readonly DependencyProperty OrientedGraphProperty;
		public static readonly RoutedEvent IsDirtyChangedEvent;
		private GraphStruct.GraphStruct graphStructure;
		private VisualCollection graphicsList;

		private ToolPointer toolPointer;
		private ToolText toolText;
		private Tool[] tools;
		private UndoManager undoManager;

		static GraphCanvas()
		{
			// **********************************************************
			// Create dependency properties
			// **********************************************************

			// Tracker
			var metaData = new PropertyMetadata(false);

			TrackerProperty = DependencyProperty.Register(
			                                              "Tracker", typeof (bool), typeof (GraphCanvas),
			                                              metaData);

			// Tool
			metaData = new PropertyMetadata(ToolType.Pointer);

			ToolProperty = DependencyProperty.Register(
			                                           "Tool", typeof (ToolType), typeof (GraphCanvas),
			                                           metaData);


			// ActualScale
			metaData = new PropertyMetadata(
				1.0, // default value
				ActualScaleChanged); // change callback

			ActualScaleProperty = DependencyProperty.Register(
			                                                  "ActualScale", typeof (double), typeof (GraphCanvas),
			                                                  metaData);

			// IsDirty
			metaData = new PropertyMetadata(false);

			IsDirtyProperty = DependencyProperty.Register(
			                                              "IsDirty", typeof (bool), typeof (GraphCanvas),
			                                              metaData);

			// LineWidth
			metaData = new PropertyMetadata(
				1.0,
				LineWidthChanged);

			LineWidthProperty = DependencyProperty.Register(
			                                                "LineWidth", typeof (double), typeof (GraphCanvas),
			                                                metaData);

			// ObjectColor
			metaData = new PropertyMetadata(
				Colors.Black,
				ObjectColorChanged);

			ObjectColorProperty = DependencyProperty.Register(
			                                                  "ObjectColor", typeof (Color), typeof (GraphCanvas),
			                                                  metaData);

			// SelectedColor
			metaData = new PropertyMetadata(
				Colors.Red,
				SelectedColorChanged);

			SelectedColorProperty = DependencyProperty.Register(
			                                                    "SelectedColor", typeof (Color), typeof (GraphCanvas),
			                                                    metaData);

			// TextColor
			metaData = new PropertyMetadata(
				Colors.Black,
				TextColorChanged);

			TextColorProperty = DependencyProperty.Register(
			                                                "TextColor", typeof (Color), typeof (GraphCanvas),
			                                                metaData);


			metaData = new PropertyMetadata(false);
			OrientedGraphProperty = DependencyProperty.Register(
			                                                    "IsOrientedGraph", typeof (bool), typeof (GraphCanvas),
			                                                    metaData);
			/*
            // TextFontFamilyName
            metaData = new PropertyMetadata(
                Properties.Settings.Default.DefaultFontFamily,
                new PropertyChangedCallback(TextFontFamilyNameChanged));

            TextFontFamilyNameProperty = DependencyProperty.Register(
                "TextFontFamilyName", typeof(string), typeof(DrawingCanvas),
                metaData);

            // TextFontStyle
            metaData = new PropertyMetadata(
                FontStyles.Normal,
                new PropertyChangedCallback(TextFontStyleChanged));

            TextFontStyleProperty = DependencyProperty.Register(
                "TextFontStyle", typeof(FontStyle), typeof(DrawingCanvas),
                metaData);

            // TextFontWeight
            metaData = new PropertyMetadata(
                FontWeights.Normal,
                new PropertyChangedCallback(TextFontWeightChanged));

            TextFontWeightProperty = DependencyProperty.Register(
                "TextFontWeight", typeof(FontWeight), typeof(DrawingCanvas),
                metaData);

            // TextFontStretch
            metaData = new PropertyMetadata(
                FontStretches.Normal,
                new PropertyChangedCallback(TextFontStretchChanged));

            TextFontStretchProperty = DependencyProperty.Register(
                "TextFontStretch", typeof(FontStretch), typeof(DrawingCanvas),
                metaData);

            // TextFontSize
            metaData = new PropertyMetadata(
                12.0,
                new PropertyChangedCallback(TextFontSizeChanged));

            TextFontSizeProperty = DependencyProperty.Register(
                "TextFontSize", typeof(double), typeof(DrawingCanvas),
                metaData);
            */
			// CanUndo
			metaData = new PropertyMetadata(false);

			CanUndoProperty = DependencyProperty.Register(
			                                              "CanUndo", typeof (bool), typeof (GraphCanvas),
			                                              metaData);

			// CanRedo
			metaData = new PropertyMetadata(false);

			CanRedoProperty = DependencyProperty.Register(
			                                              "CanRedo", typeof (bool), typeof (GraphCanvas),
			                                              metaData);

			// CanSelectAll
			metaData = new PropertyMetadata(false);

			CanSelectAllProperty = DependencyProperty.Register(
			                                                   "CanSelectAll", typeof (bool), typeof (GraphCanvas),
			                                                   metaData);

			// CanUnselectAll
			metaData = new PropertyMetadata(false);

			CanUnselectAllProperty = DependencyProperty.Register(
			                                                     "CanUnselectAll", typeof (bool), typeof (GraphCanvas),
			                                                     metaData);

			// CanDelete
			metaData = new PropertyMetadata(false);

			CanDeleteProperty = DependencyProperty.Register(
			                                                "CanDelete", typeof (bool), typeof (GraphCanvas),
			                                                metaData);

			// CanDeleteAll
			metaData = new PropertyMetadata(false);

			CanDeleteAllProperty = DependencyProperty.Register(
			                                                   "CanDeleteAll", typeof (bool), typeof (GraphCanvas),
			                                                   metaData);
			/*
            // CanMoveToFront
            metaData = new PropertyMetadata(false);

            CanMoveToFrontProperty = DependencyProperty.Register(
                "CanMoveToFront", typeof(bool), typeof(DrawingCanvas),
                metaData);

            // CanMoveToBack
            metaData = new PropertyMetadata(false);

            CanMoveToBackProperty = DependencyProperty.Register(
                "CanMoveToBack", typeof(bool), typeof(DrawingCanvas),
                metaData);

            // CanSetProperties
            metaData = new PropertyMetadata(false);

            CanSetPropertiesProperty = DependencyProperty.Register(
                "CanSetProperties", typeof(bool), typeof(DrawingCanvas),
                metaData);
            */
			// **********************************************************
			// Create routed events
			// **********************************************************

			// IsDirtyChanged

			IsDirtyChangedEvent = EventManager.RegisterRoutedEvent("IsDirtyChangedChanged",
			                                                       RoutingStrategy.Bubble, typeof (DependencyPropertyChangedEventHandler), typeof (GraphCanvas));
		}

		public GraphCanvas()
		{
			graphicsList = new VisualCollection(this);
			graphStructure = new GraphStruct.GraphStruct();
			//CreateContextMenu();

			// create array of drawing tools
			tools = new Tool[(int) ToolType.Max];

			toolPointer = new ToolPointer();
			tools[(int) ToolType.Pointer] = toolPointer;
			tools[(int) ToolType.Edge] = new ToolEdge();
			tools[(int) ToolType.Ellipse] = new ToolVertex();
			tools[(int) ToolType.Eraser] = new ToolEraser();
			toolText = new ToolText(this);
			tools[(int) ToolType.Text] = toolText;
			/*
            tools[(int)ToolType.Rectangle] = new ToolRectangle();
            
            tools[(int)ToolType.PolyLine] = new ToolPolyLine();

            toolText = new ToolText(this);
            tools[(int)ToolType.Text] = toolText;   // kept as class member for in-place editing
            */
			// Create undo manager
			undoManager = new UndoManager(this);
			undoManager.StateChanged += undoManager_StateChanged;


			FocusVisualStyle = null;

			Loaded += DrawingCanvas_Loaded;
			MouseDown += DrawingCanvas_MouseDown;
			MouseMove += DrawingCanvas_MouseMove;
			MouseUp += DrawingCanvas_MouseUp;
			KeyDown += DrawingCanvas_KeyDown;
			LostMouseCapture += DrawingCanvas_LostMouseCapture;
		}

		/// <summary>
		///     Currently active drawing tool
		/// </summary>
		public ToolType Tool
		{
			get { return (ToolType) GetValue(ToolProperty); }
			set
			{
				if ((int) value >= 0 && (int) value < (int) ToolType.Max)
				{
					SetValue(ToolProperty, value);

					// Set cursor immediately - important when tool is selected from the menu
					tools[(int) Tool].SetCursor(this);
				}
			}
		}

		public bool Tracker
		{
			get { return (bool) GetValue(TrackerProperty); }
			set
			{
				SetValue(TrackerProperty, value);
				foreach (var el in graphicsList)
					((GraphicsBase) el).IsTrackerOn = value;
			}
		}

		public bool IsOrientedGraph
		{
			get { return (bool) GetValue(OrientedGraphProperty); }
			set
			{
				SetValue(OrientedGraphProperty, value);
				foreach (var el in graphicsList)
					if (el is GraphicsEdge)
						(el as GraphicsEdge).IsOriented = value;
			}
		}

		/// <summary>
		///     Return true if Select All function is available
		/// </summary>
		public bool CanSelectAll
		{
			get { return (bool) GetValue(CanSelectAllProperty); }
			internal set { SetValue(CanSelectAllProperty, value); }
		}


		/// <summary>
		///     Return true if Unselect All function is available
		/// </summary>
		public bool CanUnselectAll
		{
			get { return (bool) GetValue(CanUnselectAllProperty); }
			internal set { SetValue(CanUnselectAllProperty, value); }
		}


		/// <summary>
		///     Return true if Delete function is available
		/// </summary>
		public bool CanDelete
		{
			get { return (bool) GetValue(CanDeleteProperty); }
			internal set { SetValue(CanDeleteProperty, value); }
		}

		/// <summary>
		///     Return true if Delete All function is available
		/// </summary>
		public bool CanDeleteAll
		{
			get { return (bool) GetValue(CanDeleteAllProperty); }
			internal set { SetValue(CanDeleteAllProperty, value); }
		}

		/// <summary>
		///     Color of new graphics object.
		///     Setting this property is also applied to current selection.
		/// </summary>
		public Color ObjectColor
		{
			get { return (Color) GetValue(ObjectColorProperty); }
			set { SetValue(ObjectColorProperty, value); }
		}

		public Color SelectedColor
		{
			get { return (Color) GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color) GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		/// <summary>
		///     Get number of selected graphic objects
		/// </summary>
		internal int SelectionCount
		{
			get { return graphicsList.Cast<GraphicsBase>().Count(g => g.IsSelected); }
		}

		/// <summary>
		///     Returns true if document is changed
		/// </summary>
		public bool IsDirty
		{
			get { return (bool) GetValue(IsDirtyProperty); }
			internal set
			{
				SetValue(IsDirtyProperty, value);

				// Raise IsDirtyChanged event.
				var newargs = new RoutedEventArgs(IsDirtyChangedEvent);
				RaiseEvent(newargs);
			}
		}

		/// <summary>
		///     Return True if Undo operation is possible
		/// </summary>
		public bool CanUndo
		{
			get { return (bool) GetValue(CanUndoProperty); }
			internal set { SetValue(CanUndoProperty, value); }
		}

		/// <summary>
		///     Return True if Redo operation is possible
		/// </summary>
		public bool CanRedo
		{
			get { return (bool) GetValue(CanRedoProperty); }
			internal set { SetValue(CanRedoProperty, value); }
		}

		public VisualCollection GraphicsList
		{
			get { return graphicsList; }
		}

		internal int Count
		{
			get { return graphicsList.Count; }
		}

		internal GraphicsBase this[int index]
		{
			get
			{
				if (index >= 0 && index < Count)
				{
					return (GraphicsBase) graphicsList[index];
				}

				return null;
			}
		}

		public double ActualScale
		{
			get { return (double) GetValue(ActualScaleProperty); }
			set { SetValue(ActualScaleProperty, value); }
		}

		internal IEnumerable<GraphicsBase> Selection
		{
			get { return from GraphicsBase o in graphicsList where o.IsSelected select o; }
		}

		public GraphStruct.GraphStruct GraphStructure
		{
			get { return graphStructure; }
		}

		/// <summary>
		///     Cancel currently executed operation:
		///     add new object or group selection.
		///     Called when mouse capture is lost or Esc is pressed.
		/// </summary>
		private void CancelCurrentOperation()
		{
			if (Tool == ToolType.Pointer)
			{
				if (graphicsList.Count > 0)
				{
					if (graphicsList[graphicsList.Count - 1] is GraphicsSelectionRectangle)
					{
						// Delete selection rectangle if it exists
						graphicsList.RemoveAt(graphicsList.Count - 1);
					}
					else
					{
						// Pointer tool moved or resized graphics object.
						// Add this action to the history
						toolPointer.AddChangeToHistory(this);
					}
				}
			}
			else if (Tool > ToolType.Pointer && Tool < ToolType.Max)
			{
				// Delete last graphics object which is currently drawn
				if (graphicsList.Count > 0)
				{
					graphicsList.RemoveAt(graphicsList.Count - 1);
				}
			}

			Tool = ToolType.Pointer;

			ReleaseMouseCapture();
			Cursor = HelperFunctions.DefaultCursor;
		}

		/// <summary>
		///     Callback function called when ActualScale dependency property is changed.
		/// </summary>
		private static void ActualScaleChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			var d = property as GraphCanvas;

			double scale = d.ActualScale;

			foreach (GraphicsBase b in d.GraphicsList)
			{
				b.ActualScale = scale;
			}
		}

		/// <summary>
		///     Mouse capture is lost
		/// </summary>
		private void DrawingCanvas_LostMouseCapture(object sender, MouseEventArgs e)
		{
			if (IsMouseCaptured)
			{
				CancelCurrentOperation();
				UpdateState();
			}
		}

		/// <summary>
		///     Handle keyboard input
		/// </summary>
		private void DrawingCanvas_KeyDown(object sender, KeyEventArgs e)
		{
			// Esc key stops currently active operation
			/*if (e.Key == Key.Escape)
            {
                if (this.IsMouseCaptured)
                {
                    CancelCurrentOperation();
                    UpdateState();
                }
            }*/
		}

		/// <summary>
		///     Callback function called when ObjectColor dependency property is changed
		/// </summary>
		private static void ObjectColorChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			var d = property as GraphCanvas;

			HelperFunctions.ApplyColor(d, d.ObjectColor, true);
		}

		private static void SelectedColorChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			var d = property as GraphCanvas;

			HelperFunctions.ApplyColor(d, d.SelectedColor, true);
		}

		private static void TextColorChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			var d = property as GraphCanvas;

			HelperFunctions.ApplyColor(d, d.SelectedColor, true);
		}

		/// <summary>
		///     Open in-place edit box if GraphicsText is clicked
		/// </summary>
		private void HandleDoubleClick(MouseButtonEventArgs e)
		{
			var point = e.GetPosition(this);

			// Enumerate all text objects
			for (int i = graphicsList.Count - 1; i >= 0; i--)
			{
				var t = graphicsList[i] as GraphicsBase;

				if (t != null)
				{
					if (t.Contains(point))
					{
						toolText.CreateTextBox(t, this);
						return;
					}
				}
			}
		}

		/// <summary>
		///     Update state of Can* dependency properties
		///     used for Edit commands.
		///     This function calls after any change in drawing canvas state,
		///     caused by user commands.
		///     Helps to keep client controls state up-to-date, in the case
		///     if Can* properties are used for binding.
		/// </summary>
		public void UpdateState()
		{
			bool hasObjects = (Count > 0);
			bool hasSelectedObjects = (SelectionCount > 0);

			CanSelectAll = hasObjects;
			CanUnselectAll = hasObjects;
			CanDelete = hasSelectedObjects;
			CanDeleteAll = hasObjects;
		}

		/// <summary>
		///     UndoManager state is changed.
		///     Refresh CanUndo, CanRedo and IsDirty properties.
		/// </summary>
		private void undoManager_StateChanged(object sender, EventArgs e)
		{
			CanUndo = undoManager.CanUndo;
			CanRedo = undoManager.CanRedo;

			// Set IsDirty only if it is actually changed.
			// Setting IsDirty raises event for client.
			if (undoManager.IsDirty != IsDirty)
			{
				IsDirty = undoManager.IsDirty;
			}
		}

		/// <summary>
		///     Initialization after control is loaded
		/// </summary>
		private void DrawingCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			Focusable = true; // to handle keyboard messages
		}

		/// <summary>
		///     Mouse down.
		///     Left button down event is passed to active tool.
		///     Right button down event is handled in this class.
		/// </summary>
		private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (tools[(int) Tool] == null)
			{
				return;
			}


			Focus();


			if (e.ChangedButton == MouseButton.Left)
			{
				if (e.ClickCount == 2)
				{
					HandleDoubleClick(e); // special case for GraphicsText
				}
				else
				{
					tools[(int) Tool].OnMouseDown(this, e);
				}

				UpdateState();
			}
			else if (e.ChangedButton == MouseButton.Right && Tool == ToolType.Edge)
			{
				tools[(int) Tool].OnMouseDown(this, e);
			}
		}

		/// <summary>
		///     Mouse move.
		///     Moving without button pressed or with left button pressed
		///     is passed to active tool.
		/// </summary>
		private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			if (tools[(int) Tool] == null)
			{
				return;
			}

			if (e.MiddleButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
			{
				tools[(int) Tool].OnMouseMove(this, e);

				UpdateState();
			}
		}

		/// <summary>
		///     Mouse up event.
		///     Left button up event is passed to active tool.
		/// </summary>
		private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (tools[(int) Tool] == null)
			{
				return;
			}


			if (e.ChangedButton == MouseButton.Left)
			{
				tools[(int) Tool].OnMouseUp(this, e);

				UpdateState();
			}
		}

		/// <summary>
		///     Undo
		/// </summary>
		public void Undo()
		{
			undoManager.Undo();
			HelperFunctions.OrgonizeGraphics(this);
			UpdateState();
			ReDraw();
		}

		/// <summary>
		///     Redo
		/// </summary>
		public void Redo()
		{
			undoManager.Redo();
			HelperFunctions.OrgonizeGraphics(this);
			UpdateState();
			ReDraw();
		}

		public void RefreshClip()
		{
			foreach (GraphicsBase b in graphicsList)
			{
				b.Clip = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));

				// Good chance to refresh actual scale
				b.ActualScale = ActualScale;
			}
		}

		/// <summary>
		///     Add command to history.
		/// </summary>
		internal void AddCommandToHistory(CommandBase command)
		{
			undoManager.AddCommandToHistory(command);
		}

		/// <summary>
		///     Draw all graphics objects to DrawingContext supplied by client.
		///     Can be used for printing or saving image together with graphics
		///     as single bitmap.
		///     withSelection = true - draw selected objects with tracker.
		/// </summary>
		public void Draw(DrawingContext drawingContext, bool withSelection = false)
		{
			bool oldSelection = false;

			foreach (GraphicsBase b in graphicsList)
			{
				if (!withSelection)
				{
					// Keep selection state and unselect
					oldSelection = b.IsSelected;
					b.IsSelected = false;
				}

				b.Draw(drawingContext);

				if (!withSelection)
				{
					// Restore selection state
					b.IsSelected = oldSelection;
				}
			}
		}

		public void Delete()
		{
			HelperFunctions.DeleteSelection(this);
			UpdateState();
		}

		public void SelectAll()
		{
			HelperFunctions.SelectAll(this);
			UpdateState();
		}

		public void UnselectAll()
		{
			HelperFunctions.UnselectAll(this);
			UpdateState();
		}

		public void SelectElements(IEnumerable<GraphElementBase> itemsToSelect)
		{
			HelperFunctions.SelectElements(this, itemsToSelect);
			UpdateState();
		}

		public void ReDraw()
		{
			foreach (GraphicsBase gr in GraphicsList)
			{
				if (gr is GraphicsVertex)
				{
					(gr as GraphicsVertex).SetSize(20, GraphStructure.GetVertex(gr.Id).Connections.Count, 5);
					HelperFunctions.SeclectConnections(this, gr, true);
				}
				else if (gr is GraphicsEdge)
					(gr as GraphicsEdge).IsOriented = IsOrientedGraph;
			}
		}

		public void ClearHistory()
		{
			undoManager.ClearHistory();
		}

		public void Clear()
		{
			graphicsList.Clear();
			graphStructure.Clear();
			ClearHistory();
			UpdateState();
		}

		#region Visual Children Overrides

		/// <summary>
		///     Get number of children: VisualCollection count.
		///     If in-place editing textbox is active, add 1.
		/// </summary>
		protected override int VisualChildrenCount
		{
			get
			{
				int n = graphicsList.Count;

				if (toolText.TextBox != null)
				{
					n++;
				}

				return n;
			}
		}

		internal void HideTextbox(GraphicsBase graphics)
		{
			if (toolText.TextBox == null)
			{
				return;
			}

			UnselectAll();
			graphics.IsSelected = true;

			if (toolText.TextBox.Text.Trim().Length == 0)
			{
				// Textbox is empty
			}
			else
			{
				if (!String.IsNullOrEmpty(toolText.OldText)) // existing text was edited
				{
					if (toolText.TextBox.Text.Trim() != toolText.OldText) // text was really changed
					{
						// Create command
						var command = new CommandChangeState(this);

						// Make change in the text object
						String tmp = toolText.TextBox.Text.Trim();
						var el = GraphStructure.GetElement(graphics.Id);
						if (graphics is GraphicsEdge)
						{
							try
							{
								((GraphElementEdge) el).Weight = Convert.ToDouble(tmp);
								graphics.Label = tmp;
							}
							catch (FormatException)
							{
								MessageBox.Show("Edge weight may be only real number", "Naming Error");
							}
							catch (OverflowException)
							{
								MessageBox.Show(String.Format("Edge weight must be less than {0}.", Double.MaxValue), "Naming Error");
							}
							catch (Exception)
							{
							}
						}
						else
						{
							graphics.Label = tmp;
							if (el is GraphElementVertex)
								(el as GraphElementVertex).Label = tmp;
						}
						graphics.RefreshDrawing();
						ReDraw();

						if (graphics is GraphicsVertex)
							HelperFunctions.SeclectConnections(this, graphics, true);
						// Keep state after change and add command to the history
						command.NewState(this);
						undoManager.AddCommandToHistory(command);
					}
				}
			}

			// Remove textbox and set it to null.
			Children.Remove(toolText.TextBox);
			toolText.TextBox = null;
			// This enables back all ApplicationCommands,
			// which are disabled while textbox is active.
			Focus();
		}

		/// <summary>
		///     Get visual child - one of GraphicsBase objects
		///     or in-place editing textbox, if it is active.
		/// </summary>
		protected override Visual GetVisualChild(int index)
		{
			if (index < 0 || index >= graphicsList.Count)
			{
				if (toolText.TextBox != null && index == graphicsList.Count)
				{
					return toolText.TextBox;
				}

				throw new ArgumentOutOfRangeException("index");
			}

			return graphicsList[index];
		}

		#endregion Visual Children Overrides

		#region LineWidth

		/// <summary>
		///     Line width of new graphics object.
		///     Setting this property is also applied to current selection.
		/// </summary>
		public double LineWidth
		{
			get { return (double) GetValue(LineWidthProperty); }
			set { SetValue(LineWidthProperty, value); }
		}

		/// <summary>
		///     Callback function called when LineWidth dependency property is changed
		/// </summary>
		private static void LineWidthChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
		{
			var d = property as GraphCanvas;

			HelperFunctions.ApplyLineWidth(d, d.LineWidth, true);
		}

		#endregion LineWidth
	}
}