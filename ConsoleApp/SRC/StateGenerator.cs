using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OptimaliserenPracticum
{
	public class StateGenerator : SimulatedAnnealing 
	{
        private Thread[] successorfunctions;                       // A list of threads, each containing a successorfunction
        private State oldState, newState;                              // Each iteration is given an old state, and must return a new state
        private List<Status>[] status1, status2;                        // The two seperate days of the old state, written explicitely to make calcuations easier
        private bool foundSucc;                                        // A bool that checks whether a successor has been found
        private Random r;                                              // Random number generator that is used sometimes

        public StateGenerator()
		{
            // Initialize the thread array
            successorfunctions = new Thread[7];
            r = new Random();
            successorfunctions[0] = new Thread(RemoveRandomAction1);
            successorfunctions[1] = new Thread(RemoveRandomAction2);
            successorfunctions[2] = new Thread(AddRandomAction1);
            successorfunctions[3] = new Thread(AddRandomAction2);
            successorfunctions[4] = new Thread(SwapRandomActionsWithin1);
            successorfunctions[5] = new Thread(SwapRandomActionsWithin2);
            successorfunctions[6] = new Thread(SwapRandomActionsBetween);
        }

		private State GetNextState(State old)
		{
            oldState = old;
            status1 = old.status1;
            status2 = old.status2;
            // Start all the threads
            successorfunctions[0].Start(1);
            successorfunctions[1].Start(2);
            successorfunctions[2].Start(1);
            successorfunctions[3].Start(2);
            successorfunctions[4].Start(1);
            successorfunctions[5].Start(2);
            successorfunctions[6].Start();
            // Wait untill all threads finish
            for(int i = 0; i < successorfunctions.Length; i++)
            {
                successorfunctions[i].Join();
            }
            return newState;
        }


        // TODO: waarschijnlijk checken dat hij legen niet gaat swappen of herberekenen wanneer je moet legen

        // Remove a random action on a random day of the schedule of a truck
        public void RemoveRandomAction1(object o)
        {
            List<Status>[] localStatus = GetStatus((int)o);
            State localNew;
            while (!foundSucc)
            {
                // pick a random day of the week
                int day = r.Next(6);
                // pick a random action
                int actionIndex = r.Next(status1[day].Count);
                // Remove the action
                localNew = oldState.status1[day].RemoveAt(actionIndex);
                // FIx the next action so that it starts from the right point
                // Return the remaining schedule
            }
            return ;
        }
        // Remove a random action on a random day of the schedule of a truck

        public void RemoveRandomAction1(object o)
        {
            List<Status> oldDay;
            List<Status> newDay;
            while (!foundSucc)
            {
                // pick a random day of the week
                oldDay = oldState.status1[r.Next(6)];
                newDay = oldDay;
                int removedIndex = (r.Next(oldDay.Count);
                // Remove a random action
                newDay.RemoveAt(removedIndex);
                // Fix the next action so that it starts from the right point
                MoveAction(newDay, removedIndex);
                // Return the remaining schedule
            }
            return;
        }

        // Swap two random actions between two trucks
        public Tuple<List<Status>[], List<Status>[]> SwapRandomActionsBetween()
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
        public List<Status>[] SwapRandomActionsWithin()
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
        public List<Status>[] ChangeActionDay()
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

        public int EvalDay(List<Status> day)
        {
            int score = 0;
            // More orders on a day is generally better
            score += day.Count * 100;
            int previousEnd = 21600;
            // Iterate over all actions
            foreach(Status action in day)
            {
                // See if the two events overlap. If yes, deduct points
                if (action.startTime < previousEnd) score -= ((previousEnd - action.startTime) * 10);
                // Reward "free" time in between orders
                else if (action.startTime > previousEnd) score += ((previousEnd - action.startTime) / 5);
                // Check if there's a moment when the truck is full. deduct a lot of score for that
                if (action.truck.CheckIfOverloaded()) score -= 1000;
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            return score;
        }
        public List<Status> MoveAction(List<Status> list, int index)
        {
            Company comp = Datastructures.maarheeze;
            int endTime = 21600;
            if (index > 0)
            {
                comp = list[index - 1].company;
                endTime = list[index - 1].endTime;
            }
            Status toSwap = list[index];
            list[index] = new Status(toSwap.day, toSwap.endTime, toSwap.endTime + Datastructures.timeMatrix[comp.companyIndex, toSwap.company.companyIndex] + Datastructures.iets, toSwap.c, toSwap.truck.FillTruck(Datastructures.iets[toSwap.ordnr]), toSwap.ordnr);
            return list;

        }



        // x Swap actions of 2 cars
        // x Swap actions within a car
        // Add action (wat voor actie? Ergens pakken uit een lijst?)
        // x Remove action
        // x Change day of action




    }
}
