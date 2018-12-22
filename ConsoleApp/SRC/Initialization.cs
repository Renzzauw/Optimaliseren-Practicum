using System;
using System.IO;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

namespace OptimaliserenPracticum
{
	class Initialization
    {
		// Variables
		string localPath, distanceName, orderName; //localPath is the path to the right folder, distanceName and orderName are the names of the files

		// Constructor
        public Initialization()
        {
            // Set all of the path names
			localPath = Directory.GetCurrentDirectory() + '\\';
			distanceName = "dist.txt";
            orderName = "ord.txt";
        }

		// Creates a list of all the companies
        public void MakeOrders()
        {
            Order order;                                    // The order that gets created each iteration

			// Loop through the list of orders and add them to the right datastructures
			foreach (string line in File.ReadLines(localPath + orderName, Encoding.UTF8))
            {
                string[] words = line.Split(';');			           // The splitted input line
                int ordernumb = int.Parse(words[0]);		           // Order number
                string place = words[1].Trim();				           // Placename
                int freq = (int)Char.GetNumericValue(words[2][0]);     // The frequency
                int ContCount = int.Parse(words[3]);		           // Container count
                int volumCont = int.Parse(words[4]);		           // Volume container
                float emptTime = float.Parse(words[5], new CultureInfo("us-US").NumberFormat) * 60f;		   // Empty time, DIT IS CONVERTED NAAR SECONDEN
                int matrixID = int.Parse(words[6]);			           // Matrix ID
                int xCoord = int.Parse(words[7]);			           // X coordinate order
                int yCoord = int.Parse(words[8]);                      // Y coordinate order
                // Create the order and add it to the company it belongs to
                order = new Order(ordernumb, place, freq, ContCount, volumCont, emptTime, matrixID, xCoord, yCoord);
                DTS.orders[order.orderNumber] = order;
                DTS.availableOrders.Add(order.orderNumber);
            }			
        }

        // Function that processes all the data from the dist file
        public Tuple<int[,],int[,]> GetDistTimeMatrix()
        {
            // Read the input file, and declare variables needed for the iteration
            string[] input = File.ReadAllLines(localPath + distanceName);
			int allLines = input.Length;
            int[,] distMatrix, timeMatrix;
            int inputLength = (int)Math.Sqrt(allLines);
			distMatrix = timeMatrix = new int[inputLength, inputLength];
			// Take all lines one by one, and process them
			int start, end, dist, time;
            for (long i = 0; i < allLines; i++)
            {
				// Split the current input line 
				string[] data = input[i].Split(';');
                start = int.Parse(data[0]);
                end = int.Parse(data[1]);
                dist = int.Parse(data[2]);
                time = int.Parse(data[3]);
				distMatrix[start,end] = dist;
				timeMatrix[start,end] = time;
			}
			// Combine them in one tuple
			return new Tuple<int[,], int[,]>(distMatrix,timeMatrix);
        }
        public State LoadState(string name)
        {
            int truck, day, daycounter, ordnr;
            List<Status>[][] status = new List<Status>[2][];
            status[0] = new List<Status>[5];
            status[1] = new List<Status>[5];
            for (int i = 0; i < 5; i++)
            {
                status[0][i] = new List<Status>();
                status[1][i] = new List<Status>();
            }
            foreach (string line in File.ReadLines(Directory.GetCurrentDirectory() + "\\Solutions\\" + name, Encoding.UTF8))
            {
                string[] data = line.Split(';');
                truck = int.Parse(data[0]);
                day = int.Parse(data[1]);
                daycounter = int.Parse(data[2]);
                ordnr = int.Parse(data[3]);
                if (ordnr == 0)
                {
                    status[truck - 1][day].Add(new Status(day, DTS.maarheeze, ordnr));
                }
                else
                {
                    status[truck - 1][day].Add(new Status(day, DTS.orders[ordnr].matrixID, ordnr));
                    DTS.availableOrders.Remove(ordnr);
                }
            }
            return new State(status);
        }
    }
}
