using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OptimaliserenPracticum
{
	public class StateGenerator : SimulatedAnnealing 
	{
        private List<Thread> successorfunctions;                           // A list of threads, each containing a successorfunction
        private State oldState, newState;                              // Each iteration is given an old state, and must return a new state
        private bool foundSucc;                                        // A bool that checks whether a successor has been found

        public StateGenerator()
		{
			// Initialize the thread list
		}

		private void CreateNeighbourStates(State oldState)
		{
			List<Status>[] status1, status2;
			status1 = oldState.status1;
			status2 = oldState.status2;
			removedActionState1.RemoveRandomAction(status1);
			removedActionState2.RemoveRandomAction(status2);
			//AddedActionState. TODO: nog implementeren
			SwappedWithinState1.SwapRandomActionsWithin(status1);
			SwappedWithinState2.SwapRandomActionsWithin(status2);
			SwappedBetweenState.SwapRandomActionsBetween(status1, status2);			
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
        public Tuple<List<Status>[], List<Status>[]> SwapRandomActionsBetween(List<Status>[] statuses1, List<Status>[] statuses2)
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
        public List<Status>[] SwapRandomActionsWithin(List<Status>[] statuses)
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
        // Add action (wat voor actie? Ergens pakken uit een lijst?)
        // x Remove action
        // x Change day of action




    }
}
