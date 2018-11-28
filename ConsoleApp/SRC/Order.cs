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
		public int visitDays = 0;               // Binary representation of the days the company has been visited 
		public byte visitCount = 0;             // Amount of times the company has been visited

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

		// When visiting the company, set the bit of this day to 1
		// dayIndex: monday = 0, tuesday = 1, ..., friday = 4
		public void VisitCompany(int dayIndex)
		{
			// Shift dayIndex to the right position (shift fully to the left first, then to the right by the dayIndex)
			int bit = 16 >> dayIndex;
			// Perform OR operation to set the bit to true
			visitDays = visitDays | bit;
			// Increment visit counter
			visitCount++;
		}
	}
}
