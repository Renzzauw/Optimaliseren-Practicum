using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Class for handling input/output of the various neighbour states
	public class FileHandler
	{
		// Print all the actions to the console
		public void Print(State state)
		{
			int daycounter = 1;
			// print the path of the first truck
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day = state.status1[i];
				day.AddRange(state.status2[i]);
				// Print the routes in the format of the autochecker
				foreach (Status status in day)
				{
					Console.WriteLine(status.truck.truckNumber + "; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
			}
		}

		// Save a state to a textfile with the current datetime
		public void SaveState(State state)
		{
			// Create a filename for saving the state with the current time
			string path = Directory.GetCurrentDirectory() + "\\Solutions\\Sol" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
			if (!File.Exists(path))
			{
				StreamWriter sw = File.CreateText(path);
				int daycounter = 1;
				// Write the path of both trucks
				for (int i = 0; i < 5; i++)
				{
					daycounter = 1;
					List<Status> day = state.status1[i];
					day.AddRange(state.status2[i]);
					foreach (Status status in day)
					{
						// day, starttime, endtime, company, truck number, truck capacity, ordernummer
						sw.WriteLine("{0}; {1}; {2}; {3}; {4}; {5}; {6}", status.day, status.beginTime, status.endTime, status.company.placeName, status.truck.truckNumber, status.truck.currentCapacity, status.ordnr);
						daycounter++;
					}

					// Close the streamwriter
					sw.Close();
				}
			}
		}

		// Load a state from a given filepath
		public State LoadStates(string path)
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
				int day = int.Parse(parts[0]);
				int startTime = int.Parse(parts[1]);
				int endTime = int.Parse(parts[2]);
				Company comp = CompanyFromName(parts[3]);
				int trucknr = int.Parse(parts[4]);
				GarbageTruck truck = new GarbageTruck(trucknr, int.Parse(parts[5]));
				int ordnr = int.Parse(parts[6]);
				// Create a status from the input
				Status status = new Status(day, startTime, endTime, comp, truck, ordnr);
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
		public Company CompanyFromName(string companyName)
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
