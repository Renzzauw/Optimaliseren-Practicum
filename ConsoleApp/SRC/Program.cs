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
        ulong i; // A counter that keeps track of the total amount of iterations
        int t; // The control parameter
        int alpha; // The percentage to reduce T with, every q iterations
        int q; // The number of iterations before q gets reduced
        int qCounter; // Keeps track of how many iterations we have had since our last T change
        Random r; // A random number generator to potentially accept worse states

        // Initialize the program
        public void Init()
        {
            //Load all variables from the two input files
            Initialization init = new Initialization();
            Tuple<int[,], int[,]> distTuple = init.GetMatrix();
            distanceMatrix = distTuple.Item1;
            timeMatrix = distTuple.Item2;
            // Initialize all other variables

        }

        public void Run()
        {
            //State current = GetInitialstate;
            //State successor;
            //int currentR = getScore(current);
            //int nextR = 0;
            while (i < 1000000) //TODO: Change this to a stopping condition
            {
                i++;
                if (qCounter == q)
                {
                    t *= alpha;
                    qCounter = 0;
                }
                while (true)
                {
                    //successor = getSuccessor(current);
                    //nextR = getScore(successor);
                    //if (nextR < currentR && !PCheck(currentR, nextR) continue; // New state didn't get accepted, try again
                    //current = successor;
                    //currentR = nextR;
                    break;
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
