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
            Status previous = new Status(0, timestart, 4, comp, initialTruck, 0);
            int companynr;
            int iterations = 0;
            GarbageTruck truck = initialTruck;
            Order ord;
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
                newTime = timestart + traveltime;
                day.Add(new Status(timestart, newTime, 1, comp, truck, ord.orderNumber));
                float newnewtime = newTime + processtime; // TODO: de naam
                day.Add(new Status(newTime, newnewtime, 2, comp, truck.FillTruck(ord), ord.orderNumber));
                ord.ordersDone = true;
                timestart = newnewtime;
                iterations = 0;
                // If the truck is full, and there is time to empty, do it
                if (truck.CheckIfFull() && newnewtime + timeToMaarheze < 63000)
                {
                    newTime = GetTraveltime(comp, data.maarheeze);
                    // Drive to Maarheze and empty the truck
                    day.Add(new Status(timestart, newTime, 1, comp, truck, 0));
                    previous = new Status(newTime, newTime + 1800, 3, data.maarheeze, truck.EmptyTruck(), 0);
                    timestart = newTime + 1800;
                    day.Add(previous);
                }

            }
            newTime = GetTraveltime(comp, data.maarheeze);
            // Drive to Maarheze and empty the truck, if its not already there and emptied
            if (comp != data.maarheeze) day.Add(new Status(timestart, newTime, 1, comp, truck, 0));
            if (!truck.CheckIfEmpty()) day.Add(new Status(newTime, newTime + 1800, 3, data.maarheeze, truck.EmptyTruck(), 0));
            return day;
        }

        public float GetTraveltime(Company a, Company b)
        {
            return data.timeMatrix[a.companyIndex, b.companyIndex];
        }

		// TODO: waarschijnlijk checken dat hij legen niet gaat swappen of herberekenen wanneer je moet legen

		// Remove a random action on a random day of the schedule of a truck
		public List<Status>[] RemoveRandomAction(List<Status>[] statuses)
		{
			// pick a random day of the week
			Random r = new Random();		
			int day = r.Next(6);
			// pick a random action
			int actionIndex = r.Next(statuses[day].Count);
			// Remove the action
			statuses[day].RemoveAt(actionIndex);
			// Return the remaining schedule
			return statuses;
		}

		// Swap two random actions between two trucks
		public Tuple<List<Status>[], List<Status>[]> SwapRandomActions(List<Status>[] statuses1, List<Status>[] statuses2)
		{
			// pick two random days of the week
			Random r = new Random();
			int day1 = r.Next(6);
			int day2 = r.Next(6);
			// pick two random actions			
			int actionIndex1 = r.Next(statuses1[day1].Count);
			int actionIndex2 = r.Next(statuses1[day1].Count);
			Status status1 = statuses1[day1][actionIndex1];
			Status status2 = statuses1[day2][actionIndex2];
			// Swap the actions
			statuses1[day1].RemoveAt(actionIndex1);
			statuses1[day1].Insert(actionIndex1, status2);
			statuses2[day2].RemoveAt(actionIndex2);
			statuses2[day2].Insert(actionIndex2, status1);
			// Return the new schedules
			return new Tuple<List<Status>[], List<Status>[]>(statuses1, statuses2);
		}

		// Swap two random actions within a truck
		public List<Status>[] SwapRandomActions(List<Status>[] statuses)
		{
			// pick two random days of the week
			Random r = new Random();
			int day1 = r.Next(6);
			int day2 = r.Next(6);
			// pick two random actions			
			int actionIndex1 = r.Next(statuses[day1].Count);
			int actionIndex2 = r.Next(statuses[day2].Count);
			Status status1 = statuses[day1][actionIndex1];
			Status status2 = statuses[day2][actionIndex2];
			// Swap the actions
			statuses[day1].Insert(actionIndex1, status2);
			statuses[day2].Insert(actionIndex2, status1);
			// Return the new schedules
			return statuses;
		}

		// Change the day of an action
		public List<Status>[] ChangeActionDay(List<Status>[] statuses)
		{
			// pick a random day of the week
			Random r = new Random();
			int day1 = r.Next(6);
			int day2 = r.Next(6);
			// pick a random action
			int actionIndex1 = r.Next(statuses[day1].Count);
			int actionIndex2 = r.Next(statuses[day2].Count);
			// Remove the action
			Status status = statuses[day1][actionIndex1];
			statuses[day1].RemoveAt(actionIndex1);
			statuses[day2].Insert(actionIndex2, status);
			// Return the remaining schedule
			return statuses;
		}


		// x Swap actions of 2 cars
		// x Swap actions within a car
		// Add action (wat voor actie?)
		// x Remove action
		// x Change day of action

	}
    public class Status
    {
        public float startTime, endTime;
        public enum Actione { Driving, Collecting, Emptying, Nothing } // Added the extra E to avoid conflicts with System.Action
        public Actione action;
        public Company company;
        public GarbageTruck truck;
        public int ordnr;

        public Status(float s, float e, float z, Company c, GarbageTruck gt, int ord)
        {
            startTime = s;
            endTime = e;
            switch (z)
            {
                case 1:
                    action = Actione.Driving;
                    break;
                case 2:
                    action = Actione.Collecting;
                    break;
                case 3:
                    action = Actione.Emptying;
                    break;
                case 4:
                    action = Actione.Nothing;
                    break;
                default:
                    break;
            }
            company = c;
            truck = gt;
            ordnr = ord;
        }
        public bool IfCollecting()
        {
            return action == Actione.Collecting || action == Actione.Emptying;
        }
    }

}
