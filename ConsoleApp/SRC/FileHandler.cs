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
						sw.WriteLine("1; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
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
						sw.WriteLine("2; " + (i + 1) + "; " + daycounter + "; " + status.ordnr);
						daycounter++;						
					}
				}
				// Close the streamwriter
				sw.Close();
			}	
		}

		public State LoadState(string path)
		{
			State state = new State();
			StreamReader sr = File.OpenText(path);
			GarbageTruck truck1 = new GarbageTruck();
			GarbageTruck truck2 = new GarbageTruck();
			// Read all lines from the text file
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				Status status = new Status();
				string[] parts = line.Split(new string[] { " ;" }, StringSplitOptions.None);

			}



		}
	}
}
