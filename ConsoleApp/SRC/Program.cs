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
            var distTime = init.GetDistTimeMatrix();
            DTS.distanceMatrix = distTime.Item1;
            DTS.timeMatrix = distTime.Item2;
            // Initialize all other DTS variables
            DTS.dayStart = 0;
            DTS.dayEnd = 43200;
            DTS.emptyingTime = 1800;
            DTS.timeSinceNewBest = 0;
            DTS.temperature = 25;
            DTS.maarheeze = 287;
            DTS.bestRating = float.MaxValue;
            DTS.truckCapacity = 100000;
            // initialize orders
            init.MakeOrders();
            // Initialize all other variables
            i = 0;
            alpha = 0.99F;
            q = 1000000; // q is hardcoded for now, we did not have the time to make it dependent on the amount of neighbourstates
            // Make an initial state, and set the best state ever to this state
            Console.WriteLine("Type 1 if you want to load a state, or another number if you want to create a new random state");
            string inputstring = Console.ReadLine();
            int value = int.Parse(inputstring);
            State initial = null;
            if (value == 1)
            {
                Console.WriteLine("Type the name of your file!");
                string filename = Console.ReadLine();
                initial = init.LoadState(filename);
            }
            else
            {
                initial = new State();
            }
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
			while (Diagnostics.runtimeWatch.ElapsedMilliseconds < 1000 * 30)
			{
                // Keep track of how many iterations happen each second, and print that amount each second
                Diagnostics.AcceptationsPerSecond++;
                if (Diagnostics.runtimeWatch.ElapsedMilliseconds > 1000 * Diagnostics.second)
                {
					Console.WriteLine("Number of acceptations in second {0}: {1}\t\tBest solution: {2}\t\tTemp: {3}", Diagnostics.second, Diagnostics.AcceptationsPerSecond, DTS.bestRating, DTS.temperature); 
                    Diagnostics.AcceptationsPerSecond = 0;
                    Diagnostics.second++;
                    Diagnostics.Printed = false;
                }
                if (Diagnostics.second % 60 == 0 && Diagnostics.Printed == false)
                {
                    Console.WriteLine("Number of iterations per minute equals: " + Diagnostics.IterationsPerMinute);
                    Diagnostics.IterationsPerMinute = 0;
                    Diagnostics.Printed = true;
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
            Console.WriteLine("time : " + DTS.bestState.GetAllEval());
            Console.WriteLine("orders : " + DTS.bestState.orderScore);
            Console.WriteLine(DTS.bestRating);
            // Print the amount of time that was spent iterating
            Diagnostics.runtimeWatch.Stop();
            Console.WriteLine("Runtime: " + Diagnostics.runtimeWatch.ElapsedMilliseconds + " ms");
            // Do a readkey at the end so that the console does not close after printing a solution
            Console.ReadKey();
		}
	}



}
