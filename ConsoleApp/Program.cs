using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

	class SimulatedAnnealing
	{
        int[,] distanceMatrix;
        int[,] timeMatrix;

		// Initialize the program
		public void Init()
		{
            //Load all variables from the two input files
            Initialization init = new Initialization();
            Tuple<int[,],int[,]> distTuple = init.GetMatrix();
            distanceMatrix = distTuple.Item1;
            timeMatrix = distTuple.Item2;
		}
	}


}
