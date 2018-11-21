using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Garbage truck object
	public class GarbageTruck
	{
		// Variables
		public const int capacity = 20000;	// Capacity of the truck in liters
		public const int emptyTime = 30;    // Time to empty garbage truck in minutes
		public int currentCapacity;         // Current content of the truck in liters
		public Company currentCompany;		// Company where the truck currently is

		// Constructor
		public GarbageTruck(int currentCapacity = 0)
		{
			this.currentCapacity = currentCapacity;
		}

		// Function that handles the emptying of the garbage truck
		public void EmptyTruck()
		{
			// TODO: iets met timer vooruitzetten?
			currentCapacity = 0;
		}

		// Travel from the current company to the next company
		public void TravelToNextCompany(Company nextCompany)
		{
			// TODO: operator == maken voor Company class, denk dat zo vergelijken nu niet werkt

			// Do not travel if the next company is the current company
			if (nextCompany.companyIndex == currentCompany.companyIndex)
				return;


		}
	}
}
