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
		public int truckNumber;				// Number of the truck (1 or 2)
		public const int capacity = 100000;	// Capacity of the truck in liters, times 5 because in reality garbage gets compressed
		public const int emptyTime = 30;    // Time to empty garbage truck in minutes
		public int currentCapacity = 0;     // Current content of the truck in liters
		public Company currentCompany;      // Company where the truck currently is

		// Constructor
		public GarbageTruck(int truckNumber, int currentCapacity)
		{
			this.truckNumber = truckNumber;
			this.currentCapacity = currentCapacity;
		}

		// Function that handles the emptying of the garbage truck
		public GarbageTruck EmptyTruck()
		{
			// TODO: iets met timer vooruitzetten?
			currentCapacity = 0;
            return this;
		}
        // Function that handles the filling of the garbage truck
        public GarbageTruck FillTruck(Order ord)
        {
            int cap = ord.containerCount * ord.volumePerContainer;
            currentCapacity += cap;
            return this;
        }
        public bool CheckIfFull()
        {
            return capacity * 0.9 < currentCapacity;
        }

        public bool CheckIfOverloaded()
        {
            return currentCapacity > capacity;
        }

        public bool CheckIfEmpty()
        {
            return currentCapacity == 0;
        }

        // Travel from the current company to the next company
        public void TravelToCompany(Company company)
		{
			// Do not travel if the next company is the current company
			if (company.companyIndex == currentCompany.companyIndex) return;

			// Iets met tijd 

		}



	}
}
