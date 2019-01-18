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
        //public static State bestState;                // The best state ever found during SA, at a given moment
        public static List<Status>[][] bestStatus;    // THe route of the best state ever found
        public static Eval[][] bestEvals;       // The evaluation of the best state ever found
        public static double orderScore;              // The amount of point given for not completed orders
        public static double bestOrderScore;              // The amount of point given for not completed orders, in the best state found
        public static double bestRating;              // The rating belonging to that state
        public static int timeSinceNewBest;           // The amount of iterations since the best state was changed last
        public static int truckCapacity;              // The capacity of a garbage truck

        // Function that changes the best state found, if the new rating is better than the old one
        public static void NewBest(State state)
        {
            double evalRating = GetAllEval(state.evals);
            if (evalRating + orderScore < bestRating)
            {
                CopyStatus(state.status);
                CopyEval(state.evals);
                bestRating = evalRating + orderScore;
                bestOrderScore = orderScore;
                Console.WriteLine("time " + evalRating);
                Console.WriteLine("order " + bestOrderScore);
                Console.WriteLine("total " + bestRating);
                timeSinceNewBest = 0;
            }
            timeSinceNewBest++;
        }
        // Helper function to copy the Status jagged array
        public static void CopyStatus(List<Status>[][] old)
        {
            bestStatus = new List<Status>[2][];
            for(int i = 0; i < bestStatus.Length; i++)
            {
                bestStatus[i] = new List<Status>[5];
                for(int j = 0; j < bestStatus[i].Length; j++)
                {
                    bestStatus[i][j] = new List<Status>();
                    for(int k = 0; k < old[i][j].Count; k++)
                    {
                        bestStatus[i][j].Add(new Status(old[i][j][k].day, old[i][j][k].ordid, old[i][j][k].ordnr)); // dit gaat denk ik niet zo rap
                    }
                }
            }
        }
        // Helper function to copy the Eval jagged array
        public static void CopyEval(Eval[][] old)
        {
            bestEvals = new Eval[2][];
            for (int i = 0; i < bestEvals.Length; i++)
            {
                bestEvals[i] = new Eval[5];
                for (int j = 0; j < bestEvals[i].Length; j++)
                {
                    bestEvals[i][j] = new Eval(old[i][j].value, old[i][j].time);
                }
            }
        }
        // Calcuate the evaluation value of a day, and return the new value
        public static double CalcDayEval(double time, int load1, int load2)
        {
            double eval = 0;
            eval += time;
            // Check if there is any overtime, deduct for that
            if (time > dayEnd)
            {
                eval += (time - dayEnd) * 50;
            }
            // Check if the truck is overloaded on either route, deduct score dependent of the amount that is overloaded
            if (load1 > truckCapacity) eval += (load1 - truckCapacity) * 5;
            if (load2 > truckCapacity) eval += (load2 - truckCapacity) * 5;
            return eval / 60;
        }

        // Return the saved evaluation values of every day
        public static double GetAllEval(Eval[][] evals)
        {
            double acc = 0;
            for (ushort i = 0; i < 2; i++)
            {
                for (ushort j = 0; j < 5; j++)
                {
                    acc += evals[i][j].value;
                }
            }
            return acc;
        }
    }



    public class Eval
    {
        public double value;
        public double time;

        public Eval(double v, double t)
        {
            value = v;
            time = t;
        }
    }
}
