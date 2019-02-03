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
		public static State LoadStates(string path)
		{
			State state = new State(false);
			List<Status>[] status1 = new List<Status>[5];
			List<Status>[] status2 = new List<Status>[5];
			
			// Read all lines from the text file
			StreamReader sr = File.OpenText(path);			
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				// Split the input in: day, starttime, endtime, company, truck number, truck capacity, ordernummer
				string[] parts = line.Split(new string[] { " ;" }, StringSplitOptions.None);
                int trucknr = int.Parse(parts[0]);
                int day = int.Parse(parts[1]);
				int ordid = int.Parse(parts[2]);
				int ordnr = int.Parse(parts[3]);
				// Create a status from the input
				Status status = new Status(day, ordid, ordnr);
				// Add the status to the right day and list 
				if (trucknr == 1) status1[day].Add(status);
				else status2[day].Add(status);				
			}
			// Close the filestream
			sr.Close();
			// Fill the state
			state.status[0] = status1;
			state.status[1] = status2;
			return state;
		}
	}
}
