using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Class for handling input/output of the various neighbour states
	public static class FileHandler
	{
		// Print all the actions to the console
		public static void Print(State state)
		{
			int daycounter = 1;
			// print the path of the first truck
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day1 = state.status1[i];
				List<Status> day2 = state.status2[i];
				// Print the routes in the format of the autochecker
				foreach (Status status in day1)
				{
					Console.WriteLine(1 + "; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
                foreach (Status status in day2)
                {
                    Console.WriteLine(2 + "; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
                    daycounter++;
                }
            }
		}

		// Save a state to a textfile with the current datetime
		public static void SaveState(State state)
		{
			// Create a filename for saving the state with the current time
			string path = Directory.GetCurrentDirectory() + "\\Solutions\\Sol" + DateTime.UtcNow.ToFileTime();
			if (!File.Exists(path))
			{
				StreamWriter sw = File.CreateText(path);
				int daycounter = 1;
				// Write the path of both trucks
				for (int i = 0; i < 5; i++)
				{
					daycounter = 1;
                    List<Status> day1 = state.status1[i];
                    List<Status> day2 = state.status2[i];
                    foreach (Status status in day1)
					{
						// day, starttime, endtime, company, truck number, truck capacity, ordernummer
						sw.WriteLine("{0}; {1}; {2}; {3}",1, status.day, status.company.placeName, status.ordnr);
						daycounter++;
					}
                    foreach (Status status in day2)
                    {
                        // day, starttime, endtime, company, truck number, truck capacity, ordernummer
                        sw.WriteLine("{0}; {1}; {2}; {3}",2, status.day, status.company.placeName, status.ordnr);
                        daycounter++;
                    }
				}
                // Close the streamwriter
                sw.Close();
            }
		}

		// Load a state from a given filepath
		public static State LoadStates(string path)
		{
			State state = new State();
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
				Company comp = CompanyFromName(parts[2]);
				int ordnr = int.Parse(parts[3]);
				// Create a status from the input
				Status status = new Status(day, comp, ordnr);
				// Add the status to the right day and list 
				if (trucknr == 1) status1[day].Add(status);
				else status2[day].Add(status);				
			}
			// Close the filestream
			sr.Close();
			// Fill the state
			state.status1 = status1;
			state.status2 = status2;
			return state;
		}

		// Return a company given its name
		public static Company CompanyFromName(string companyName)
		{
			// Check if any company has the name, if so return it
			foreach (Company c in DTS.companyList)
			{
				if (c.placeName == companyName) return c;
			}
			// Otherwise return null
			return null;
		}
	}
}
