using System;
using System.Collections.Generic;
using System.IO;

namespace OptimaliserenPracticum
{
    // Class for handling input/output of the various neighbour states
    public static class FileHandler
    {
        // Print all the actions to the console
        public static void Print(List<Status>[][] state)
        {
            int daycounter = 1;
            for (int i = 0; i < 10; i++)
            {
                List<Status> day1 = state[0][i];
                if (i % 2 == 0)
                {
                    daycounter = 1;
                }
                // Print the routes in the format of the autochecker
                foreach (Status status in day1)
                {
                    Console.WriteLine(1 + "; " + (i / 2 + 1) + "; " + daycounter + "; " + status.ordnr);
                    daycounter++;
                }
            }
            // print the path of the second truck
            for (int i = 0; i < 10; i++)
            {
                List<Status> day2 = state[1][i];
                if (i % 2 == 0)
                {
                    daycounter = 1;
                }
                // Print the routes in the format of the autochecker
                foreach (Status status in day2)
                {
                    Console.WriteLine(2 + "; " + (i / 2 + 1) + "; " + daycounter + "; " + status.ordnr);
                    daycounter++;
                }
            }
        }

        // Save a state to a textfile with the current datetime
        public static void SaveState(List<Status>[][] state)
        {
            // Create a filename for saving the state with the current time
            //DateTime time = DateTime.UtcNow.ToLocalTime();
            DateTime time = DateTime.UtcNow.ToLocalTime();
            string filetime = time.Day + "-" + time.Month + "-" + time.Hour + "-" + time.Minute + "-" + time.Second;
            string path = Directory.GetCurrentDirectory() + "\\Solutions\\Sol" + filetime;
            StreamWriter sw = File.CreateText(path);
            int daycounter = 1;
            // Write the path of both trucks
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    daycounter = 1;
                }
                List<Status> day1 = state[0][i];
                foreach (Status status in day1)
                {
                    // day, starttime, endtime, company, truck number, truck capacity, ordernummer
                    sw.WriteLine("{0}; {1}; {2}; {3}", 1, status.day + 1, daycounter, status.ordnr);
                    daycounter++;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    daycounter = 1;
                }
                List<Status> day2 = state[1][i];
                foreach (Status status in day2)
                {
                    // day, starttime, endtime, company, truck number, truck capacity, ordernummer
                    sw.WriteLine("{0}; {1}; {2}; {3}", 2, status.day + 1, daycounter, status.ordnr);
                    daycounter++;
                }
            }
            // Close the streamwriter
            sw.Close();
        }

		// Load a state from a given filepath
		public static State LoadState(string path)
		{
			State state = new State();
            int route = 0;
			// Read all lines from the text file
			StreamReader sr = File.OpenText("Solutions" + '\\' + path);
            string line;
			while ((line = sr.ReadLine()) != null)
			{
				// Split the input in: day, starttime, endtime, company, truck number, truck capacity, ordernummer
				string[] parts = line.Split(new string[] { ";" }, StringSplitOptions.None);
                int trucknr = int.Parse(parts[0]);
                int day = int.Parse(parts[1]) - 1;
				int ordnr = int.Parse(parts[3]);
                // Create a status from the input
                Status status;
                if (ordnr != 0)
                {
                    status = new Status(day, DTS.orders[ordnr].matrixID, ordnr);
                    DTS.availableOrders.Remove(ordnr);
                }
                else
                {
                    status = new Status(day, DTS.maarheeze, ordnr);
                }
                // Add the status to the right day and list 
                if (trucknr == 1) state.status[0][day * 2 + route].Add(status);
				else state.status[1][day * 2 + route].Add(status);
                if (ordnr == 0) route = 1 - route; 
			}
			// Close the filestream
			sr.Close();
            for(int j = 0; j < 5; j++)
            {
                int i = j * 2;
                state.evals[0][j] = new Eval(DTS.CalcDayEval(TimeSpent(state.status[0][i]) + TimeSpent(state.status[0][i + 1]), LoadLoaded(state.status[0][i]), LoadLoaded(state.status[0][i + 1])), TimeSpent(state.status[0][i]) + TimeSpent(state.status[0][i + 1]));
                state.evals[1][j] = new Eval(DTS.CalcDayEval(TimeSpent(state.status[1][i]) + TimeSpent(state.status[1][i + 1]), LoadLoaded(state.status[1][i]), LoadLoaded(state.status[1][i + 1])), TimeSpent(state.status[1][i]) + TimeSpent(state.status[1][i + 1]));
                state.truckloads[0][i] = LoadLoaded(state.status[0][i]);
                state.truckloads[0][i + 1] = LoadLoaded(state.status[0][i + 1]);
                state.truckloads[1][i] = LoadLoaded(state.status[1][i]);
                state.truckloads[1][i + 1] = LoadLoaded(state.status[1][i + 1]);
            }
            DTS.orderScore = 0;
            // Add all of the available order to the total value
            foreach (int x in DTS.availableOrders)
            {
                DTS.orderScore += 3 * DTS.orders[x].emptyingTime * DTS.orders[x].frequency / 60;
            }
            DTS.CopyStatus(state.status);
            DTS.CopyEval(state.evals);
            DTS.bestRating = DTS.GetAllEval(state.evals) + DTS.orderScore;
            return state;
		}

        public static double TimeSpent(List<Status> stats)
        {
            if (stats.Count == 1) return DTS.emptyingTime / 2;
            double time = DTS.timeMatrix[DTS.maarheeze,stats[0].ordid] + DTS.orders[stats[0].ordnr].emptyingTime;
            for(int i = 1; i < stats.Count - 1; i++)
            {
                time += DTS.timeMatrix[stats[i - 1].ordid, stats[i].ordid] + DTS.orders[stats[i].ordnr].emptyingTime;
            }
            time += DTS.timeMatrix[stats[stats.Count - 1].ordid, DTS.maarheeze] + (DTS.emptyingTime / 2);
            return time;
        }

        public static int LoadLoaded(List<Status> stats)
        {
            int load = 0;
            for (int i = 0; i < stats.Count - 1; i++) load += DTS.orders[stats[i].ordnr].containerCount * DTS.orders[stats[i].ordnr].volumePerContainer;
            return load;
        }
	}
}
