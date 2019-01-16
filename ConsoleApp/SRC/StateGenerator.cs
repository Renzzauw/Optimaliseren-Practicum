using System;
using System.Collections.Generic;

namespace OptimaliserenPracticum
{
    public class StateGenerator
    {
        private State oldState;                              // Each iteration is given an old state, and must return a new state
        private double oldRating;                                      // the evaluation value of the old state
        private double evalValue;
        private Random r;

        public StateGenerator(State initial)
        {
            r = new Random();
        }

        public State GetNextState(State old)
        {
            oldState = old;
            oldRating = oldState.GetAllEval() + oldState.orderScore;
            State returnState = null;
            // Try one of the successorfunctions, and keep on trying untill one of them returns a successor
            double i = r.NextDouble();
            Diagnostics.IterationsPerMinute++;
            switch (i)
            {
                case double n when n < 0.30: returnState = Remove(); break;
                case double n when 0.30 <= n && n < 0.80: returnState = Add(); break;
                default: return oldState;
                //default: returnState = Shift(); break;
            }
            if (returnState == null) return oldState;
            return returnState;
        }


        // Remove a random action on a random day of the schedule of a truck
        public State Remove()
        {
            // Pick a random truck and day
            int truck = r.Next(2);
            int findDay = r.Next(5);
            List<Status>[] oldStatus = oldState.status[truck];
            // Pick a random day of the week
            if (oldStatus[findDay].Count < 2) return null; // Return if there is only one order left, aka the emptying
            Eval oldDayEval = oldState.evals[truck][findDay];
            List<Status> oldDay = oldStatus[findDay];
            int removedIndex = r.Next(oldDay.Count - 1);
            // Remove a random action
            int ordnr = oldDay[removedIndex].ordnr;
            if (ordnr == 0) return null; // Return if you try to delete an emptying moment
            Order ord = DTS.orders[ordnr];
            int prev = DTS.maarheeze;
            if (removedIndex > 0) prev = oldDay[removedIndex - 1].ordid;
            int next = DTS.maarheeze;
            if (removedIndex < oldDay.Count - 1) next = oldDay[removedIndex + 1].ordid;
            // Check for frequency, and add it properly
            if (DTS.orders[ordnr].frequency > 1)
            //{
            //    newState = RemoveAllOrd(newState, DTS.orders[ordnr], findDay);
            //    if (newState == null)
            //    {
                    return null;
            //    }
            //}*/
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Deletion(oldDayEval.time, oldDayEval.truckload, prev, ord, next);
            double newRating = oldRating + RemoveRating(truck, findDay, dayEval, ord);
            if (AcceptNewDay(oldRating, newRating, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                oldState.orderScore += 3 * DTS.orders[ordnr].emptyingTime * DTS.orders[ordnr].frequency / 60;
                oldState.status[truck][findDay].RemoveAt(removedIndex);
                // Adjust the evaluation so that it is correct again
                oldState.evals[truck][findDay].value = dayEval;
                oldState.evals[truck][findDay].time += DTS.timeMatrix[prev, next] - DTS.timeMatrix[prev, ord.matrixID] - ord.emptyingTime - DTS.timeMatrix[ord.matrixID, next];
                oldState.evals[truck][findDay].truckload = oldDayEval.truckload - ord.containerCount * ord.volumePerContainer;
                if (oldState.evals[truck][findDay].time > DTS.dayEnd)
                {
                    Console.WriteLine("oeps");
                }
                DTS.NewBest(oldState, newRating);
                return oldState;
            }
            return null;
        }

        public State RemoveAllOrd(State state, Order ord, int day)
        {
            int z;
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    z = 0;
                    while (z < state.status[x][y].Count)
                    {
                        if (state.status[x][y][z].ordnr == ord.orderNumber)
                        {
                            state.status[x][y].Remove(state.status[x][y][z]);
                        }
                        z++;
                    }
                }
            }
            return state;
        }

        // Add a random action at a random time
        public State Add()
        {
            if (DTS.availableOrders.Count == 0) return null; // Return when there are no available orders
            // Pick a random truck and day
            int truck = r.Next(2);
            int findDay = r.Next(5);
            List<Status>[] oldStatus = oldState.status[truck];
            // pick a random day of the week          
            Eval oldDayEval = oldState.evals[truck][findDay];
            List<Status> oldDay = oldStatus[findDay];
            int addedIndex = r.Next(oldDay.Count - 1);
            // Add a random available order in between two other actions
            Order ord = DTS.orders[DTS.availableOrders[r.Next(DTS.availableOrders.Count)]];
            int prev = DTS.maarheeze;
            if (addedIndex > 0) prev = oldDay[addedIndex - 1].ordid;
            int next = DTS.maarheeze;
            if (addedIndex < oldDay.Count - 1) next = oldDay[addedIndex].ordid;
            // Check for frequency, and add it properly
            if (ord.frequency > 1)
            //{
            //    newState = AddAllOrd(newState, ord, findDay, r);
            //    if (newState == null)
            //    {
                    return null;
            //    }
            //}
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Insertion(oldDayEval.time, oldDayEval.truckload, prev, ord, next);
            double newRating = oldRating + AddRating(truck, findDay, dayEval, ord);
            if (AcceptNewDay(oldRating, newRating, r))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                oldState.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                oldState.status[truck][findDay].Insert(addedIndex, new Status(findDay, ord.matrixID, ord.orderNumber));
                // Adjust the evaluation so that it is correct again
                oldState.evals[truck][findDay].value = dayEval;
                oldState.evals[truck][findDay].time += DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next] - DTS.timeMatrix[prev, next];
                oldState.evals[truck][findDay].truckload = oldDayEval.truckload + ord.containerCount * ord.volumePerContainer;
                if (oldState.evals[truck][findDay].time > DTS.dayEnd)
                {
                    Console.WriteLine("oeps");
                }
                DTS.NewBest(oldState, newRating);
                return oldState;
            }
            return null;
        }
        /*
        public State AddAllOrd(State state, Order ord, int day, Random r)
        {
            int randomTruck, randomTime, unluckyDay = day;
            switch (ord.frequency)
            {
                case 2:
                    switch (day)
                    {
                        case 0:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][3].Count);
                            newState.status[randomTruck][3].Insert(randomTime, new Status(3, ord.matrixID, ord.orderNumber));
                            break;
                        case 1:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][4].Count);
                            newState.status[randomTruck][4].Insert(randomTime, new Status(4, ord.matrixID, ord.orderNumber));
                            break;
                        case 2:
                            return null;
                        case 3:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][0].Count);
                            newState.status[randomTruck][0].Insert(randomTime, new Status(0, ord.matrixID, ord.orderNumber));
                            break;
                        case 4:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][1].Count);
                            newState.status[randomTruck][1].Insert(randomTime, new Status(1, ord.matrixID, ord.orderNumber));
                            break;
                        default: break;
                    }
                    break;
                case 3:
                    switch (day)
                    {
                        case 0:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][2].Count);
                            newState.status[randomTruck][2].Insert(randomTime, new Status(2, ord.matrixID, ord.orderNumber));
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][4].Count);
                            newState.status[randomTruck][4].Insert(randomTime, new Status(4, ord.matrixID, ord.orderNumber));
                            break;
                        case 1:
                            return null;
                        case 2:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][0].Count);
                            newState.status[randomTruck][0].Insert(randomTime, new Status(0, ord.matrixID, ord.orderNumber));
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][4].Count);
                            newState.status[randomTruck][4].Insert(randomTime, new Status(4, ord.matrixID, ord.orderNumber));
                            break;
                        case 3:
                            return null;
                        case 4:
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][0].Count);
                            newState.status[randomTruck][0].Insert(randomTime, new Status(0, ord.matrixID, ord.orderNumber));
                            randomTruck = r.Next(2);
                            randomTime = r.Next(state.status[randomTruck][2].Count);
                            newState.status[randomTruck][2].Insert(randomTime, new Status(2, ord.matrixID, ord.orderNumber));
                            break;
                        default: break;
                    }
                    break;
                case 4:
                    while (unluckyDay != day) unluckyDay = r.Next(5);
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == unluckyDay || i == day) continue;
                        randomTruck = r.Next(2);
                        randomTime = r.Next(state.status[randomTruck][i].Count);
                        newState.status[randomTruck][i].Insert(randomTime, new Status(i, ord.matrixID, ord.orderNumber));
                    }
                    break;
                case 5:
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == day) continue;
                        randomTruck = r.Next(2);
                        randomTime = r.Next(state.status[randomTruck][i].Count);
                        newState.status[randomTruck][i].Insert(randomTime, new Status(i, ord.matrixID, ord.orderNumber));
                    }
                    break;
                default: break;
            }
            return state;
        }*/
        //// Swap two random actions within a truck
        //public State SwapRandomActionsWithin(object i)
        //{
        //    // Declare all of the necessary variables
        //    int x = (int) i;
        //    List<Status>[] oldStatus;
        //    List<Status> oldDay1, oldDay2, newDay1, newDay2;
        //    int day1, day2, actionIndex1, actionIndex2, nr1, nr2, id1, id2;
        //    double rating1, rating2;
        //    Status stat1, stat2, tempstat1, tempstat2;
        //    oldStatus = oldState.status[x];
        //    // Pick two random days in which to swap
        //    day1 = r.Next(5);
        //    day2 = r.Next(5);
        //    if (day1 == day2) return null; // Return if both days are the same
        //    oldDay1 = oldStatus[day1];
        //    oldDay2 = oldStatus[day2];
        //    newDay1 = new List<Status>(oldDay1);
        //    newDay2 = new List<Status>(oldDay2);
        //    if (newDay1.Count < 2 || newDay2.Count < 2) return null; // Return if either day has only one status, aka the emptying at the end
        //    // pick two random actions			
        //    actionIndex1 = r.Next(oldDay1.Count - 2);
        //    actionIndex2 = r.Next(oldDay2.Count - 2);
        //    stat1 = oldDay1[actionIndex1];
        //    stat2 = oldDay2[actionIndex2];
        //    if (stat1.ordnr == 0)
        //    {
        //        nr1 = 0;
        //        id1 = DTS.maarheeze;
        //    }
        //    else
        //    {
        //        nr1 = stat1.ordnr;
        //        id1 = stat1.ordid;
        //    }
        //    if (stat2.ordnr == 0)
        //    {
        //        nr2 = 0;
        //        id2 = DTS.maarheeze;
        //    }
        //    else
        //    {
        //        nr2 = stat1.ordnr;
        //        id2 = stat1.ordid;
        //    }
        //    if(nr1 != 0)
        //    {
        //        if (DTS.orders[nr1].frequency > 1) return null;
        //    }
        //    if (nr2 != 0)
        //    {
        //        if (DTS.orders[nr2].frequency > 1) return null;
        //    }
        //    // Swap these actions around
        //    tempstat2 = new Status(day1, id2, nr2);
        //    tempstat1 = new Status(day2, id1, nr1);
        //    newDay1.Remove(stat1);
        //    newDay2.Remove(stat2);
        //    newDay1.Insert(actionIndex1, tempstat2);
        //    newDay2.Insert(actionIndex2, tempstat1);
        //    // Already make the the new state, so that it can be properly evaluated
        //    newState = new State(oldState.status);
        //    newState.status[x][day1] = newDay1;
        //    newState.status[x][day2] = newDay2;
        //    // Give ratings to the old and new day, and evaluate them
        //    rating1 = Eval(oldState);
        //    rating2 = Eval(newState);
        //    if (AcceptNewDay(rating1, rating2, r))
        //    {
        //        // If accepted, return the new state
        //        if (!DTS.hasOvertime) DTS.NewBest(newState, rating2);
        //        return newState;
        //    }
        //    return null;
        //}\
        /*
        public State Shift()
        {
            // Pick a random truck and day twice
            int truck1 = r.Next(2);
            int truck2 = r.Next(2);
            int day1Index = r.Next(5);
            int day2Index = r.Next(5);
            // Clone the random days of the random trucks
            List<Status> day1 = new List<Status>(oldState.status[truck1][day1Index]);
            List<Status> day2 = new List<Status>(oldState.status[truck2][day2Index]);
            // order1 is the index of the order to be shifted, order2 is the index to insert the order1 at
            int order1 = r.Next(day1.Count);
            int order2 = r.Next(day2.Count);
            // Do the list stuff
            Status shiftOrd = day1[order1];
            // TODO: verander
            if (shiftOrd.ordnr == 0) return null;
            if (DTS.orders[shiftOrd.ordnr].frequency > 1) return null;
            day1.RemoveAt(order1);
            day2.Insert(order2, shiftOrd);
            // Already make the the new state, so that it can be properly evaluated
            newState = new State(oldState.status);
            newState.status[truck1][day1Index] = day1;
            newState.status[truck2][day2Index] = day2;
            // Give ratings to the old and new day, and evaluate them
            double eval1 = oldState[truck1][day1Index].value + Deletion(truck1, day1Index,;
            double newRating = oldRating += RemoveRating(truck1,day1Index, newEval1,DTS.orders[shiftOrd.ordnr]) += AddRating(truck2, day2Index, newEval2, DTS.orders[shiftOrd.ordnr]);
            if (AcceptNewDay(oldRating, newRating, r))
            {
                // If accepted, return the new state
                if (!DTS.hasOvertime) DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;


        }*/

        // A function to calculate the new rating of a day, when something was added
        public double AddRating(int truck, int day, double newEval, Order added)
        {
            double score = -oldState.evals[truck][day].value;
            score += newEval;
            score -= 3 * added.emptyingTime * added.frequency / 60;
            return score;
        }

        // A function to calculate the new rating of a day, when something was removed
        public double RemoveRating(int truck, int day, double newEval, Order removed)
        {
            double score = -oldState.evals[truck][day].value;
            score += newEval;
            score += 3 * removed.emptyingTime * removed.frequency / 60;
            return score;
        }



        // Adjust parameters for when an order gets inserted, and return the new value
        public double Insertion(double time, int truckload,int prev, Order curr, int next)
        {
            double newTime = time + DTS.timeMatrix[prev, curr.matrixID] + curr.emptyingTime + DTS.timeMatrix[curr.matrixID, next] - DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload + curr.containerCount * curr.volumePerContainer);
        }

        // Adjust parameters for when an order gets removed, where curr is the order that was deleted, and return the new value
        public double Deletion(double time, int truckload, int prev, Order curr, int next)
        {
            double newTime = time - DTS.timeMatrix[prev, curr.matrixID] - curr.emptyingTime - DTS.timeMatrix[curr.matrixID, next] + DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload - curr.containerCount * curr.volumePerContainer);
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(double oldrating, double newrating, Random r)
        {
            if (oldrating > newrating)
            {
                return true;
            }
            return PCheck(oldrating, newrating, r);
        }

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(double fx, double fy, Random r)
        {
            evalValue = Math.Pow(Math.E, (- (fy - fx)) / DTS.temperature);
            return evalValue >= r.NextDouble();
        }
    }
}
