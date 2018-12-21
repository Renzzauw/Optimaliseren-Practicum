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
        private int check;
        private Random r;

        public StateGenerator(State initial)
        {
            r = new Random();
        }

        public State GetNextState(State old)
        {
            check = 0;
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
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
             
                DTS.availableOrders.Add(orda);
                newState = oldState;
                newState.status[x][findDay] = newDay;
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
            if (AcceptNewDay(EvalDay(oldDay), EvalDay(newDay), r))
            {
                DTS.availableOrders.Remove(ord.orderNumber);
                newState = oldState;
                newState.status[x][findDay] = newDay;
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
            List<Status> oldDaya1, oldDaya2, newDaya1, newDaya2;
            int daya1, daya2;
            Status stata1, stata2, tempstata1, tempstata2;
            int actionIndexa1, actionIndexa2;
            oldStatus = oldState.status[x];
            daya1 = r.Next(5);
            daya2 = r.Next(5);
            oldDaya1 = oldStatus[daya1];
            oldDaya2 = oldStatus[daya2];
            newDaya1 = new List<Status>(oldDaya1);
            newDaya2 = new List<Status>(oldDaya2);
            if (daya1 == daya2) return null;
            if (newDaya1.Count < 2 || newDaya2.Count < 2) return null;
            // pick two random actions			
            actionIndexa1 = r.Next(oldDaya1.Count - 2);
            actionIndexa2 = r.Next(oldDaya2.Count - 2);
            stata1 = oldDaya1[actionIndexa1];
            stata2 = oldDaya2[actionIndexa2];
            // Change times so that they are correct, if there was a different action before
            if (actionIndexa1 != 0)
            {
                tempstata2 = new Status(daya1, stata2.company, stata2.ordnr);
            }
            else
            {
                tempstata2 = new Status(daya1, stata2.company, stata2.ordnr);
            }
            if (actionIndexa2 != 0)
            {
                tempstata1 = new Status(daya2, stata1.company, stata1.ordnr);
            }
            else
            {
                tempstata1 = new Status(daya2, stata1.company, stata1.ordnr);
            }
            // Swap the actions
            newDaya1.Remove(stata1);
            newDaya2.Remove(stata2);
            newDaya1.Insert(actionIndexa1, tempstata2);
            newDaya2.Insert(actionIndexa2, tempstata1);
            if (AcceptNewDay(EvalDay(oldDaya1) + EvalDay(oldDaya2), EvalDay(newDaya1) + EvalDay(newDaya2), r))
            {
                newState = oldState;
                newState.status[x][daya1] = newDaya1;
                newState.status[x][daya2] = newDaya2;
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
            if (AcceptNewDay(EvalDay(oldDayc1) + EvalDay(oldDayc2), EvalDay(newDayc1) + EvalDay(newDayc2), r) && Interlocked.Exchange(ref check, 1) == 0)
            {
                newState = oldState;
                newState.status[0][dayc1] = newDayc1;
                newState.status[1][dayc2] = newDayc2;
                return newState;
            }
            return null;
        }

        
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
                    // More orders on a day is generally better
                    Order ord = DTS.orders[action.ordnr];
                    score += 100;

                    newstart += DTS.timeMatrix[previousLoc.companyIndex, action.company.companyIndex] + (int)ord.emptyingTime;
                    truck.FillTruck(ord);
                    // Check if there's a moment when the truck is full. deduct a lot of score for that
                    if (truck.CheckIfOverloaded()) score -= 1000;
                }
                // See if an order is placed on the wrong day (according to a pattern), punish that
                // TODO: implement this
            }
            //score + de tijd die de checke           
            // Punish overtime pretty heavily
            if (newstart >= DTS.dayEnd) score += DTS.dayEnd - newstart * 5;
            return score;
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(int oldrating, int newrating, Random r)
        {
            return (newrating > oldrating) || PCheck(oldrating, newrating, r);
        }

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(int fx, int fy, Random r)
        {
            if (Math.Sign(fx) == -1 || Math.Sign(fy) == -1) return false;
            double quickmaffs = Math.Pow(Math.E, (fy - fx) / DTS.temperature);
            return quickmaffs >= r.NextDouble();
        }
    }
}
