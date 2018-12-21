using System.Collections.Generic;

namespace OptimaliserenPracticum
{
	// Global class for important data
	public static class DTS
	{
		public static int[,] distanceMatrix;          // Distance matrix
		public static int[,] timeMatrix;              // Time matrix
        public static int dayStart, dayEnd;           // The time when the day starts and end, in seconds
        public static int emptyingTime;               // The time that it takes to empty the truck in Maarheze, in seconds
        public static int maarheeze;                  // The matrixid where the dumping spot is located.
		public static Dictionary<int, Order> orders;  // A dictionary that contains all orders, with their ID as key
        public static List<int> availableOrders;      // A list containing all ID's of available orders
        public static float temperature;              // The temperature used in SA 
        public static State bestState;                // The best state ever found during SA, at a given moment
        public static double bestRating;              // The rating belonging to that state
        public static int timeSinceNewBest;           // The amount of iterations since the best state was changed last
        public static bool hasOvertime;               // Checks whether a given state has overtime. If so, it cannot be accepted as a best state

        // Function that changes the best state found, if the new rating is better than the old one
        public static void NewBest(State state, double rating)
        {
            // Right now we also replace of the new state is as good as the best one, so that the program doesn't terminate too early
            if (rating >= bestRating)
            {
                bestState = state;
                bestRating = rating;
                timeSinceNewBest = 0;
            }
            timeSinceNewBest++;
        }
	}
}
