using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
	public class Order
	{
		public int orderNumber, containterCount, volumePerContainer, matrixID, xCoordinate, yCoordinate;
		public string placeName;
		public CollectPattern collectPattern;
		public float emptyingTime;
		
		// Constructor
		public Order()
		{

		}
	}
}
