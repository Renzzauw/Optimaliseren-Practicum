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
		float t;                                // The control parameter
		float alpha;                            // The percentage to reduce T with, every q iterations
		int q;                                  // The number of iterations before q gets reduced
		int qCounter;                           // Keeps track of how many iterations we have had since our last T change
		protected int seed;                     // Seed for the random generators 
		Random r;                               // A random number generator to potentially accept worse states
		StateGenerator generator;               // An instance of the stateGenerator class, which will calcuate successor states of a given state

		// Initialize the program
		public void Init()
		{
			// Start the stopwatch
			initWatch.Start();
			// Load all variables from the two input files
			Initialization init = new Initialization();
			//data = new Datastructures();
			var adjacencyList = init.GetAdjacencyList();
			Datastructures.distanceMatrix = adjacencyList.Item1;
			Datastructures.timeMatrix = adjacencyList.Item2;
			// initialize orders
			Datastructures.companyList = init.MakeCompanies();
			Datastructures.maarheeze = Datastructures.companyList[287];
			// Initialize all other variables
			i = 0;
			t = 10000;
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
			State successor;
			//int currentR = getScore(current);
			int nextR = 0;

			while (i < 1000000) //TODO: Change this to a stopping condition
			{
				i++;
				if (qCounter == q)
				{
					t *= alpha;
					qCounter = 0;
				}
				successor = generator.GetNewState(current);
			}
			runtimeWatch.Stop();
			//Console.WriteLine("Runtime: " + initWatch.ElapsedMilliseconds + " ms");
			Print(current);
		}

		public void Print(State state)
		{
			int daycounter = 1;
			// print the path of the first truck
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day = state.status1[i];
				foreach (Status status in day)
				{
					Console.WriteLine("1; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
			}
			// print the path of the second truck
			// TODO: losse functie
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day = state.status2[i];
				foreach (Status status in day)
				{
					Console.WriteLine("2; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
			}
		}

		// Checks if the P is smaller than a random number. Return true if yes.
		public bool PCheck(int fx, int fy)
		{
			return Math.Pow(Math.E, (fx - fy) / t) < r.Next(0, 1);
		}
	}



}
