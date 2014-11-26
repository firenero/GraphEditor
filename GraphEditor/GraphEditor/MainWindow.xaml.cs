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
using GraphEditor.IO;
using GraphEditor.Tools;
using Microsoft.Win32;

namespace GraphEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string currentFileName = "";

		public MainWindow()
		{
			InitializeComponent();
		}

		#region Buttons event handlers

		#region Graph Edit Buttons

		private void AddEdgeButton_OnChecked(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Tool = ToolType.Edge;
		}

		private void AddEdgeButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (DrawingGraphCanvas.Tool == ToolType.Edge)
				DrawingGraphCanvas.Tool = ToolType.Pointer;
		}

		private void AddVertexButton_OnChecked(object sender, RoutedEventArgs e)
		{
			DrawingGraphCanvas.Tool = ToolType.Ellipse;
		}

		private void AddVertexButton_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if (DrawingGraphCanvas.Tool == ToolType.Ellipse)
				DrawingGraphCanvas.Tool = ToolType.Pointer;
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

		#endregion

		#region IO Buttons

		private void NewFileButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (IsSaveAndContinue())
			{
				New();
			}
		}

		private void OpenFileButton_OnClick(object sender, RoutedEventArgs e)
		{
			if (IsSaveAndContinue())
			{
				Open();
			}
		}

		private void SaveButton_OnClick(object sender, RoutedEventArgs e)
		{
			Save();
		}

		private void SaveAsButton_OnClick(object sender, RoutedEventArgs e)
		{
			SaveAs();
		}

		#endregion

		#region Application Buttons

		private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void ExitButton_OnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		#endregion

		#endregion

		#region Methods

		private void New()
		{
			DrawingGraphCanvas.Clear();
			currentFileName = "";
		}

		/// <summary>
		/// Open new graph
		/// </summary>
		/// <returns>New file name if open success else old file name.</returns>>
		private void Open()
		{
			var openFileDialog = new OpenFileDialog()
			{
				Filter = "XML files (*.xml)|*.xml|All Files|*.*",
				DefaultExt = "xml",
				InitialDirectory = Environment.CurrentDirectory
			};
			if (openFileDialog.ShowDialog() != false)
			{
				try
				{
					InputOutputService.Load(openFileDialog.FileName, DrawingGraphCanvas);
					currentFileName = openFileDialog.FileName;
				}
				catch (Exception)
				{
					MessageBox.Show("Can't open selected file.", Title, MessageBoxButton.OK);
				}
			}
		}

		private void Save()
		{
			if (currentFileName == "")
			{
				SaveAs();
			}
			else
			{
				Save(currentFileName);
			}
		}

		private void SaveAs()
		{
			var saveFileDialog = new SaveFileDialog()
			{
				Filter = "XML files (*.xml)|*.xml|All Files|*.*",
				DefaultExt = "xml",
				InitialDirectory = Environment.CurrentDirectory
			};

			if (saveFileDialog.ShowDialog() != false)
			{
				Save(saveFileDialog.FileName);
			}
		}

		private void Save(string filename)
		{
			try
			{
				InputOutputService.Save(filename, DrawingGraphCanvas);
				currentFileName = filename;
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Saving error.", MessageBoxButton.OK);
			}
		}

		private MessageBoxResult ConfirmSave()
		{
			return MessageBox.Show("Do you want to save changes?", Title, MessageBoxButton.YesNoCancel);
		}

		private bool IsSaveAndContinue()
		{
			if (DrawingGraphCanvas.IsDirty)
			{
				switch (ConfirmSave())
				{
					case MessageBoxResult.Yes:
						Save();
						return true;
					case MessageBoxResult.Cancel:
						return false;
					default:
						return true;
				}
			}
			return true;
		}

		#endregion

	}
}
