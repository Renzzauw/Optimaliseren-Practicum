using System.Collections.Generic;
using System;

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

        // Function that changes the best state found, if the new rating is better than the old one
        public static void NewBest(State state, double rating)
        {
            if (rating < bestRating)
            {
                bestState = state;
                bestRating = rating;
                timeSinceNewBest = 0;
            }
            timeSinceNewBest++;
        }
        // Calcuate the evaluation value of a day, and return the new value
        public static double CalcDayEval(double time, GarbageTruck truck)
        {
            double eval = 0;
            eval += time;
            // Check if there is any overtime, deduct for that
            if (time > dayEnd)
            {
                eval += (time - dayEnd) * 10;
            }
            // Check if the truck is overloaded, deduct score dependant of the amount that is overloaded
            if (truck.CheckIfOverloaded()) eval += truck.AmountOverloaded() * 5;
            return eval / 60;
        }
    }

    public class Eval
    {
        public double value;
        public double time;
        public GarbageTruck truck;

        public Eval(double v, double t, GarbageTruck l)
        {
            value = v;
            time = t;
            truck = l;
        }
    }
}
