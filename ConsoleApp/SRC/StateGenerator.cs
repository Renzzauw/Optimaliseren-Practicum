﻿using System;
using System.Collections.Generic;

namespace OptimaliserenPracticum
{
    public class StateGenerator
    {
        private State oldState;                              // Each iteration is given an old state, and must return a new state
        private double oldRating;                                      // the evaluation value of the old state
        private double evalValue;
        private Random r;
        private Order ord; // The order that gets selected for a certain operation

        public StateGenerator(State initial)
        {
            r = new Random();
        }

        public State GetNextState(State old)
        {
            oldState = old;
            oldRating = old.GetAllEval() + old.orderScore;
            State returnState = null;
            ord = null;
            // Try one of the successorfunctions, and keep on trying untill one of them returns a successor
            double i = r.NextDouble();
            Diagnostics.IterationsPerMinute++;
            switch (i)
            {
                case double n when n < 0.30: return oldState; //returnState = Remove(); break;
                case double n when 0.30 <= n && n < 0.80: return oldState; //returnState = Add(); break;
                default: returnState = Shift();  break;
            }
            if (returnState == null) return oldState;
            return returnState;
        }

        #region Remove & Helpers
        // Remove a random action on a random day of the schedule of a truck
        public State Remove()
        {
            // Pick a random truck and day
            int truck = r.Next(2);
            int day = r.Next(5);
            List<Status>[] oldStatus = oldState.status[truck];
            // Pick a random day of the week
            if (oldStatus[day].Count < 2) return null; // Return if there is only one order left, aka the emptying
            List<Status> oldDay = oldStatus[day];
            int index = r.Next(oldDay.Count - 1);
            // Remove a random action
            int ordnr = oldDay[index].ordnr;
            if (ordnr == 0) return null; // Return if you try to delete an emptying moment
            ord = DTS.orders[ordnr];
            int prev = DTS.maarheeze;
            if (index > 0) prev = oldDay[index - 1].ordid;
            int next = oldDay[index + 1].ordid;
            // Check for frequency, and add it properly
            if (DTS.orders[ordnr].frequency > 1)
            {
                switch (ord.frequency)
                {
                    case 2: return Remove2(day, truck);
                    case 3: return Remove345();
                    case 4: return Remove345();
                    case 5: return Remove345(); // A frequency of 5 does not happen
                }
            }
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Deletion(oldState.evals[truck][day].time, oldState.evals[truck][day].truckload, prev, next);
            double newRating = oldRating + RemoveRating(truck, day, dayEval);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                oldState.orderScore += 3 * DTS.orders[ordnr].emptyingTime * DTS.orders[ordnr].frequency / 60;
                RemoveSomething(truck, day, index, dayEval, prev, next);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Remove2(int day1, int truck1)
        {
            // Set the variables needed
            int prev1, next1, prev2, next2;
            int day2 = -1;
            int truck2 = 0;
            double dayEval1, dayEval2;
            List<Status> oldDay1, oldDay2;
            int index1, index2;
            // Locate and process the first item
            oldDay1 = oldState.status[truck1][day1];
            index1 = oldDay1.FindIndex(i => i.ordnr == ord.orderNumber);
            prev1 = DTS.maarheeze;
            if (index1 > 0) prev1 = oldDay1[index1 - 1].ordid;
            next1 = oldDay1[index1 + 1].ordid;
            dayEval1 = Deletion(oldState.evals[truck1][day1].time, oldState.evals[truck1][day1].truckload, prev1, next1);
            switch (day1)
            {
                case 0: day2 = 3; break;
                case 1: day2 = 4; break;
                case 3: day2 = 0; break;
                case 4: day2 = 1; break;
                default: return null;
            }
            // Locate the order on the second day
            oldDay2 = oldState.status[truck2][day2];
            index2 = oldDay2.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index2 == -1)
            {
                truck2 = 1;
                oldDay2 = oldState.status[truck2][day2];
                index2 = oldDay2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the second order
            prev2 = DTS.maarheeze;
            if (index2 > 0) prev2 = oldDay2[index2 - 1].ordid;
            next2 = oldDay2[index2 + 1].ordid;
            dayEval2 = Deletion(oldState.evals[truck2][day2].time, oldState.evals[truck2][day2].truckload, prev2, next2);
            double newRating = oldRating + RemoveRating(truck1, day1, dayEval1) + RemoveRating(truck2, day2, dayEval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                oldState.orderScore += 3 * DTS.orders[ord.orderNumber].emptyingTime * 2 / 60; // Frequency is always 2 here
                RemoveSomething(truck1, day1, index1, dayEval1, prev1, next1);
                RemoveSomething(truck2, day2, index2, dayEval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        // This function will look at every day for an index, and remove if one is there
        public State Remove345()
        {
            // Set the variables needed
            int prev1 = 0, next1 = 0, prev2 = 0, next2 = 0, prev3 = 0, next3 = 0, prev4 = 0, next4 = 0, prev5 = 0, next5 = 0;
            int truck1 = 0, truck2 = 0, truck3 = 0, truck4 = 0, truck5 = 0;
            double dayEval1 = 0, dayEval2 = 0, dayEval3 = 0, dayEval4 = 0, dayEval5 = 0;
            List<Status> oldDay1, oldDay2, oldDay3, oldDay4, oldDay5;
            int index1, index2, index3, index4, index5;
            // Locate the order on the first day
            oldDay1 = oldState.status[truck1][0];
            index1 = oldDay1.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index1 == -1)
            {
                truck1 = 1;
                oldDay1 = oldState.status[truck1][0];
                index1 = oldDay1.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // process the first item
            if (index1 != -1)
            {
                oldDay1 = oldState.status[truck1][0];
                index1 = oldDay1.FindIndex(i => i.ordnr == ord.orderNumber);
                prev1 = DTS.maarheeze;
                if (index1 > 0) prev1 = oldDay1[index1 - 1].ordid;
                next1 = oldDay1[index1 + 1].ordid;
                dayEval1 = Deletion(oldState.evals[truck1][0].time, oldState.evals[truck1][0].truckload, prev1, next1);
            }
            // Locate the order on the second day
            oldDay2 = oldState.status[truck2][1];
            index2 = oldDay2.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index2 == -1)
            {
                truck2 = 1;
                oldDay2 = oldState.status[truck2][1];
                index2 = oldDay2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the second order
            if (index2 != -1)
            {
                prev2 = DTS.maarheeze;
                if (index2 > 0) prev2 = oldDay2[index2 - 1].ordid;
                next2 = oldDay2[index2 + 1].ordid;
                dayEval2 = Deletion(oldState.evals[truck2][1].time, oldState.evals[truck2][1].truckload, prev2, next2);
            }
            // Locate the order on the third day
            oldDay3 = oldState.status[truck3][2];
            index3 = oldDay3.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index3 == -1)
            {
                truck3 = 1;
                oldDay3 = oldState.status[truck3][2];
                index3 = oldDay3.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the third order
            if (index3 != -1)
            {
                prev3 = DTS.maarheeze;
                if (index3 > 0) prev3 = oldDay3[index3 - 1].ordid;
                next3 = oldDay3[index3 + 1].ordid;
                dayEval3 = Deletion(oldState.evals[truck3][2].time, oldState.evals[truck3][2].truckload, prev3, next3);
            }
            // Locate the order on the fourth day
            oldDay4 = oldState.status[truck4][3];
            index4 = oldDay4.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index4 == -1)
            {
                truck4 = 1;
                oldDay4 = oldState.status[truck4][3];
                index4 = oldDay4.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the fourth order
            if (index4 != -1)
            {
                prev4 = DTS.maarheeze;
                if (index4 > 0) prev4 = oldDay4[index4 - 1].ordid;
                next4 = oldDay4[index4 + 1].ordid;
                dayEval4 = Deletion(oldState.evals[truck4][3].time, oldState.evals[truck4][3].truckload, prev4, next4);
            }
            // Locate the order on the fifth day
            oldDay5 = oldState.status[truck5][4];
            index5 = oldDay5.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index5 == -1)
            {
                truck5 = 1;
                oldDay5 = oldState.status[truck5][4];
                index5 = oldDay5.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the fifth order
            if (index5 != -1)
            {
                prev5 = DTS.maarheeze;
                if (index5 > 0) prev5 = oldDay5[index5 - 1].ordid;
                next5 = oldDay5[index5 + 1].ordid;
                dayEval5 = Deletion(oldState.evals[truck5][4].time, oldState.evals[truck5][4].truckload, prev5, next5);
            }
            double newRating = oldRating + RemoveRating(truck1, 0, dayEval1) + RemoveRating(truck2, 1, dayEval2) + RemoveRating(truck3, 2, dayEval3) + RemoveRating(truck4, 3, dayEval4) + RemoveRating(truck5, 4, dayEval5);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                oldState.orderScore += 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                if (index1 != -1) RemoveSomething(truck1, 0, index1, dayEval1, prev1, next1);
                if (index2 != -1) RemoveSomething(truck2, 1, index2, dayEval2, prev2, next2);
                if (index3 != -1) RemoveSomething(truck3, 2, index3, dayEval3, prev3, next3);
                if (index4 != -1) RemoveSomething(truck4, 3, index4, dayEval4, prev4, next4);
                if (index5 != -1) RemoveSomething(truck5, 4, index5, dayEval5, prev5, next5);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        #endregion
        #region Add & Helpers
        // Add a random action at a random time
        public State Add()
        {
            if (DTS.availableOrders.Count == 0) return null; // Return when there are no available orders
            // Add a random available order in between two other actions
            ord = DTS.orders[DTS.availableOrders[r.Next(DTS.availableOrders.Count)]];
            // Pick a random truck and day
            int truck = r.Next(2);
            int day = r.Next(5);
            // Check for frequency, handle frequencies higher than 1 seperately
            if (ord.frequency > 1)
            {
                switch (ord.frequency)
                {
                    case 2:  return Add2(day);
                    case 3:  return Add3();
                    case 4:  return Add45(r.Next(5));
                    case 5:  return Add45(-1); // A frequency of 5 does not happen
                }
            }
            List<Status>[] oldStatus = oldState.status[truck];
            // pick a random day of the week          
            List<Status> oldDay = oldStatus[day];
            int index = r.Next(oldDay.Count - 1);
            int prev = DTS.maarheeze;
            if (index > 0) prev = oldDay[index - 1].ordid;
            int next = oldDay[index].ordid;
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Insertion(oldState.evals[truck][day].time, oldState.evals[truck][day].truckload, prev, next);
            double newRating = oldRating + AddRating(truck, day, dayEval);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                oldState.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                AddSomething(truck, day, index, dayEval,prev, next);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Add2(int day1)
        {
            // Return if it wants to add on a wednesday (which is not allowed)
            if (day1 == 2) return null;
            // Set the variables needed
            int prev1, next1, prev2, next2;
            int day2 = -1;
            double dayEval1, dayEval2;
            int truck1 = r.Next(2); int truck2 = r.Next(2);
            List<Status> oldDay1, oldDay2;
            int time1, time2;
            oldDay1 = oldState.status[truck1][day1];
            time1 = r.Next(oldDay1.Count - 1);
            // Calculate the values needed for the first day
            prev1 = DTS.maarheeze;
            if (time1 > 0) prev1 = oldDay1[time1 - 1].ordid;
            next1 = oldDay1[time1].ordid;
            dayEval1 = Insertion(oldState.evals[truck1][day1].time, oldState.evals[truck1][day1].truckload, prev1, next1);
            // Depending on the first day, determine which second day to pick
            switch (day1)
            {
                case 0: day2 = 3; break;
                case 1: day2 = 4; break;
                case 3: day2 = 0; break;
                case 4: day2 = 1; break;
                default: return null;
            }
            // Calculate the values needed for the first day
            time2 = r.Next(oldState.status[truck2][day2].Count);
            oldDay2 = oldState.status[truck2][day2];
            prev2 = DTS.maarheeze;
            if (time2 > 0) prev2 = oldDay2[time2 - 1].ordid;
            next2 = oldDay2[time2].ordid;
            dayEval2 = Insertion(oldState.evals[truck2][day2].time, oldState.evals[truck2][day2].truckload, prev2, next2);
            double newRating = oldRating + AddRating(truck1, day1, dayEval1) + AddRating(truck2, day2, dayEval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                oldState.orderScore -= 3 * ord.emptyingTime * 2 / 60; // Frequency is always 2 here
                AddSomething(truck1, day1, time1, dayEval1, prev1, next1);
                AddSomething(truck2, day2, time2, dayEval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Add3()
        {
            // Set the variables needed
            int prev1, next1, prev2, next2, prev3, next3;
            double dayEval1, dayEval2, dayEval3;
            int truck1 = r.Next(2), truck2 = r.Next(2), truck3 = r.Next(2);
            List<Status> oldDay1, oldDay2, oldDay3;
            int time1, time2, time3;
            // Calculate the values needed for the first day
            oldDay1 = oldState.status[truck1][0];
            time1 = r.Next(oldDay1.Count - 1);
            prev1 = DTS.maarheeze;
            if (time1 > 0) prev1 = oldDay1[time1 - 1].ordid;
            next1 = oldDay1[time1].ordid;
            dayEval1 = Insertion(oldState.evals[truck1][0].time, oldState.evals[truck1][0].truckload, prev1, next1);
            // Calculate the values needed for the second day
            oldDay2 = oldState.status[truck2][2];
            time2 = r.Next(oldDay2.Count);
            prev2 = DTS.maarheeze;
            if (time2 > 0) prev2 = oldDay2[time2 - 1].ordid;
            next2 = oldDay2[time2].ordid;
            dayEval2 = Insertion(oldState.evals[truck2][2].time, oldState.evals[truck2][2].truckload, prev2, next2);
            // Calculate the values needed for the third day
            oldDay3 = oldState.status[truck3][4];
            time3 = r.Next(oldDay3.Count);
            prev3 = DTS.maarheeze;
            if (time3 > 0) prev3 = oldDay3[time3 - 1].ordid;
            next3 = oldDay3[time3].ordid;
            dayEval3 = Insertion(oldState.evals[truck3][4].time, oldState.evals[truck3][4].truckload, prev3, next3);
            double newRating = oldRating + AddRating(truck1, 0, dayEval1) + AddRating(truck2, 2, dayEval2) + AddRating(truck2, 4, dayEval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                oldState.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * 3 / 60; // Frequency is always 3 here
                AddSomething(truck1, 0, time1, dayEval1, prev1, next1);
                AddSomething(truck2, 2, time2, dayEval2, prev2, next2);
                AddSomething(truck3, 4, time3, dayEval3, prev3, next3);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Add45(int skipday)
        {
            // Set the variables needed, We assign default values of zero to avoid errors later
            int prev1 = 0, next1 = 0, prev2 = 0, next2 = 0, prev3 = 0, next3 = 0, prev4 = 0, next4 = 0, prev5 = 0, next5 = 0;
            double dayEval1 = 0, dayEval2 = 0, dayEval3 = 0, dayEval4 = 0, dayEval5 = 0;
            int truck1 = r.Next(2), truck2 = r.Next(2), truck3 = r.Next(2), truck4 = r.Next(2), truck5 = r.Next(2);
            List<Status> oldDay1, oldDay2, oldDay3, oldDay4, oldDay5;
            int time1 = 0, time2 = 0, time3 = 0, time4 = 0, time5 = 0;
            // Skip the day that was given, and calcuate all other days
            // Calculate the values needed for the first day
            if (skipday != 0)
            {
                oldDay1 = oldState.status[truck1][0];
                time1 = r.Next(oldDay1.Count - 1);
                prev1 = DTS.maarheeze;
                if (time1 > 0) prev1 = oldDay1[time1 - 1].ordid;
                next1 = oldDay1[time1].ordid;
                dayEval1 = Insertion(oldState.evals[truck1][0].time, oldState.evals[truck1][0].truckload, prev1, next1);
            }
            // Calculate the values needed for the second day
            if (skipday != 1)
            {
                oldDay2 = oldState.status[truck2][1];
                time2 = r.Next(oldDay2.Count);
                prev2 = DTS.maarheeze;
                if (time2 > 0) prev2 = oldDay2[time2 - 1].ordid;
                next2 = oldDay2[time2].ordid;
                dayEval2 = Insertion(oldState.evals[truck2][1].time, oldState.evals[truck2][1].truckload, prev2, next2);
            }
            // Calculate the values needed for the third day
            if (skipday != 2)
            {
                oldDay3 = oldState.status[truck3][2];
                time3 = r.Next(oldDay3.Count);
                prev3 = DTS.maarheeze;
                if (time3 > 0) prev3 = oldDay3[time3 - 1].ordid;
                next3 = oldDay3[time3].ordid;
                dayEval3 = Insertion(oldState.evals[truck3][2].time, oldState.evals[truck3][2].truckload, prev3, next3);
            }
            // Calculate the values needed for the fourth day
            if (skipday != 3)
            {
                oldDay4 = oldState.status[truck4][3];
                time4 = r.Next(oldDay4.Count);
                prev4 = DTS.maarheeze;
                if (time4 > 0) prev4 = oldDay4[time4 - 1].ordid;
                next4 = oldDay4[time4].ordid;
                dayEval4 = Insertion(oldState.evals[truck4][4].time, oldState.evals[truck4][3].truckload, prev4, next4);
            }
            // Calculate the values needed for the fifth day
            if (skipday != 4)
            {
                oldDay5 = oldState.status[truck5][4];
                time5 = r.Next(oldDay5.Count);
                prev5 = DTS.maarheeze;
                if (time5 > 0) prev5 = oldDay5[time5 - 1].ordid;
                next5 = oldDay5[time5].ordid;
                dayEval5 = Insertion(oldState.evals[truck5][4].time, oldState.evals[truck5][4].truckload, prev5, next5);
            }
            double newRating = oldRating + AddRating(truck1, 0, dayEval1) + AddRating(truck2, 1, dayEval2) + AddRating(truck3, 2, dayEval3) + AddRating(truck4, 3, dayEval4) + AddRating(truck5, 2, dayEval5);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                oldState.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                if (skipday != 0) AddSomething(truck1, 0, time1, dayEval1, prev1, next1);
                if (skipday != 1) AddSomething(truck2, 1, time2, dayEval2, prev2, next2);
                if (skipday != 2) AddSomething(truck3, 2, time3, dayEval3, prev3, next3);
                if (skipday != 3) AddSomething(truck4, 3, time4, dayEval4, prev4, next4);
                if (skipday != 4) AddSomething(truck5, 4, time5, dayEval5, prev5, next5);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }
        #endregion
        /*
        // Swap two random actions within a truck
        public State SwapRandomActionsWithin(object i)
        {
            // Declare all of the necessary variables
            int x = (int)i;
            List<Status>[] oldStatus;
            List<Status> oldDay1, oldDay2, newDay1, newDay2;
            int day1, day2, actionIndex1, actionIndex2, nr1, nr2, id1, id2;
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
            if (stat1.ordnr == 0)
            {
                nr1 = 0;
                id1 = DTS.maarheeze;
            }
            else
            {
                nr1 = stat1.ordnr;
                id1 = stat1.ordid;
            }
            if (stat2.ordnr == 0)
            {
                nr2 = 0;
                id2 = DTS.maarheeze;
            }
            else
            {
                nr2 = stat1.ordnr;
                id2 = stat1.ordid;
            }
            if (nr1 != 0)
            {
                if (DTS.orders[nr1].frequency > 1) return null;
            }
            if (nr2 != 0)
            {
                if (DTS.orders[nr2].frequency > 1) return null;
            }
            // Swap these actions around
            tempstat2 = new Status(day1, id2, nr2);
            tempstat1 = new Status(day2, id1, nr1);
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
                if (!DTS.hasOvertime) DTS.NewBest(newState, rating2);
                return newState;
            }
            return null;
        }
        */
        public State Shift()
        {
            int prev1, next1, prev2, next2;
            // Pick a random truck and day twice
            int truck1 = r.Next(2);
            int truck2 = r.Next(2);
            int day1 = r.Next(5);
            int day2 = r.Next(5);
            if (truck1 == truck2 && day1 == day2) return null; // TODO: Dit werkt nu nog een beetje buggy, maar goed om later te fiksen
            List<Status> oldDay1 = oldState.status[truck1][day1];
            List<Status> oldDay2 = oldState.status[truck2][day2];
            // Return if there is nothing to swap in day 1
            if (oldDay1.Count < 2) return null;
            // pos1 is the index of the order to be swapped, pos2 is the position where to insert the shifted order
            int pos1 = r.Next(oldDay1.Count - 1);
            ord = DTS.orders[oldDay1[pos1].ordnr];
            int pos2 = r.Next(oldDay2.Count - 1);
            // Only allow shifting of an order with a higher frequency when it happens on the same day
            if (ord.frequency > 1 && day1 != day2) return null;
            // Find the previous and next places of both positions
            prev1 = DTS.maarheeze;
            if (pos1 > 0) prev1 = oldDay1[pos1 - 1].ordid;
            next1 = oldDay1[pos1 + 1].ordid;
            prev2 = DTS.maarheeze;
            if (pos2 > 0) prev2 = oldDay2[pos2 - 1].ordid;
            next2 = oldDay2[pos2].ordid;
            // Give ratings to the old and new day, and evaluate them
            double eval1 = Deletion(oldState.evals[truck1][day1].time, oldState.evals[truck1][day1].truckload, prev1, next1);
            double eval2 = Insertion(oldState.evals[truck2][day2].time, oldState.evals[truck2][day2].truckload, prev2, next2);
            double newRating = oldRating + RemoveRating(truck1, day1, eval1) + AddRating(truck2, day2, eval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, return the new state
                RemoveSomething(truck1, day1, pos1, eval1, prev1, next1);
                AddSomething(truck2, day2, pos2, eval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public void RemoveSomething(int truck, int day, int index, double dayEval, int prev, int next)
        {
            oldState.status[truck][day].RemoveAt(index);
            // Adjust the evaluation so that it is correct again
            oldState.evals[truck][day].value = dayEval;
            oldState.evals[truck][day].time += DTS.timeMatrix[prev, next] - (DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next]);
            oldState.evals[truck][day].truckload -= ord.containerCount * ord.volumePerContainer;
        }

        public void AddSomething(int truck, int day, int index, double dayEval, int prev, int next)
        {
            oldState.status[truck][day].Insert(index, new Status(day, ord.matrixID, ord.orderNumber));
            // Adjust the evaluation so that it is correct again
            oldState.evals[truck][day].value = dayEval;
            oldState.evals[truck][day].time += DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next] - DTS.timeMatrix[prev, next];
            oldState.evals[truck][day].truckload += ord.containerCount * ord.volumePerContainer;
        }


        // A function to calculate the new rating of a day, when something was added
        public double AddRating(int truck, int day, double newEval)
        {
            if (newEval == 0) return 0;
            double score = -oldState.evals[truck][day].value;
            score += newEval;
            return score;
        }

        // A function to calculate the new rating of a day, when something was removed
        public double RemoveRating(int truck, int day, double newEval)
        {
            if (newEval == 0) return 0;
            double score = -oldState.evals[truck][day].value;
            score += newEval;
            return score;
        }




        // Adjust parameters for when an order gets inserted, and return the new value
        public double Insertion(double time, int truckload,int prev, int next)
        {
            double newTime = time + DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next] - DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload + ord.containerCount * ord.volumePerContainer);
        }

        // Adjust parameters for when an order gets removed, where curr is the order that was deleted, and return the new value
        public double Deletion(double time, int truckload, int prev, int next)
        {
            double newTime = time - DTS.timeMatrix[prev, ord.matrixID] - ord.emptyingTime - DTS.timeMatrix[ord.matrixID, next] + DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload - ord.containerCount * ord.volumePerContainer);
        }

        // Function that returns whether a new Day, and so, the new state would be accepted
        public bool AcceptNewDay(double oldrating, double newrating)
        {
            if (oldrating > newrating)
            {
                return true;
            }
            return PCheck(oldrating, newrating);
        }

        // Checks if the P is smaller than a random number. Return true if yes.
        public bool PCheck(double fx, double fy)
        {
            evalValue = Math.Pow(Math.E, (- (fy - fx)) / DTS.temperature);
            return evalValue >= r.NextDouble();
        }
    }
}
