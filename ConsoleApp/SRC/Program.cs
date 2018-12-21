using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OptimaliserenPracticum
{
	class Program
    {
        
        static void Main(string[] args)
        {
            SimulatedAnnealing SA = new SimulatedAnnealing();
            SA.Init();
        }
    }

	public class SimulatedAnnealing
	{
		// Variables      
		private int i;                                  // A counter that keeps track of the total amount of iterations
		private float alpha;                            // The percentage to reduce T with, every q iterations
		private int q;                                  // The number of iterations before q gets reduced
		private StateGenerator generator;               // An instance of the stateGenerator class, which will calcuate successor states of a given state

		// Initialize the program
		public void Init()
		{
            // Start the diagnostics
            Diagnostics.initWatch = new Stopwatch();
            Diagnostics.runtimeWatch = new Stopwatch();
			Diagnostics.initWatch.Start();
            Diagnostics.second = 1;
			// Load the order lists from the input file
			DTS.orders = new Dictionary<int, Order>();
			DTS.availableOrders = new List<int>();
            // Construct the distance and time matrix
			Initialization init = new Initialization();
			var adjacencyList = init.GetAdjacencyList();
			DTS.distanceMatrix = adjacencyList.Item1;
			DTS.timeMatrix = adjacencyList.Item2;
            // Initialize all other DTS variables
            DTS.dayStart = 0;
            DTS.dayEnd = 43200;
            DTS.emptyingTime = 1800;
            DTS.timeSinceNewBest = 0;
            DTS.temperature = 100;
            DTS.maarheeze = 287;
            // initialize orders
            DTS.companyList = init.MakeCompanies();
			// Initialize all other variables
			i = 0;
			alpha = 0.99F;
			q = 10000; // q is hardcoded for now, we did not have the time to make it dependent on the amount of neighbourstates
            // Make an initial state, and set the best state ever to this state
			State initial = new State();
            DTS.bestState = initial;
            // Initialize the StateGenerator class, that is used to make successor states
            generator = new StateGenerator(initial);
            // Stop the stopwatch and see how long the initialization took
            Diagnostics.initWatch.Stop();
			Console.WriteLine("Initializationtime: " + Diagnostics.initWatch.ElapsedMilliseconds + " ms");
            // Run the Simulated Annealing
			Run(initial);

		}
        // Run the Simulated Annealing with the given initial state
		public void Run(State initialState)
		{
            // Start a stopwatch to see how long the annealing takes
            Diagnostics.runtimeWatch.Start();
			State current = initialState;
            // Keep iterating untill the best state ever found, has not been improved (or matched) in a while
			while (DTS.timeSinceNewBest < DTS.temperature * 1000)
			{
                // Keep track of how many iterations happen each second, and print that amount each second
                Diagnostics.IterationsPerSecond++;
                if (Diagnostics.runtimeWatch.ElapsedMilliseconds > 1000 * Diagnostics.second)
                {
                    Console.WriteLine("Number of iterations in second: " + Diagnostics.second + " equals: " + Diagnostics.IterationsPerSecond);
                    Diagnostics.IterationsPerSecond = 0;
                    Diagnostics.second++;
                }
                // Every q iterations, lower the temperature by alpha
				if (i % q == 0)
				{
					DTS.temperature *= alpha;
				}
                // Get a successor via the StateGenerator class
				current = generator.GetNextState(current);
                i++;

            }
            // Write the best state ever found to a file, and print it to the console
            FileHandler.SaveState(DTS.bestState);
            FileHandler.Print(DTS.bestState);
            // Print the amount of time that was spent iterating
            Diagnostics.runtimeWatch.Stop();
            Console.WriteLine("Runtime: " + Diagnostics.runtimeWatch.ElapsedMilliseconds + " ms");
            // Do a readkey at the end so that the console does not close after printing a solution
            Console.ReadKey();
		}
	}



}
