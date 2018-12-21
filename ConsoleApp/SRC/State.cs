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
            Company comp = DTS.maarheeze;
            GarbageTruck truck = new GarbageTruck();
            Status previous = new Status(0, comp, 0);
			int iterations = 0;
            Order ord;
            // Add a random action on top of the already existing day. Allow up to 30 iterations to see whether it is possible to go to another address. If that is no longer possible, end the day
            while (iterations < 30)
            {
                comp = DTS.companyList.ElementAt(random.Next(DTS.companyList.Length));
                // Check if that company has outstanding orders, or whether it has aready been visited
                if (!comp.HasOrders() || comp.IsDayVisited(dayIndex)) continue; // DIT kan gwn want hij mag verschillende orders doen van een bepaalde company op de zelfde dag FOUT. Niet dezelfde order nog een keer met frequenty hoger dan 1.
                // Select one of the outstanding orders of that company
                ord = comp.RandomOrder();
                // Calculate the time needed to process and order when having to return immediately
                int traveltime = DTS.timeMatrix[previous.company.companyIndex, comp.companyIndex];
                int processtime = (int)ord.emptyingTime;
                int timeToMaarheze = DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > DTS.dayEnd)
                {
                    iterations++;
                    continue;
                }
                // Process the order          
                day.Add(new Status(dayIndex, comp, ord.orderNumber));
                truck.FillTruck(ord);
                previous = new Status(dayIndex, comp, ord.orderNumber);
				timestart += traveltime + processtime;
                DTS.availableOrders.Remove(ord.orderNumber);
				ord.ordersDone = true;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && timestart + timeToMaarheze < DTS.dayEnd)
                {
					// Drive to Maarheze and empty the truck
					day.Add (new Status(dayIndex, DTS.maarheeze, 0));
                    timestart += DTS.emptyingTime + DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
                    truck.EmptyTruck();
                    day.Add(previous);
                }
            }
			// Drive to Maarheze and empty the truck.
            if (!truck.CheckIfEmpty())
            {
                day.Add(new Status(dayIndex, DTS.maarheeze, 0));
            }
           return day;
        }

	}
    public class Status
    {
        public int day;
        public Company company;
        public int ordnr;

        public Status(int d, Company c, int ord)
        {
            day = d;
            company = c;
            ordnr = ord;
        }
    }

}
