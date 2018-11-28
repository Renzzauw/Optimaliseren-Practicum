using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Order object
	public class Order
	{
		// Variables (these are the values given as input)
		public int orderNumber, frequency, containerCount, volumePerContainer, matrixID, xCoordinate, yCoordinate;
		public string placeName;
		public float emptyingTime;
		public bool ordersDone = false;

		// Constructor
		public Order(int orderNumber, string placeName, int frequency, int containerCount, int volumePerContainer, float emptyingTime, int matrixID, int xCoordinate, int yCoordinate)
		{
			this.orderNumber = orderNumber;
			this.placeName = placeName;
			this.frequency = frequency;
			this.containerCount = containerCount;
			this.volumePerContainer = volumePerContainer;
			this.emptyingTime = emptyingTime;
			this.matrixID = matrixID;
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
		}


	}
}
