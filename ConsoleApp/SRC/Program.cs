using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace OptimaliserenPracticum
{
	class Program
    {
        
        static void Main(string[] args)
        {
            SimulatedAnnealing SA = new SimulatedAnnealing();
            SA.InitDiagnostics();
            SA.Init();
        }
    }

	public class SimulatedAnnealing
	{
		// Variables      
		Stopwatch initWatch;                    // Stopwatches to keep track of time
		Stopwatch runtimeWatch;
		ulong i;                                // A counter that keeps track of the total amount of iterations
		float alpha;                            // The percentage to reduce T with, every q iterations
		int q;                                  // The number of iterations before q gets reduced
		int qCounter;                           // Keeps track of how many iterations we have had since our last T change
		protected int seed;                     // Seed for the random generators 
		StateGenerator generator;               // An instance of the stateGenerator class, which will calcuate successor states of a given state

		// Initialize the program
		public void Init()
		{
			// Start the stopwatch
			initWatch.Start();
			// Load all variables from the two input files
			DTS.orders = new Dictionary<int, Order>();
			DTS.availableOrders = new Dictionary<int, Order>();
			Initialization init = new Initialization();
			//data = new Datastructures();
			var adjacencyList = init.GetAdjacencyList();
			DTS.distanceMatrix = adjacencyList.Item1;
			DTS.timeMatrix = adjacencyList.Item2;
			// initialize orders
			DTS.companyList = init.MakeCompanies();
			DTS.maarheeze = DTS.companyList[287];
			// Initialize all other variables
			i = 0;
			DTS.temperature = 10000;
			alpha = 0.99F;
			q = 8; //TODO calcuate the total number of neighbours, times 8
			qCounter = 0;
			State initial = new State();
			// Initialize the StateGenerator class
			generator = new StateGenerator();
			// Stop the stopwatch and see how long the initialization took
			initWatch.Stop();
			//Console.WriteLine("Initializationtime: " + initWatch.ElapsedMilliseconds + " ms");
			Run(initial);

		}

		// Initialize stopwatches for program diagnostics
		public void InitDiagnostics()
		{
			initWatch = new Stopwatch();
			runtimeWatch = new Stopwatch();
		}

		public void Run(State initialState)
		{
			runtimeWatch.Start();
			State current = initialState;
			while (i < 1000000) //TODO: Change this to a better stopping condition
			{
				i++;
				if (qCounter == q)
				{
					DTS.temperature *= alpha;
					qCounter = 0;
				}
				current = generator.GetNextState(current);
			}
			runtimeWatch.Stop();

			FileHandler.SaveState(current);

            Console.WriteLine("Runtime: " + initWatch.ElapsedMilliseconds + " ms");
		}
	}



}
