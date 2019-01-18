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
        private Order ord; // The order that gets selected for a certain operation

        public StateGenerator(State initial)
        {
            r = new Random();
        }

        public State GetNextState(State old)
        {
            oldState = old;
            oldRating = DTS.GetAllEval(old.evals) + DTS.orderScore;
            State returnState = null;
            ord = null;
            // Try one of the successorfunctions, and keep on trying untill one of them returns a successor
            double i = r.NextDouble();
            Diagnostics.IterationsPerMinute++;
            switch (i)
            {
                case double n when n < 0.30: returnState = Remove(); break;
                case double n when 0.30 <= n && n < 0.80: returnState = Add(); break;
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
            int route = r.Next(10);
            int day = route / 2;
            List<Status>[] oldStatus = oldState.status[truck];
            // Pick a random route
            if (oldStatus[route].Count < 2) return null; // Return if there is only one order left, aka the emptying
            List<Status> oldRoute = oldStatus[route];
            int index = r.Next(oldRoute.Count - 1);
            // Remove a random action
            int ordnr = oldRoute[index].ordnr;
            ord = DTS.orders[ordnr];
            // Check for frequency, and add it properly
            if (DTS.orders[ordnr].frequency > 1)
            {
                switch (ord.frequency)
                {
                    case 2: return Remove2(day, truck, route);
                    case 3: return Remove345();
                    case 4: return Remove345();
                    case 5: return Remove345(); // A frequency of 5 does not happen
                }
            }
            int prev = DTS.maarheeze;
            if (index > 0) prev = oldRoute[index - 1].ordid;
            int next = oldRoute[index + 1].ordid;
            //calculate the otherroute int to get the pairing route of the day
            int otherRoute = route - 1;
            if (route % 2 == 0) otherRoute = route + 1;
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Deletion(oldState.evals[truck][day].time, oldState.truckloads[truck][route], oldState.truckloads[truck][otherRoute], prev, next);          
            double newRating = oldRating + RemoveRating(truck, day, dayEval);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                DTS.orderScore += 3 * DTS.orders[ordnr].emptyingTime * DTS.orders[ordnr].frequency / 60;
                RemoveSomething(truck, day, route, index, dayEval, prev, next);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Remove2(int day1, int truck1, int route1)
        {
            // Set the variables needed
            int prev1, next1, prev2, next2;
            int day2 = -1;
            int truck2 = 0;
            double dayEval1, dayEval2;
            List<Status> oldRoute1, oldRoute2;
            int index1, index2;
            int otherRoute1 = route1 - 1;
            if (route1 % 2 == 0) otherRoute1 = route1 + 1;
            // Locate and process the first item
            oldRoute1 = oldState.status[truck1][route1];
            index1 = oldRoute1.FindIndex(i => i.ordnr == ord.orderNumber);
            prev1 = DTS.maarheeze;
            if (index1 > 0) prev1 = oldRoute1[index1 - 1].ordid;
            next1 = oldRoute1[index1 + 1].ordid;
            dayEval1 = Deletion(oldState.evals[truck1][day1].time, oldState.truckloads[truck1][route1], oldState.truckloads[truck1][otherRoute1], prev1, next1);
            switch (day1)
            {
                case 0: day2 = 3; break;
                case 1: day2 = 4; break;
                case 3: day2 = 0; break;
                case 4: day2 = 1; break;
                default: return null;
            }
            // Locate the order on the second day
            int route2 = day2 * 2;
            int otherRoute2 = route2 + 1;
            oldRoute2 = oldState.status[truck2][route2];
            index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index2 == -1)
            {
                route2 += 1;
                otherRoute2 -= 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index2 == -1)
            {
                route2 -= 1;
                otherRoute2 += 1;
                truck2 = 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index2 == -1)
            {
                route2 += 1;
                otherRoute2 -= 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the second order
            prev2 = DTS.maarheeze;
            if (index2 > 0) prev2 = oldRoute2[index2 - 1].ordid;
            next2 = oldRoute2[index2 + 1].ordid;
            dayEval2 = Deletion(oldState.evals[truck2][day2].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2],  prev2, next2);
            double newRating = oldRating + RemoveRating(truck1, day1, dayEval1) + RemoveRating(truck2, day2, dayEval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                DTS.orderScore += 3 * DTS.orders[ord.orderNumber].emptyingTime * 2 / 60; // Frequency is always 2 here
                RemoveSomething(truck1, day1, route1, index1, dayEval1, prev1, next1);
                RemoveSomething(truck2, day2, route2, index2, dayEval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        // This function will look at every day for an index, and remove if one is there
        public State Remove345()
        {
            // Set the variables neededs
            int prev1 = 0, next1 = 0, prev2 = 0, next2 = 0, prev3 = 0, next3 = 0, prev4 = 0, next4 = 0, prev5 = 0, next5 = 0;
            int truck1 = 0, truck2 = 0, truck3 = 0, truck4 = 0, truck5 = 0;
            double dayEval1 = 0, dayEval2 = 0, dayEval3 = 0, dayEval4 = 0, dayEval5 = 0;
            List<Status> oldRoute1, oldRoute2, oldRoute3, oldRoute4, oldRoute5;
            int route1 = 0, route2 = 2, route3 = 4, route4 = 6, route5 = 8;
            int otherRoute1 = 1, otherRoute2 = 3, otherRoute3 = 5, otherRoute4 = 7, otherRoute5 = 9;
            int index1, index2, index3, index4, index5;
            // Locate the order on the first day
            oldRoute1 = oldState.status[truck1][route1];
            index1 = oldRoute1.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index1 == -1)
            {
                route1 += 1;
                otherRoute1 -= 1;
                oldRoute1 = oldState.status[truck1][route1];
                index1 = oldRoute1.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index1 == -1)
            {
                route1 -= 1;
                otherRoute1 += 1;
                truck1 = 1;
                oldRoute1 = oldState.status[truck1][route1];
                index1 = oldRoute1.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index1 == -1)
            {
                route1 += 1;
                otherRoute1 -= 1;
                oldRoute1 = oldState.status[truck1][route1];
                index1 = oldRoute1.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // process the first item
            if (index1 != -1)
            {
                prev1 = DTS.maarheeze;
                if (index1 > 0) prev1 = oldRoute1[index1 - 1].ordid;
                next1 = oldRoute1[index1 + 1].ordid;
                dayEval1 = Deletion(oldState.evals[truck1][0].time, oldState.truckloads[truck1][route1],oldState.truckloads[truck1][otherRoute1], prev1, next1);
            }
            // Locate the order on the second day
            oldRoute2 = oldState.status[truck2][route2];
            index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index2 == -1)
            {
                route2 += 1;
                otherRoute2 -= 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index2 == -1)
            {
                route2 -= 1;
                otherRoute2 += 1;
                truck2 = 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index2 == -1)
            {
                route2 += 1;
                otherRoute2 -= 1;
                oldRoute2 = oldState.status[truck2][route2];
                index2 = oldRoute2.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the second order
            if (index2 != -1)
            {
                prev2 = DTS.maarheeze;
                if (index2 > 0) prev2 = oldRoute2[index2 - 1].ordid;
                next2 = oldRoute2[index2 + 1].ordid;
                dayEval2 = Deletion(oldState.evals[truck2][1].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2], prev2, next2);
            }
            // Locate the order on the third day
            oldRoute3 = oldState.status[truck3][route3];
            index3 = oldRoute3.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index3 == -1)
            {
                route3 += 1;
                otherRoute3 -= 1;
                oldRoute3 = oldState.status[truck3][route3];
                index3 = oldRoute3.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index3 == -1)
            {
                route3 -= 1;
                otherRoute3 += 1;
                truck3 = 1;
                oldRoute3 = oldState.status[truck3][route3];
                index3 = oldRoute3.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index3 == -1)
            {
                route3 += 1;
                otherRoute3 -= 1;
                oldRoute3 = oldState.status[truck3][route3];
                index3 = oldRoute3.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the third order
            if (index3 != -1)
            {
                prev3 = DTS.maarheeze;
                if (index3 > 0) prev3 = oldRoute3[index3 - 1].ordid;
                next3 = oldRoute3[index3 + 1].ordid;
                dayEval3 = Deletion(oldState.evals[truck3][2].time, oldState.truckloads[truck3][route3], oldState.truckloads[truck3][otherRoute3], prev3, next3);
            }
            // Locate the order on the fourth day
            oldRoute4 = oldState.status[truck4][route4];
            index4 = oldRoute4.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index4 == -1)
            {
                route4 += 1;
                otherRoute4 -= 1;
                oldRoute4 = oldState.status[truck4][route4];
                index4 = oldRoute4.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index4 == -1)
            {
                route4 -= 1;
                otherRoute4 += 1;
                truck4 = 1;
                oldRoute4 = oldState.status[truck4][route4];
                index4 = oldRoute4.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index4 == -1)
            {
                route4 += 1;
                otherRoute4 -= 1;
                oldRoute4 = oldState.status[truck4][route4];
                index4 = oldRoute4.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the fourth order
            if (index4 != -1)
            {
                prev4 = DTS.maarheeze;
                if (index4 > 0) prev4 = oldRoute4[index4 - 1].ordid;
                next4 = oldRoute4[index4 + 1].ordid;
                dayEval4 = Deletion(oldState.evals[truck4][3].time, oldState.truckloads[truck4][route4], oldState.truckloads[truck4][otherRoute4], prev4, next4);
            }
            // Locate the order on the fifth day
            oldRoute5 = oldState.status[truck5][route5];
            index5 = oldRoute5.FindIndex(i => i.ordnr == ord.orderNumber);
            if (index5 == -1)
            {
                route5 += 1;
                otherRoute5 -= 1;
                oldRoute5 = oldState.status[truck5][route5];
                index5 = oldRoute5.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index5 == -1)
            {
                route5 -= 1;
                otherRoute5 += 1;
                truck5 = 1;
                oldRoute5 = oldState.status[truck5][route5];
                index5 = oldRoute5.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            if (index5 == -1)
            {
                route5 += 1;
                otherRoute5 -= 1;
                oldRoute5 = oldState.status[truck5][route5];
                index5 = oldRoute5.FindIndex(i => i.ordnr == ord.orderNumber);
            }
            // Process the fifth order
            if (index5 != -1)
            {
                prev5 = DTS.maarheeze;
                if (index5 > 0) prev5 = oldRoute5[index5 - 1].ordid;
                next5 = oldRoute5[index5 + 1].ordid;
                dayEval5 = Deletion(oldState.evals[truck5][4].time, oldState.truckloads[truck5][route5], oldState.truckloads[truck5][otherRoute5], prev5, next5);
            }
            double newRating = oldRating + RemoveRating(truck1, 0, dayEval1) + RemoveRating(truck2, 1, dayEval2) + RemoveRating(truck3, 2, dayEval3) + RemoveRating(truck4, 3, dayEval4) + RemoveRating(truck5, 4, dayEval5);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Add(ord.orderNumber);
                DTS.orderScore += 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                if (index1 != -1) RemoveSomething(truck1, 0, route1, index1, dayEval1, prev1, next1);
                if (index2 != -1) RemoveSomething(truck2, 1, route2, index2, dayEval2, prev2, next2);
                if (index3 != -1) RemoveSomething(truck3, 2, route3, index3, dayEval3, prev3, next3);
                if (index4 != -1) RemoveSomething(truck4, 3, route4, index4, dayEval4, prev4, next4);
                if (index5 != -1) RemoveSomething(truck5, 4, route5, index5, dayEval5, prev5, next5);
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
            int route = r.Next(10);
            int day = route / 2;
            // Check for frequency, handle frequencies higher than 1 seperately
            if (ord.frequency > 1)
            {
                switch (ord.frequency)
                {
                    case 2:  return Add2(day, route);
                    case 3:  return Add3();
                    case 4:  return Add45(r.Next(5));
                    case 5:  return Add45(-1); // A frequency of 5 does not happen
                }
            }
            List<Status>[] oldStatus = oldState.status[truck];
            //calculate the otherroute int to get the pairing route of the day
            int otherRoute = route - 1;
            if (route % 2 == 0) otherRoute = route + 1;
            // pick a random route of the week          
            List<Status> oldRoute = oldStatus[route];
            int index = r.Next(oldRoute.Count - 1);
            int prev = DTS.maarheeze;
            if (index > 0) prev = oldRoute[index - 1].ordid;
            int next = oldRoute[index].ordid;
            // Give ratings to the old and new day, and evaluate them
            double dayEval = Insertion(oldState.evals[truck][day].time, oldState.truckloads[truck][route], oldState.truckloads[truck][otherRoute], prev, next);
            double newRating = oldRating + AddRating(truck, day, dayEval);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                DTS.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                AddSomething(truck, day, route, index, dayEval,prev, next);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Add2(int day1, int route1)
        {
            // Return if it wants to add on a wednesday (which is not allowed)
            if (day1 == 2) return null;
            // Set the variables needed
            int prev1, next1, prev2, next2;
            int day2 = -1;
            double dayEval1, dayEval2;
            int truck1 = r.Next(2); int truck2 = r.Next(2);
            List<Status> oldRoute1, oldRoute2;
            int time1, time2;
            oldRoute1 = oldState.status[truck1][route1];
            time1 = r.Next(oldRoute1.Count - 1);
            //calculate the otherroute int to get the pairing route of the day
            int otherRoute1 = route1 - 1;
            if (route1 % 2 == 0) otherRoute1 = route1 + 1;
            // Calculate the values needed for the first day
            prev1 = DTS.maarheeze;
            if (time1 > 0) prev1 = oldRoute1[time1 - 1].ordid;
            next1 = oldRoute1[time1].ordid;
            dayEval1 = Insertion(oldState.evals[truck1][day1].time, oldState.truckloads[truck1][route1], oldState.truckloads[truck1][otherRoute1], prev1, next1);
            // Depending on the first day, determine which second day to pick
            switch (day1)
            {
                case 0: day2 = 3; break;
                case 1: day2 = 4; break;
                case 3: day2 = 0; break;
                case 4: day2 = 1; break;
                default: return null;
            }
            // Calculate the values needed for the second day
            int x = r.Next(2);
            int route2 = day2 * 2 + x;
            int otherRoute2 = day2 * 2 + (1 - x);
            oldRoute2 = oldState.status[truck2][route2];
            time2 = r.Next(oldRoute2.Count);
            prev2 = DTS.maarheeze;
            if (time2 > 0) prev2 = oldRoute2[time2 - 1].ordid;
            next2 = oldRoute2[time2].ordid;
            dayEval2 = Insertion(oldState.evals[truck2][day2].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2], prev2, next2);
            double newRating = oldRating + AddRating(truck1, day1, dayEval1) + AddRating(truck2, day2, dayEval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                DTS.orderScore -= 3 * ord.emptyingTime * 2 / 60; // Frequency is always 2 here
                AddSomething(truck1, day1, route1, time1, dayEval1, prev1, next1);
                AddSomething(truck2, day2, route2, time2, dayEval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public State Add3()
        {
            // Set the variables needed
            int prev1, next1, prev2, next2, prev3, next3 ,x;
            double dayEval1, dayEval2, dayEval3;
            int truck1 = r.Next(2), truck2 = r.Next(2), truck3 = r.Next(2);
            List<Status> oldRoute1, oldRoute2, oldRoute3;
            int time1, time2, time3;
            // Calculate the values needed for the first day
            x = r.Next(2);
            int route1 = 0 * 2 + x;
            int otherRoute1 = 0 * 2 + (1 - x);
            oldRoute1 = oldState.status[truck1][route1];
            time1 = r.Next(oldRoute1.Count);
            prev1 = DTS.maarheeze;
            if (time1 > 0) prev1 = oldRoute1[time1 - 1].ordid;
            next1 = oldRoute1[time1].ordid;
            dayEval1 = Insertion(oldState.evals[truck1][0].time, oldState.truckloads[truck1][route1], oldState.truckloads[truck1][otherRoute1], prev1, next1);
            // Calculate the values needed for the second day
            x = r.Next(2);
            int route2 = 2 * 2 + x;
            int otherRoute2 = 2 * 2 + (1 - x);
            oldRoute2 = oldState.status[truck2][route2];
            time2 = r.Next(oldRoute2.Count);
            prev2 = DTS.maarheeze;
            if (time2 > 0) prev2 = oldRoute2[time2 - 1].ordid;
            next2 = oldRoute2[time2].ordid;
            dayEval2 = Insertion(oldState.evals[truck2][2].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2], prev2, next2);
            // Calculate the values needed for the third day
            x = r.Next(2);
            int route3 = 4 * 2 + x;
            int otherRoute3 = 4 * 2 + (1 - x);
            oldRoute3 = oldState.status[truck3][route3];
            time3 = r.Next(oldRoute3.Count);
            prev3 = DTS.maarheeze;
            if (time3 > 0) prev3 = oldRoute3[time3 - 1].ordid;
            next3 = oldRoute3[time3].ordid;
            dayEval3 = Insertion(oldState.evals[truck3][4].time, oldState.truckloads[truck3][route3], oldState.truckloads[truck3][otherRoute3], prev3, next3);
            double newRating = oldRating + AddRating(truck1, 0, dayEval1) + AddRating(truck2, 2, dayEval2) + AddRating(truck3, 4, dayEval3);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                DTS.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * 3 / 60; // Frequency is always 3 here
                AddSomething(truck1, 0, route1, time1, dayEval1, prev1, next1);
                AddSomething(truck2, 2, route2, time2, dayEval2, prev2, next2);
                AddSomething(truck3, 4, route3, time3, dayEval3, prev3, next3);
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
            List<Status> oldRoute1, oldRoute2, oldRoute3, oldRoute4, oldRoute5;
            int time1 = 0, time2 = 0, time3 = 0, time4 = 0, time5 = 0;
            int route1 = 0, route2 = 0, route3 = 0, route4 = 0, route5 = 0;
            int x;
            // Skip the day that was given, and calcuate all other days
            // Calculate the values needed for the first day
            if (skipday != 0)
            {
                x = r.Next(2);
                route1 = 0 * 2 + x;
                int otherRoute1 = 0 * 2 + (1 - x);
                oldRoute1 = oldState.status[truck1][route1];
                time1 = r.Next(oldRoute1.Count);
                prev1 = DTS.maarheeze;
                if (time1 > 0) prev1 = oldRoute1[time1 - 1].ordid;
                next1 = oldRoute1[time1].ordid;
                dayEval1 = Insertion(oldState.evals[truck1][0].time, oldState.truckloads[truck1][route1], oldState.truckloads[truck1][otherRoute1], prev1, next1);
            }
            // Calculate the values needed for the second day
            if (skipday != 1)
            {
                x = r.Next(2);
                route2 = 1 * 2 + x;
                int otherRoute2 = 1 * 2 + (1 - x);
                oldRoute2 = oldState.status[truck2][route2];
                time2 = r.Next(oldRoute2.Count);
                prev2 = DTS.maarheeze;
                if (time2 > 0) prev2 = oldRoute2[time2 - 1].ordid;
                next2 = oldRoute2[time2].ordid;
                dayEval2 = Insertion(oldState.evals[truck2][1].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2], prev2, next2);
            }
            // Calculate the values needed for the third day
            if (skipday != 2)
            {
                x = r.Next(2);
                route3 = 2 * 2 + x;
                int otherRoute3 = 2 * 2 + (1 - x);
                oldRoute3 = oldState.status[truck3][route3];
                time3 = r.Next(oldRoute3.Count);
                prev3 = DTS.maarheeze;
                if (time3 > 0) prev3 = oldRoute3[time3 - 1].ordid;
                next3 = oldRoute3[time3].ordid;
                dayEval3 = Insertion(oldState.evals[truck3][2].time, oldState.truckloads[truck3][route3], oldState.truckloads[truck3][otherRoute3], prev3, next3);
            }
            // Calculate the values needed for the fourth day
            if (skipday != 3)
            {
                x = r.Next(2);
                route4 = 3 * 2 + x;
                int otherRoute4 = 3 * 2 + (1 - x);
                oldRoute4 = oldState.status[truck4][route4];
                time4 = r.Next(oldRoute4.Count);
                prev4 = DTS.maarheeze;
                if (time4 > 0) prev4 = oldRoute4[time4 - 1].ordid;
                next4 = oldRoute4[time4].ordid;
                dayEval4 = Insertion(oldState.evals[truck4][3].time, oldState.truckloads[truck4][route4], oldState.truckloads[truck4][otherRoute4], prev4, next4);
            }
            // Calculate the values needed for the fifth day
            if (skipday != 4)
            {
                x = r.Next(2);
                route5 = 4 * 2 + x;
                int otherRoute5 = 4 * 2 + (1 - x);
                oldRoute5 = oldState.status[truck5][route5];
                time5 = r.Next(oldRoute5.Count);
                prev5 = DTS.maarheeze;
                if (time5 > 0) prev5 = oldRoute5[time5 - 1].ordid;
                next5 = oldRoute5[time5].ordid;
                dayEval5 = Insertion(oldState.evals[truck5][4].time, oldState.truckloads[truck5][route5], oldState.truckloads[truck5][otherRoute5], prev5, next5);
            }
            double newRating = oldRating + AddRating(truck1, 1, dayEval1) + AddRating(truck2, 1, dayEval2) + AddRating(truck3, 2, dayEval3) + AddRating(truck4, 3, dayEval4) + AddRating(truck5, 2, dayEval5);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, adjust the available orders, and return the new state
                DTS.availableOrders.Remove(ord.orderNumber);
                DTS.orderScore -= 3 * DTS.orders[ord.orderNumber].emptyingTime * DTS.orders[ord.orderNumber].frequency / 60;
                if (skipday != 0) AddSomething(truck1, 0, route1, time1, dayEval1, prev1, next1);
                if (skipday != 1) AddSomething(truck2, 1, route2, time2, dayEval2, prev2, next2);
                if (skipday != 2) AddSomething(truck3, 2, route3, time3, dayEval3, prev3, next3);
                if (skipday != 3) AddSomething(truck4, 3, route4, time4, dayEval4, prev4, next4);
                if (skipday != 4) AddSomething(truck5, 4, route5, time5, dayEval5, prev5, next5);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }
        #endregion
       
        public State Shift()
        {
            int prev1, next1, prev2, next2;
            // Pick a random truck and day twice
            int truck1 = r.Next(2);
            int truck2 = r.Next(2);
            int route1 = r.Next(10);
            int route2 = r.Next(10);
            int day1 = route1 / 2;
            int day2 = route2 / 2;

            if (truck1 == truck2 && route1 == route2) return null; // TODO: Dit werkt nu nog een beetje buggy, maar goed om later te fiksen
            List<Status> oldRoute1 = oldState.status[truck1][route1];
            List<Status> oldRoute2 = oldState.status[truck2][route2];
            // Return if there is nothing to swap in day 1
            if (oldRoute1.Count < 2) return null;
            // pos1 is the index of the order to be swapped, pos2 is the position where to insert the shifted order
            int pos1 = r.Next(oldRoute1.Count - 1);
            ord = DTS.orders[oldRoute1[pos1].ordnr];
            int pos2 = r.Next(oldRoute2.Count - 1);
            // Only allow shifting of an order with a higher frequency when it happens on the same day
            if (ord.frequency > 1 && day1 != day2) return null;
            // Find the previous and next places of both positions
            prev1 = DTS.maarheeze;
            if (pos1 > 0) prev1 = oldRoute1[pos1 - 1].ordid;
            next1 = oldRoute1[pos1 + 1].ordid;
            prev2 = DTS.maarheeze;
            if (pos2 > 0) prev2 = oldRoute2[pos2 - 1].ordid;
            next2 = oldRoute2[pos2].ordid;
            // Give ratings to the old and new day, and evaluate them

            int otherRoute1 = route1 - 1;
            if (route1 % 2 == 0) otherRoute1 = route1 + 1;
            // Give ratings to the old and new day, and evaluate them
            double eval1 = Deletion(oldState.evals[truck1][day1].time, oldState.truckloads[truck1][route1], oldState.truckloads[truck1][otherRoute1], prev1, next1);

            int otherRoute2 = route2 - 1;
            if (route2 % 2 == 0) otherRoute2 = route2 + 1;

            double eval2 = Insertion(oldState.evals[truck2][day2].time, oldState.truckloads[truck2][route2], oldState.truckloads[truck2][otherRoute2], prev2, next2);
            double newRating = oldRating + RemoveRating(truck1, day1, eval1) + AddRating(truck2, day2, eval2);
            if (AcceptNewDay(oldRating, newRating))
            {
                // If accepted, return the new state
                RemoveSomething(truck1, day1, route1, pos1, eval1, prev1, next1);
                AddSomething(truck2, day2, route2, pos2, eval2, prev2, next2);
                DTS.NewBest(oldState);
                return oldState;
            }
            return null;
        }

        public void RemoveSomething(int truck, int day, int route, int index, double dayEval, int prev, int next)
        {
            oldState.status[truck][route].RemoveAt(index);
            // Adjust the evaluation so that it is correct again
            oldState.evals[truck][day].value = dayEval;
            oldState.evals[truck][day].time += DTS.timeMatrix[prev, next] - (DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next]);
            oldState.truckloads[truck][route] -= ord.containerCount * ord.volumePerContainer;
        }

        public void AddSomething(int truck, int day, int route, int index, double dayEval, int prev, int next)
        {
            oldState.status[truck][route].Insert(index, new Status(day, ord.matrixID, ord.orderNumber));
            // Adjust the evaluation so that it is correct again
            oldState.evals[truck][day].value = dayEval;
            oldState.evals[truck][day].time -= DTS.timeMatrix[prev, next] - (DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next]);
            oldState.truckloads[truck][route] += ord.containerCount * ord.volumePerContainer;
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
        public double Insertion(double time, int truckload1, int truckload2,int prev, int next)
        {
            double newTime = time + DTS.timeMatrix[prev, ord.matrixID] + ord.emptyingTime + DTS.timeMatrix[ord.matrixID, next] - DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload1 + ord.containerCount * ord.volumePerContainer, truckload2);
        }

        // Adjust parameters for when an order gets removed, where curr is the order that was deleted, and return the new value
        public double Deletion(double time, int truckload1, int truckload2, int prev, int next)
        {
            double newTime = time - DTS.timeMatrix[prev, ord.matrixID] - ord.emptyingTime - DTS.timeMatrix[ord.matrixID, next] + DTS.timeMatrix[prev, next];
            return DTS.CalcDayEval(newTime, truckload1 - ord.containerCount * ord.volumePerContainer, truckload2);
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
