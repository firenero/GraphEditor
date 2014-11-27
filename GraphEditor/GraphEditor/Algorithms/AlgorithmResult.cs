using System.Collections.Generic;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	public class AlgorithmResult
	{
		public AlgorithmResult()
		{
			ItemsToSelect = new List<GraphElementBase>();
		}

		public AlgorithmResult(double cost, List<GraphElementBase> itemsToSelect)
		{
			Cost = cost;
			ItemsToSelect = itemsToSelect;
		}

		public double Cost { get; set; }

		public List<GraphElementBase> ItemsToSelect { get; set; }
	}
}