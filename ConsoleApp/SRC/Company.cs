using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Enumerator
	enum CollectionCounts { Never, Once, Twice };

	// Company object
	public class Company
	{
		// Variables
		public int companyIndex;				// Identifier for this company
		public int xCoordinate, yCoordinate;    // Position of the company
		public string placeName;				// Name of the place where this company is
		public List<Order> orders;              // The list of orders of the company

		// Constructor
		public Company(int companyIndex, int xCoordinate, int yCoordinate, string placeName, List<Order> orders)
		{
			this.companyIndex = companyIndex;
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
			this.placeName = placeName;
			this.orders = orders;
		}




	}
}
