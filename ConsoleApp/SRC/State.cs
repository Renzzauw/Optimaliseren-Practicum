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
        private Random random;          // A random number generator that will be used in the creation of the initial state

        // The constructor makes a new random initial state
        public State()
        {
            // Initialize the variables
            random = new Random();
            status = new List<Status>[2][];
            // Create the schedule of both trucks seperately
            status[0] = MakeRandomWeek();
            status[1] = MakeRandomWeek();
        }


        // The constructor copies from an old state
        public State(List<Status>[][] status)
        {
            status = new List<Status>[2][];
            status[0] = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                status[0][i] = new List<Status>(status[0][i]);
            }
            status[1] = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                status[1][i] = new List<Status>(status[1][i]);
            }
        }

        // Function that generater a random week
        public List<Status>[] MakeRandomWeek()
        {
            // Create each of the 5 days seperately
            List<Status>[] statusList = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                statusList[i] = MakeRandomDay(i);
            }
            return statusList;
        }

        public List<Status> MakeRandomDay(int dayIndex)
        {
            // Initialize all the variables needed for iterating
            List<Status> day = new List<Status>();
            int timestart = DTS.dayStart;
            int prevId    = DTS.maarheeze;
            int traveltime, processtime, timeToMaarheze;
            GarbageTruck truck = new GarbageTruck();
            Status status  = new Status(0, DTS.maarheeze, 0);
			int iterations = 0;
            int randomOrder;
            Order ord;
            // Add a random action on top of the already existing day. Allow up to 30 iterations to see whether it is possible to go to another address. If that is no longer possible, end the day
            while (iterations < 30)
            {
                // Pick a random available order, 
                randomOrder = random.Next(DTS.availableOrders.Count);
                ord = DTS.orders[DTS.availableOrders[randomOrder]];
                // Calculate the time needed to process and order when having to return immediately
                traveltime = DTS.timeMatrix[status.ordid, ord.matrixID];
                processtime = (int)ord.emptyingTime;
                timeToMaarheze = DTS.timeMatrix[ord.matrixID, DTS.maarheeze];
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > DTS.dayEnd - 1800)
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
			// Drive to Maarheze and empty the truck.
           day.Add(new Status(dayIndex, DTS.maarheeze, 0));
           return day;
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
