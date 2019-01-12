using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimaliserenPracticum
{
	
	// State object
    public class State
    {	
		// Variables
        public List<Status>[][] status; // The status is a jagged array of status lists. What this means is that the first index stands for the truck (0 or 1), the second index for the day (0 .. 4), and that contains a list of statuses for each day.
        public Eval[][] evals;          // Contains all of the evaluations for a given day and truck
        public double totalValue;       // The total score of the state
        private Random random;          // A random number generator that will be used in the creation of the initial state

        // The constructor makes a new random initial state
        public State()
        {
            // Initialize the variables
            random = new Random();
            status = new List<Status>[2][];
            evals = new Eval[2][];
            // Create the schedule of both trucks seperately
            status[0] = MakeRandomWeek(0);
            status[1] = MakeRandomWeek(1);
            totalValue = GetAllEval();
        }

        // The constructor copies from an old state
        public State(List<Status>[][] stat)
        {
            status = new List<Status>[2][];
            status[0] = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                status[0][i] = new List<Status>(stat[0][i]);
            }
            status[1] = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                status[1][i] = new List<Status>(stat[1][i]);
            }
        }

        // Function that generater a random week
        public List<Status>[] MakeRandomWeek(int truck)
        {
            evals[truck] = new Eval[5];
            // Create each of the 5 days seperately
            List<Status>[] statusList = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                statusList[i] = MakeRandomDay(i, truck);
            }
            return statusList;
        }

        public List<Status> MakeRandomDay(int dayIndex, int t)
        {
            // Initialize all the variables needed for iterating
            List<Status> day = new List<Status>();
            int timestart = DTS.dayStart;
            int prevId    = DTS.maarheeze;
            int traveltime, processtime, timeToMaarheze;
            GarbageTruck truck = new GarbageTruck();
            Status status = new Status(0, DTS.maarheeze, 0);
			int iterations = 0;
            int randomOrder;
            Order ord;
            // Add a random action on top of the already existing day. Allow up to 30 iterations to see whether it is possible to go to another address. If that is no longer possible, end the day
            while (iterations < 30)
            {
                // Pick a random available order, 
                randomOrder = random.Next(DTS.availableOrders.Count);
                ord = DTS.orders[DTS.availableOrders[randomOrder]];
                if (ord.frequency > 1)
                {
                    iterations++;
                    continue;
                }

                // Calculate the time needed to process and order when having to return immediately
                traveltime = DTS.timeMatrix[status.ordid, ord.matrixID];
                processtime = (int)ord.emptyingTime;
                timeToMaarheze = DTS.timeMatrix[ord.matrixID, DTS.maarheeze];
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > 2 * (DTS.dayEnd - DTS.emptyingTime))
                {
                    iterations++;
                    continue;
                }
                // Process the order       
                status = new Status(dayIndex, ord.matrixID, ord.orderNumber);
                day.Add(status);
                truck.FillTruck(ord);
				timestart += traveltime + processtime;
                DTS.availableOrders.Remove(ord.orderNumber);
				ord.ordersDone = true;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && timestart + timeToMaarheze < DTS.dayEnd)
                {
                    // Drive to Maarheze and empty the truck
                    status = new Status(dayIndex, DTS.maarheeze, 0);
                    day.Add(status);
                    timestart += DTS.emptyingTime + DTS.timeMatrix[ord.matrixID, DTS.maarheeze];
                    truck.EmptyTruck();
                    day.Add(status);
                }
            }
            // Determine the new evaluationvalue of the day
            evals[t][dayIndex] = new Eval(DTS.CalcDayEval(timestart,truck), timestart, truck);
            // Drive to Maarheze and empty the truck.
            day.Add(new Status(dayIndex, DTS.maarheeze, 0));
            return day;
        }

        // Return the saved evaluation values of every day
        public double GetAllEval()
        {
            double acc = 0;
            for(ushort i = 0; i < 2; i++)
            {
                for (ushort j = 0; j < 5; j++)
                {
                    acc += evals[i][j].value;
                }
            }
            // Add all of the available order to the total value
            foreach (int x in DTS.availableOrders)
            {
                acc += 3 * DTS.orders[x].emptyingTime * DTS.orders[x].frequency;
            }
            return acc;
        }

	}
    public class Status
    {
        public int day;         // The day of the status
        public int ordid;       // The matrixID of the order that is currently executed
        public int ordnr;       // The number of the order that is currently executed. In the case of emptying the truck, this is 0

        // Constructor
        public Status(int d, int id, int nr)
        {
            day = d;
            ordid = id;
            ordnr = nr;
        }
    }

}
