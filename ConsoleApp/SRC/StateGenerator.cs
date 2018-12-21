using System;
using System.Collections.Generic;

namespace OptimaliserenPracticum
{
    public class StateGenerator
    {
        private State oldState, newState;                              // Each iteration is given an old state, and must return a new state
        private double evalValue;
        private Random r;

        public StateGenerator(State initial)
        {
            r = new Random();
        }

        public State GetNextState(State old)
        {
            oldState = old;
            State newwState = null;
            while (newwState == null)
            {
                // Try one of the successorfunctions, and keep on trying untill one of them returns a successor
                int i = r.Next(7);
                switch (i)
                {
                    case 0: newwState = RemoveRandomAction(0); break;
                    case 1: newwState = RemoveRandomAction(1); break;
                    case 2: newwState = AddRandomAction(0); break;
                    case 3: newwState = AddRandomAction(1); break;
                    case 4: newwState = SwapRandomActionsWithin(0); break;
                    case 5: newwState = SwapRandomActionsWithin(1); break;
                    case 6: newwState = SwapRandomActionsBetween(); break;
                    default: break;
                }
            }
            return newwState;
        }


        // Remove a random action on a random day of the schedule of a truck
        public State RemoveRandomAction(object i)
        {
            // Declare all of the necissary variables
            int x = (int)i;
            List<Status>[] oldStatus;
            List<Status> oldDay, newDay;
            int orda, findDay, removedIndex, rating1, rating2;
            oldStatus = oldState.status[x];
            // Pick a random day of the week
            findDay = r.Next(5);
            if (oldStatus[findDay].Count < 2) return null; // Return if there is only one order left, aka the emptying
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            removedIndex = r.Next(oldDay.Count - 2);
            // Remove a random action
            orda = newDay[removedIndex].ordnr;
            if (orda == 0) return null; // Return if you try to delete an emptying moment
            newDay.RemoveAt(removedIndex);
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay);
            rating2 = EvalDay(newDay);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(orda);
                newState = oldState;
                newState.status[x][findDay] = newDay;
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }

        // Add a random action at a random time
        public State AddRandomAction(object i)
        {
            // Declare all of the necissary variables
            int x = (int)i;
            List<Status>[] oldStatus;
            List<Status> oldDay, newDay;
            int findDay, addedIndex, rating1, rating2;
            Order ord;
            oldStatus = oldState.status[x];
            // pick a random day of the week
            findDay = r.Next(5);
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            addedIndex = r.Next(oldDay.Count - 1);
            // Add a random available order in between two other actions
            if (DTS.availableOrders.Count == 0) return null; // Return when there are no available orders
            ord = DTS.orders[DTS.availableOrders[r.Next(DTS.availableOrders.Count)]];
            newDay.Insert(addedIndex, new Status(findDay, ord.matrixID, ord.orderNumber));
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay);
            rating2 = EvalDay(newDay);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                newState = oldState;
                newState.status[x][findDay] = newDay;
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }

        // Swap two random actions within a truck
        public State SwapRandomActionsWithin(object i)
        {
            // Declare all of the necissary variables
            int x = (int) i;
            List<Status>[] oldStatus;
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2, rating1, rating2, actionIndex1, actionIndex2;
            Status stat1, stat2, tempstat1, tempstat2;
            oldStatus = oldState.status[x];
            // Pick two random days in which to swap
            day1 = r.Next(5);
            day2 = r.Next(5);
            if (day1 == day2) return null; // Return if both days are the same
            oldDay1 = oldStatus[day1];
            oldDay2 = oldStatus[day2];
            newDay1 = new List<Status>(oldDay1);
            newDay2 = new List<Status>(oldDay2);
            if (newDay1.Count < 2 || newDay2.Count < 2) return null; // Return if either day has only one status, aka the emptying at the end
            // pick two random actions			
            actionIndex1 = r.Next(oldDay1.Count - 2);
            actionIndex2 = r.Next(oldDay2.Count - 2);
            stat1 = oldDay1[actionIndex1];
            stat2 = oldDay2[actionIndex2];
            // Swap these actions around
            tempstat2 = new Status(day1, stat2.ordid, stat2.ordnr);
            tempstat1 = new Status(day2, stat1.ordid, stat1.ordnr);
            newDay1.Remove(stat1);
            newDay2.Remove(stat2);
            newDay1.Insert(actionIndex1, tempstat2);
            newDay2.Insert(actionIndex2, tempstat1);
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay1) + EvalDay(oldDay2);
            rating2 = EvalDay(newDay1) + EvalDay(newDay2);
            if (AcceptNewDay(EvalDay(oldDay1) + EvalDay(oldDay2), EvalDay(newDay1) + EvalDay(newDay2), r))
            {
                // If accepted, return the new state
                newState = oldState;
                newState.status[x][day1] = newDay1;
                newState.status[x][day2] = newDay2;
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }

        public State SwapRandomActionsBetween()
        {
            // Declare all of the necissary variables
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2, actionIndex1, actionIndex2, rating1, rating2; ;
            Status stat1, stat2, tempstat1, tempstat2;
            // Pick two random days, each for a different truck
            day1 = r.Next(5);
            day2 = r.Next(5);
            oldDay1 = oldState.status[0][day1];
            oldDay2 = oldState.status[1][day2];
            newDay1 = new List<Status>(oldDay1);
            newDay2 = new List<Status>(oldDay2);
            if (newDay1.Count < 2 || newDay2.Count < 2) return null;  // Return if either day has only one status, aka the emptying at the end
            // pick two random actions			
            actionIndex1 = r.Next(oldDay1.Count - 2);
            actionIndex2 = r.Next(oldDay2.Count - 2);
            stat1 = oldDay1[actionIndex1];
            stat2 = oldDay2[actionIndex2];
            // Swap the actions
            tempstat2 = new Status(day1, stat2.ordid, stat2.ordnr);
            tempstat1 = new Status(day2, stat1.ordid, stat1.ordnr);
            newDay1.Remove(stat1);
            newDay2.Remove(stat2);
            newDay1.Insert(actionIndex1, tempstat2);
            newDay2.Insert(actionIndex2, tempstat1);
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay1) + EvalDay(oldDay2);
            rating2 = EvalDay(newDay1) + EvalDay(newDay2);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, return the new state
                newState = oldState;
                newState.status[0][day1] = newDay1;
                newState.status[1][day2] = newDay2;
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }


        // TODO: Compleet herschrijven
        // TODO: Compleet herschrijven
        public int EvalDay(List<Status> day)
        {
            int score = 0;
            int newstart = DTS.dayStart;
            int previousId = DTS.maarheeze;
            GarbageTruck truck = new GarbageTruck();
            // Iterate over all actions
            foreach (Status action in day)
            {
                if (action.ordnr == 0)
                {
                    newstart += DTS.timeMatrix[previousId, action.ordid] + DTS.emptyingTime;
                    truck.EmptyTruck();
                    previousId = DTS.maarheeze;
                }
                else
                {
                    Order ord = DTS.orders[action.ordnr];
                    newstart += DTS.timeMatrix[previousId, action.ordid] + (int)ord.emptyingTime;
                    truck.FillTruck(ord);
                    // Check if there's a moment when the truck is full. deduct a lot of score for that
                    if (truck.CheckIfOverloaded()) score -= 1000;
                }
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            //deducing score acoringly for not doing an order.
            foreach (int x in DTS.availableOrders)
            {
                score -= 3 * (int)(DTS.orders[x].emptyingTime) / 5;
            }
            // Punish overtime pretty heavily en rest time normally
            if (newstart >= DTS.dayEnd)
            {
                score += DTS.dayEnd - newstart * 10;
            }
            else
            {
                score -= newstart - DTS.dayEnd;
            }
            return score;
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(int oldrating, int newrating, Random r)
        {
            return (newrating >= oldrating) || PCheck(oldrating, newrating, r);
        }

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(int fx, int fy, Random r)
        {
            if (fy <  0) return false;
            evalValue = Math.Pow(Math.E, (fy - fx) / DTS.temperature);
            return evalValue >= r.NextDouble();
        }
    }
}
