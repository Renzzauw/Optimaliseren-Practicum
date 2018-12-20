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
        public List<Status>[][] status; // The week for each truck: It contains 5 dictionaries, one for each day. Each truck gets their own status
        //private Datastructures data;
        private Random random;
        private bool emptiedTruck;

        // Make a (random) initial state
        public State()
        {
            random = new Random();
            status = new List<Status>[2][];
            status[0] = MakeRandomState();
            status[1] = MakeRandomState();
        }

		public List<Status>[] MakeRandomState()
        {
            List<Status>[] statusList = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                statusList[i] = MakeRandomDay(i);
            }

            return statusList;
        }

        public List<Status> MakeRandomDay(int dayIndex)
        {
            List<Status> day = new List<Status>();
            int timestart = DTS.dayStart;
            Company comp = DTS.maarheeze;
            GarbageTruck truck = new GarbageTruck();
            Status previous = new Status(0, comp, 0);
			int iterations = 0;
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
                int processtime = (int)ord.emptyingTime;
                int timeToMaarheze = DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > 63000)
                {
                    iterations++;
                    continue;
                }
                // Process the order          
                day.Add(new Status(dayIndex, comp, ord.orderNumber));
                truck.FillTruck(ord);
				timestart += traveltime + processtime;
                DTS.availableOrders.Remove(ord.orderNumber);
				ord.ordersDone = true;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && timestart + timeToMaarheze < 63000)
                {
					// Drive to Maarheze and empty the truck
					day.Add (new Status(dayIndex, DTS.maarheeze, 0));
                    timestart += DTS.emptyingTime + DTS.timeMatrix[comp.companyIndex, DTS.maarheeze.companyIndex];
                    truck.EmptyTruck();
                    day.Add(previous);
                }
            }
			// Drive to Maarheze and empty the truck. TODO: check if the truck is aleady empty if its not already there and emptied
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
