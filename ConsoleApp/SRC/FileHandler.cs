using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	public class FileHandler
	{
		public void Print(State state)
		{
			int daycounter = 1;
			// print the path of the first truck
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day = state.status1[i];
				foreach (Status status in day)
				{
					Console.WriteLine("1; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
			}
			// print the path of the second truck
			// TODO: losse functie
			for (int i = 0; i < 5; i++)
			{
				daycounter = 1;
				List<Status> day = state.status2[i];
				foreach (Status status in day)
				{					
					Console.WriteLine("2; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
					daycounter++;
				}
			}
		}

		public void SaveState(State state)
		{
			// Create a filename for saving the state with the current time
			string path = Directory.GetCurrentDirectory() + "\\Solutions\\Sol" + DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
			if (!File.Exists(path))
			{
				StreamWriter sw = File.CreateText(path);

				int daycounter = 1;
				// Write the path of the first truck
				for (int i = 0; i < 5; i++)
				{
					daycounter = 1;
					List<Status> day = state.status1[i];
					foreach (Status status in day)
					{
						// day, starttime, endtime, company, truck capacity, truck number, ordernummer
						sw.WriteLine("{0}; {1}; {2}; {3}; {4}; {5}", status.day, status.startTime, status.endTime, status.company, status.truck.currentCapacity, status.ordnr);
						daycounter++;						
					}
				}
				// Write the path of the second truck
				for (int i = 0; i < 5; i++)
				{
					daycounter = 1;
					List<Status> day = state.status2[i];
					foreach (Status status in day)
					{
						// day, starttime, endtime, company, truck capacity, truck number, ordernummer
						sw.WriteLine("{0}; {1}; {2}; {3}; {4}; {5}", status.day, status.startTime, status.endTime, status.company, status.truck.currentCapacity, status.ordnr);
						daycounter++;						
					}
				}
				// Close the streamwriter
				sw.Close();
			}	
		}

		public State LoadStates(string path)
		{
			State state = new State();
			List<Status>[] states = new List<Status>[5]; 
			StreamReader sr = File.OpenText(path);
			// Read all lines from the text file
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				// day, starttime, endtime, company, truck capacity, truck number, ordernummer
				string[] parts = line.Split(new string[] { " ;" }, StringSplitOptions.None);
				// TODO: checken of de volgorde hier nog klopt
				Status status = new Status(int.Parse(parts[0]), int.Parse(parts[1]), CompanyFromName(parts[2]), new GarbageTruck(int.Parse(parts[3])), int.Parse(parts[4]), int.Parse(parts[5]));
				//states[int.Parse(parts[0])].Add(status);
				//state.
			}



		}

		// 
		public Company CompanyFromName(string companyName)
		{
			// Check if any company has the name, if so return it
			foreach (Company c in Datastructures.companyList)
			{
				if (c.placeName == companyName)
				{
					return c;
				}
			}
			// Otherwise return null
			return null;
		}
	}
}
