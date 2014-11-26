using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphEditor.GraphStruct;

namespace GraphEditor.Algorithms
{
	public class AlgorithmResult
	{
		public double Cost { get; set; }

		public List<GraphElementBase> ItemsToSelect { get; set; }

		public AlgorithmResult()
		{
			ItemsToSelect = new List<GraphElementBase>();
		}

		public AlgorithmResult(double cost, List<GraphElementBase> itemsToSelect)
		{
			Cost = cost;
			ItemsToSelect = itemsToSelect;
		}
	}
}
