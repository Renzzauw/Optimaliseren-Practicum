namespace OptimaliserenPracticum
{
	// Garbage truck object
	public class GarbageTruck
	{
		// Variables
		private const int capacity = 100000;	// Capacity of the truck in liters, times 5 because in reality garbage gets compressed
		private int currentCapacity = 0;     // Current content of the truck in liters

		// Constructor is not needed

		// Function that handles the emptying of the garbage truck
		public GarbageTruck EmptyTruck()
		{
			// Set the current capacity of the truck to zero, then return it
			currentCapacity = 0;
            return this;
		}

        // Function that handles the filling of the garbage truck
        public GarbageTruck FillTruck(Order ord)
        {
            // Fill the truck by the amount of garbage at a given order
            int cap = ord.containerCount * ord.volumePerContainer;
            currentCapacity += cap;
            return this;
        }

        // Function that returns whether the truck is nearly full
        public bool CheckIfFull()
        {
            return capacity * 0.9 < currentCapacity;
        }

        // Function that returns whether the truck has more garbage than its capacity
        public bool CheckIfOverloaded()
        {
            return currentCapacity > capacity;
        }

        // Function that returns whether the truck is empty
        public bool CheckIfEmpty()
        {
            return currentCapacity == 0;
        }

    }
}
