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
                int i = r.Next(4);
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
            List<Status>[] oldStatus, newStatus;
            List<Status> oldDay, newDay;
            int orda, findDay, removedIndex;
            double rating1, rating2;
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
            //change the day in the week for a new week
            newStatus = new List<Status>[5];
            for (int a = 0; a < 5; a++)
            {
                newStatus[a] = oldStatus[a];
            }
            newStatus[findDay] = newDay;
            // Already make the the new state, so that it can be properly evaluated
            newState = new State(oldState.status);
            newState.status[x][findDay] = newDay;
            DTS.availableOrders.Add(orda);
            // Give ratings to the old and new day, and evaluate them
            rating1 = Eval(oldState);
            rating2 = Eval(newState);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.NewBest(newState, rating2);
                return newState;
            }
            DTS.availableOrders.Remove(orda);
            return null;
        }

        // Add a random action at a random time
        public State AddRandomAction(object i)
        {
            // Declare all of the necissary variables
            int x = (int)i;
            List<Status>[] oldStatus;
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            double rating1, rating2;
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
            // Already make the the new state, so that it can be properly evaluated
            newState = new State(oldState.status);
            newState.status[x][findDay] = newDay;
            DTS.availableOrders.Remove(ord.orderNumber);
            // Give ratings to the old and new day, and evaluate them
            rating1 = Eval(oldState);
            rating2 = Eval(newState);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.NewBest(newState, rating2);
                return newState;
            }
            DTS.availableOrders.Add(ord.orderNumber);
            return null;
        }

        // Swap two random actions within a truck
        public State SwapRandomActionsWithin(object i)
        {
            // Declare all of the necissary variables
            int x = (int) i;
            List<Status>[] oldStatus;
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2, actionIndex1, actionIndex2;
            double rating1, rating2;
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
            // Already make the the new state, so that it can be properly evaluated
            newState = new State(oldState.status);
            newState.status[x][day1] = newDay1;
            newState.status[x][day2] = newDay2;
            // Give ratings to the old and new day, and evaluate them
            rating1 = Eval(oldState);
            rating2 = Eval(newState);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, return the new state
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }

        public State SwapRandomActionsBetween()
        {
            // Declare all of the necissary variables
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2, actionIndex1, actionIndex2;
            double rating1, rating2;
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
            // Already make the the new state, so that it can be properly evaluated
            newState = new State(oldState.status);
            newState.status[0][day1] = newDay1;
            newState.status[1][day2] = newDay2;
            // Give ratings to the old and new day, and evaluate them
            rating1 = Eval(oldState);
            rating2 = Eval(newState);
            if (AcceptNewDay(rating1, rating2, r))
            {
                // If accepted, return the new state
                DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }


        // TODO: Compleet herschrijven
        public double Eval(State state)
        {
            List<Status>[] actionstruck1, actionstruck2;
            double score = 0;
            List<Status>[][] statuses = state.status;
            actionstruck1 = statuses[0];
            actionstruck2 = statuses[1];
            GarbageTruck truck1 = new GarbageTruck();
            GarbageTruck truck2 = new GarbageTruck();
            int truck1totaaltijd = 0;
            int truck2totaaltijd = 0;

            int previousId = DTS.maarheeze;

            foreach (List<Status> day in actionstruck1)
            {
                //reset the day how its supposed to.
                int newstart = DTS.dayStart;

                //iterate over all actions
                foreach (Status action in day)
                {

                    if (action.ordnr == 0)
                    {
                        newstart += DTS.timeMatrix[previousId, action.ordid] + DTS.emptyingTime;
                        truck1.EmptyTruck();
                        previousId = DTS.maarheeze;
                    }
                    else
                    {
                        Order ord = DTS.orders[action.ordnr];
                        newstart += DTS.timeMatrix[previousId, action.ordid] + (int)ord.emptyingTime;
                        previousId = action.ordid;

                        truck1.FillTruck(ord);
                        // Check if there's a moment when the truck is full. deduct a lot of score for that
                        if (truck1.CheckIfOverloaded()) score -= 1000;
                    }
           

                }
                //Punish overtime pretty heavily en rest time normally
                if (newstart >= DTS.dayEnd)
                {
                    truck1totaaltijd += (newstart - DTS.dayEnd) * 10;
                }
                truck1totaaltijd += newstart;
            }

            previousId = DTS.maarheeze;

            foreach (List<Status> day in actionstruck2)
            {
                //reset the day how its supposed to.
                int newstart = DTS.dayStart;

                //iterate over all actions
                foreach (Status action in day)
                {

                    if (action.ordnr == 0)
                    {
                        newstart += DTS.timeMatrix[previousId, action.ordid] + DTS.emptyingTime;
                        truck2.EmptyTruck();
                        previousId = DTS.maarheeze;
                    }
                    else
                    {
                        Order ord = DTS.orders[action.ordnr];
                        newstart += DTS.timeMatrix[previousId, action.ordid] + (int)ord.emptyingTime;
                        previousId = action.ordid;
                        truck2.FillTruck(ord);
                        // Check if there's a moment when the truck is full. deduct a lot of score for that
                        if (truck2.CheckIfOverloaded()) score -= 1000;
                    }


                }
                //Punish overtime pretty heavily en rest time normally
                if (newstart >= DTS.dayEnd)
                {
                    truck2totaaltijd += (newstart - DTS.dayEnd) * 10;
                }
                truck2totaaltijd += newstart;
            }

            score -= truck1totaaltijd + truck2totaaltijd;



            //deducing score acoringly for not doing an order.
            foreach (int x in DTS.availableOrders)
            {
                score -= 3 * (DTS.orders[x].emptyingTime * DTS.orders[x].frequency);
            }

            
            return score / 60;
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(double oldrating, double newrating, Random r)
        {
            if (newrating > oldrating)
            {
                return true;
            }
            return PCheck(oldrating, newrating, r);
        }

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(double fx, double fy, Random r)
        {
            evalValue = Math.Pow(Math.E, (fy - fx) / DTS.temperature);
            return evalValue >= r.NextDouble();
        }
    }
}
