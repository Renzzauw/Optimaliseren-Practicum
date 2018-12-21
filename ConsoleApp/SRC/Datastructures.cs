using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Global class for important data
	public static class DTS
	{
		public static Company[] companyList;        // List of companies
		public static int[,] distanceMatrix;        // Distance matrix
		public static int[,] timeMatrix;            // Time matrix
        public static int dayStart, dayEnd;
        public static int emptyingTime;
		public static Company maarheeze;
		public static Dictionary<int, Order> orders;
        public static List<int> availableOrders;
        public static float temperature;
        public static State bestState;
        public static int bestRating;
        public static int timeSinceNewBest;

        public static void NewBest(State state, int rating)
        {
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
