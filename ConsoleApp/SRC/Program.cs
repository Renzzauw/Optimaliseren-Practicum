﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Input;

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
		int i;                                // A counter that keeps track of the total amount of iterations
		float alpha;                            // The percentage to reduce T with, every q iterations
		int q;                                  // The number of iterations before q gets reduced
		protected int seed;                     // Seed for the random generators 
		StateGenerator generator;               // An instance of the stateGenerator class, which will calcuate successor states of a given state

		// Initialize the program
		public void Init()
		{
            // Start the stopwatch
            Diagnostics.initWatch = new Stopwatch();
            Diagnostics.runtimeWatch = new Stopwatch();
			Diagnostics.initWatch.Start();
            Diagnostics.second = 1;
			// Load all variables from the two input files
			DTS.orders = new Dictionary<int, Order>();
			DTS.availableOrders = new Dictionary<int, Order>();
			Initialization init = new Initialization();
			var adjacencyList = init.GetAdjacencyList();
			DTS.distanceMatrix = adjacencyList.Item1;
			DTS.timeMatrix = adjacencyList.Item2;
            DTS.dayStart = 21600;
            DTS.dayEnd = 64800;
            DTS.emptyingTime = 1800;
			// initialize orders
			DTS.companyList = init.MakeCompanies();
			DTS.maarheeze = DTS.companyList[287];
			// Initialize all other variables
			i = 0;
			DTS.temperature = 600;
			alpha = 0.99F;
			q = 10000; //TODO calcuate the total number of neighbours, times 8
			State initial = new State();
			// Initialize the StateGenerator class
			generator = new StateGenerator(initial);
            // Stop the stopwatch and see how long the initialization took
            Diagnostics.initWatch.Stop();
			Console.WriteLine("Initializationtime: " + Diagnostics.initWatch.ElapsedMilliseconds + " ms");
			Run(initial);

		}
		public void Run(State initialState)
		{
            Diagnostics.runtimeWatch.Start();
			State current = initialState;
			while (i < 500000) //TODO: Change this to a better stopping condition
			{
                Diagnostics.IterationsPerSecond++;
                if (Diagnostics.runtimeWatch.ElapsedMilliseconds > 1000 * Diagnostics.second)
                {
                    Console.WriteLine("Number of iterations in second: " + Diagnostics.second + " equals: " + Diagnostics.IterationsPerSecond);
                    Diagnostics.IterationsPerSecond = 0;
                    Diagnostics.second++;
                }
				if (i % q == 0)
				{
					DTS.temperature *= alpha;
				}
				current = generator.GetNextState(current);
                i++;

            }
            FileHandler.SaveState(current);
            FileHandler.Print(current);
            Console.ReadKey();
            Diagnostics.runtimeWatch.Stop();
            Console.WriteLine("Runtime: " + Diagnostics.runtimeWatch.ElapsedMilliseconds + " ms");
		}
	}



}
