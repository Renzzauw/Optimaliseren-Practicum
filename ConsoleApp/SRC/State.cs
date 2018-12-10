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
        private Datastructures data;
        private Random random;
        // Make a (random) initial state
        public State(Datastructures data)
        {
            this.data = data;
            random = new Random();
            status1 = MakeRandomState(new GarbageTruck());
            status2 = MakeRandomState(new GarbageTruck());
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
            float timestart = 21600;
            float newTime = 0;
            Company comp = data.maarheeze;
            Status previous = new Status(0, timestart, comp, initialTruck, 0);
            int companynr;
            int iterations = 0;
            GarbageTruck truck = initialTruck;
            Order ord;
            // Allow up to 30 iterations to see whether it is possible to go to another address. If that is no longer possible, end the day
            while (iterations < 30)
            {
                companynr = random.Next(data.companyList.Length);
                comp = data.companyList[companynr];
                // Check if that company has outstanding orders, or whether it has aready been visited
                if (!comp.HasOrders() || comp.IsDayVisited(dayIndex)) continue;
                // Select one of the outstanding orders of that company
                ord = comp.RandomOrder();
                // Calculate the time needed to process and order when having to return immediately
                float traveltime = GetTraveltime(previous.company, comp);
                float processtime = ord.emptyingTime;
                float timeToMaarheze = GetTraveltime(comp, data.maarheeze);
                // If there is no time to complete the order and return to the depot, try again
                if (timestart + traveltime + processtime + timeToMaarheze > 63000)
                {
                    iterations++;
                    continue;
                }
                // Process the order
                newTime = timestart + traveltime + processtime;
                day.Add(new Status(timestart, newTime, comp, truck.FillTruck(ord), ord.orderNumber));
                ord.ordersDone = true;
                timestart = newTime;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && newTime + timeToMaarheze < 63000)
                {
                    newTime = GetTraveltime(comp, data.maarheeze);
                    // Drive to Maarheze and empty the truck
                    day.Add (new Status(timestart, newTime + 1800, data.maarheeze, truck.EmptyTruck(), 0));
                    timestart = newTime + 1800;
                    day.Add(previous);
                }

            }
            newTime = GetTraveltime(comp, data.maarheeze);
            // Drive to Maarheze and empty the truck. TODO: check if the truck is aleady empty if its not already there and emptied
            day.Add(new Status(timestart, newTime + 1800, data.maarheeze, truck.EmptyTruck(), 0));
            return day;
        }

        public float GetTraveltime(Company a, Company b)
        {
            return data.timeMatrix[a.companyIndex, b.companyIndex];
        }
	}
    public class Status
    {
        public float startTime, endTime;
        public Company company;
        public GarbageTruck truck;
        public int ordnr;

        public Status(float s, float e, Company c, GarbageTruck gt, int ord)
        {
            startTime = s;
            endTime = e;
            company = c;
            truck = gt;
            ordnr = ord;
        }
    }

}
