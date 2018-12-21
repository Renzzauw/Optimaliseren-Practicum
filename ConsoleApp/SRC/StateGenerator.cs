using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OptimaliserenPracticum
{
    public class StateGenerator : SimulatedAnnealing
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
                //todo terug zetten naar 7
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
            int x = (int)i;
            Random r = new Random();
            List<Status>[] oldStatus;
            List<Status> oldDay;
            List<Status> newDay;
            int orda;
            int findDay, removedIndex;
            int rating1, rating2;
            oldStatus = oldState.status[x];
            // pick a random day of the week
            findDay = r.Next(5);
            if (oldStatus[findDay].Count == 1) return null;
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            removedIndex = r.Next(oldDay.Count - 2);
            // Remove a random action
            orda = newDay[removedIndex].ordnr;
            if (orda == 0) return null;
            newDay.RemoveAt(removedIndex);
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay);
            rating2 = EvalDay(newDay);
            if (AcceptNewDay(rating1, rating2, r))
            {
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
            int x = (int)i;
            List<Status>[] oldStatus;
            Random r = new Random();
            List<Status> oldDay, newDay;
            int findDay, addedIndex;
            int rating1, rating2;
            Order ord;
            oldStatus = oldState.status[x];
            // pick a random day of the week
            findDay = r.Next(5);
            oldDay = oldStatus[findDay];
            newDay = new List<Status>(oldDay);
            addedIndex = r.Next(oldDay.Count - 1);
            if (DTS.availableOrders.Count == 0) return null;
            // Add a random available action in between two other actions
            ord = DTS.orders[DTS.availableOrders[r.Next(DTS.availableOrders.Count)]];
            newDay.Insert(addedIndex, new Status(findDay, DTS.companyList[ord.matrixID], ord.orderNumber));
            // Give ratings to the old and new day, and evaluate them
            rating1 = EvalDay(oldDay);
            rating2 = EvalDay(newDay);
            if (AcceptNewDay(rating1, rating2, r))
            {
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
            int x = (int) i;
            Random r = new Random();
            List<Status>[] oldStatus;
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2;
            int rating1, rating2;
            Status stata1, stata2, tempstata1, tempstata2;
            int actionIndexa1, actionIndexa2;
            oldStatus = oldState.status[x];
            day1 = r.Next(5);
            day2 = r.Next(5);
            oldDay1 = oldStatus[day1];
            oldDay2 = oldStatus[day2];
            newDay1 = new List<Status>(oldDay1);
            newDay2 = new List<Status>(oldDay2);
            if (day1 == day2) return null;
            if (newDay1.Count < 2 || newDay2.Count < 2) return null;
            // pick two random actions			
            actionIndexa1 = r.Next(oldDay1.Count - 2);
            actionIndexa2 = r.Next(oldDay2.Count - 2);
            stata1 = oldDay1[actionIndexa1];
            stata2 = oldDay2[actionIndexa2];
            // Change times so that they are correct, if there was a different action before
            if (actionIndexa1 != 0)
            {
                tempstata2 = new Status(day1, stata2.company, stata2.ordnr);
            }
            else
            {
                tempstata2 = new Status(day1, stata2.company, stata2.ordnr);
            }
            if (actionIndexa2 != 0)
            {
                tempstata1 = new Status(day2, stata1.company, stata1.ordnr);
            }
            else
            {
                tempstata1 = new Status(day2, stata1.company, stata1.ordnr);
            }
            // Swap the actions
            newDay1.Remove(stata1);
            newDay2.Remove(stata2);
            newDay1.Insert(actionIndexa1, tempstata2);
            newDay2.Insert(actionIndexa2, tempstata1);
            rating1 = EvalDay(oldDay1) + EvalDay(oldDay2);
            rating2 = EvalDay(newDay1) + EvalDay(newDay2);
            if (AcceptNewDay(EvalDay(oldDay1) + EvalDay(oldDay2), EvalDay(newDay1) + EvalDay(newDay2), r))
            {
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
            Random r = new Random();
            List<Status> oldDayc1, oldDayc2, newDayc1, newDayc2;
            int dayc1, dayc2;
            Status statc1, statc2, tempstatc1, tempstatc2;
            int actionIndexc1, actionIndexc2;
            int rating1, rating2;
            dayc1 = r.Next(5);
            dayc2 = r.Next(5);
            oldDayc1 = oldState.status[0][dayc1];
            oldDayc2 = oldState.status[1][dayc2];
            newDayc1 = new List<Status>(oldDayc1);
            newDayc2 = new List<Status>(oldDayc2);
            if (newDayc1.Count < 2 || newDayc2.Count < 2) return null;
            // pick two random actions			
            actionIndexc1 = r.Next(oldDayc1.Count - 2);
            actionIndexc2 = r.Next(oldDayc2.Count - 2);
            statc1 = oldDayc1[actionIndexc1];
            statc2 = oldDayc2[actionIndexc2];
            if (actionIndexc1 != 0)
            {
                tempstatc2 = new Status(dayc1, statc2.company, statc2.ordnr);
            }
            else
            {
                tempstatc2 = new Status(dayc1, statc2.company, statc2.ordnr);
            }
            if (actionIndexc2 != 0)
            {
                tempstatc1 = new Status(dayc2, statc1.company, statc1.ordnr);
            }
            else
            {
                tempstatc1 = new Status(dayc2, statc1.company, statc1.ordnr);
            }
            // Swap the actions
            newDayc1.Remove(statc1);
            newDayc2.Remove(statc2);
            newDayc1.Insert(actionIndexc1, tempstatc2);
            newDayc2.Insert(actionIndexc2, tempstatc1);
            rating1 = EvalDay(oldDayc1) + EvalDay(oldDayc2);
            rating2 = EvalDay(newDayc1) + EvalDay(newDayc2);
            if (AcceptNewDay(rating1, rating2, r))
            {
                newState = oldState;
                newState.status[0][dayc1] = newDayc1;
                newState.status[1][dayc2] = newDayc2;
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
            Company previousLoc = DTS.maarheeze;
            GarbageTruck truck = new GarbageTruck();
            // Iterate over all actions
            foreach (Status action in day)
            {
                if (action.ordnr == 0)
                {
                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + DTS.emptyingTime;
                    truck.EmptyTruck();
                    previousLoc = DTS.maarheeze;
                }
                else
                {
                    Order ord = DTS.orders[action.ordnr];
                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + (int)ord.emptyingTime;
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
