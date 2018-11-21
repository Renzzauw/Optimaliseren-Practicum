using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Enumerator
	enum CollectionCounts { Once, Twice };

	// int met bitshiften per dag 0 of 1 langsgeweest
	enum CollectionPattern { Once, Twice };

	// Company object
	public class Company
	{
		// Variables
		public int companyIndex;				// Identifier for this company
		public int xCoordinate, yCoordinate;    // Position of the company
		public int visitDays = 0;               // Binary representation of the days the company has been visited 
		public byte visitCount = 0;				// Amount of times the company has been visited
		public string placeName;				// Name of the place where this company is
		public Order[] orders;                  // The list of orders of the company

		// Constructor
		public Company(int companyIndex, int xCoordinate, int yCoordinate, string placeName, Order[] orders)
		{
			this.companyIndex = companyIndex;
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
			this.placeName = placeName;
			this.orders = orders;
		}

		// When visiting the company, set the bit of this day to 1
		// dayIndex: monday = 0, tuesday = 1, ...
		public void VisitCompany(int dayIndex)
		{
			// Shift dayIndex to the right position
			int bit = 16 >> dayIndex;
			// Perform OR operation to set the bit to true
			visitDays = visitDays | bit;
			// Increment visit counter
			visitCount++;
		}
	}
}
