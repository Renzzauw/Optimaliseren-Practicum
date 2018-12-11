using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	
	// State object
    public class State : SimulatedAnnealing
    {	
		// Variables
        public List<Status>[] status1, status2; // The week for each truck: It contains 5 dictionaries, one for each day. Each truck gets their own status
        //private Datastructures data;
        private Random random;
        // Make a (random) initial state
        public State()
        {
            random = new Random();
			// TODO: hou dit ff in de gaten of die nullen kloppen :thonking:
            status1 = MakeRandomState(new GarbageTruck(1, 0));
            status2 = MakeRandomState(new GarbageTruck(2, 0));
        }

		public List<Status>[] MakeRandomState(GarbageTruck truck)
        {
            List<Status>[] statusList = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                statusList[i] = MakeRandomDay(truck, i);
            }

            return statusList;
        }

        public List<Status> MakeRandomDay(GarbageTruck initialTruck, int dayIndex)
        {
            List<Status> day = new List<Status>();
            int timestart = 21600;
            int newTime = 0;
            Company comp = DTS.maarheeze;
            Status previous = new Status(0, timestart, comp, initialTruck, 0);
			int iterations = 0;
            GarbageTruck truck = initialTruck;
            Order ord;
            // Allow up to 30 iterations to see whether it is possible to go to another address. If that is no longer possible, end the day
            while (iterations < 30)
            {
                comp = DTS.companyList.ElementAt(random.Next(DTS.companyList.Length));
                // Check if that company has outstanding orders, or whether it has aready been visited
                if (!comp.HasOrders() || comp.IsDayVisited(dayIndex)) continue;
                // Select one of the outstanding orders of that company
                ord = comp.RandomOrder();
                // Calculate the time needed to process and order when having to return immediately
                int traveltime = DTS.timeMatrix[previous.company.companyIndex, comp.companyIndex];
                int processtime = ord.emptyingTime;
                int timeToMaarheze = DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > 63000)
                {
                    iterations++;
                    continue;
                }
                // Process the order          
                day.Add(new Status(dayIndex, timestart, comp, truck.FillTruck(ord), ord.orderNumber));
				timestart += traveltime + processtime;
				ord.ordersDone = true;
                timestart = newTime;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && newTime + timeToMaarheze < 63000)
                {
                    newTime = DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
					// Drive to Maarheze and empty the truck
					day.Add (new Status(dayIndex, timestart, DTS.maarheeze, truck.EmptyTruck(), 0));
                    timestart += 1800;
                    day.Add(previous);
                }
            }
            newTime = DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
			// Drive to Maarheze and empty the truck. TODO: check if the truck is aleady empty if its not already there and emptied
			day.Add(new Status(dayIndex, timestart, DTS.maarheeze, truck.EmptyTruck(), 0));
            return day;
        }

	}
    public class Status
    {
        public int day;
        public int beginTime;
        public Company company;
        public GarbageTruck truck;
        public int ordnr;

        public Status(int d, int startTime, Company c, GarbageTruck gt, int ord)
        {
            day = d;
            beginTime = startTime;
            company = c;
            truck = gt;
            ordnr = ord;
        }
    }

}
