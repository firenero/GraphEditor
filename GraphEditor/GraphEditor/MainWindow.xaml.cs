using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphEditor.Tools;

namespace GraphEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void AddVertexButton_OnClick(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Tool = ToolType.Ellipse;
		}

		private void AddEdgeButton_OnClick(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Tool = ToolType.Edge;
		}

		private void RemoveVertexOrNodeButton_OnClick(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Delete();
		}

		private void UndoButton_OnClick(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Undo();
		}

		private void RedoButton_OnClick(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Redo();
		}

		private void OrientedGraphToggleButton_OnChecked(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.IsOrientedGraph = true;
		}

		private void OrientedGraphToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.IsOrientedGraph = false;
		}
	}
}
