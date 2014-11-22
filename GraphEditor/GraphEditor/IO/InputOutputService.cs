using System;
using System.IO;
using System.Xml.Serialization;
using GraphEditor.PropertiesClasses;

namespace GraphEditor.IO
{
	public static class InputOutputService
	{
		public static void Save(string fileName, GraphCanvas canvas)
		{
			try
			{
				SerializationHelper helper = new SerializationHelper(canvas.GraphicsList, canvas.GraphStructure);

				XmlSerializer xml = new XmlSerializer(typeof(SerializationHelper));

				using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					xml.Serialize(stream, helper);
					canvas.ClearHistory();
					canvas.UpdateState();
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		/// <summary>
		/// Load graphics from XML file.
		/// Throws: DrawingCanvasException.
		/// </summary>
		public static void Load(string fileName, GraphCanvas canvas)
		{
			try
			{
				SerializationHelper helper;
				XmlSerializer xml = new XmlSerializer(typeof(SerializationHelper));

				using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					helper = (SerializationHelper)xml.Deserialize(stream);
				}

				if (helper.Graphics == null)
					throw new Exception("Empty Graphics List");
				if (helper.Elements == null)
					throw new Exception("Empty Elements List");

				canvas.GraphicsList.Clear();
				canvas.GraphStructure.Clear();

				foreach (PropertiesGraphicsBase g in helper.Graphics)
				{
					canvas.GraphicsList.Add(g.CreateGraphics());
				}
				foreach (PropertiesGraphBase g in helper.Elements)
				{
					canvas.GraphStructure.AddElement(g);
				}
				// Update clip for all loaded objects.
				canvas.RefreshClip();

				canvas.ClearHistory();
				canvas.UpdateState();
				canvas.ReDraw();
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
