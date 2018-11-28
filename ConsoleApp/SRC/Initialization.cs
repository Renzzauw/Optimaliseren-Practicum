using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	class Initialization
    {
        string localPath, distanceName, orderningName;
        public List<Order> OrderList = new List<Order>();

        public Initialization()
        {
            localPath = Directory.GetCurrentDirectory() + '\\';
            distanceName = "dist.txt";
            orderningName = "ord.txt";
        }

        public Company[] makeCompanies()
        {
            int highestIndex = 0;
            List<Order> orderList = new List<Order>();
            foreach (string line in File.ReadLines(localPath + orderningName, Encoding.UTF8))
            {
                string[] words = line.Split(';');
                int ordernumb = int.Parse(words[0]);
                string place = words[1];
                int freq = 1;                                //Dit wordt nog aangepast (met enum van Renzo :^|)
                int ContCount = int.Parse(words[3]);
                int volumCont = int.Parse(words[4]);
                float emptTime = float.Parse(words[5]);
                int matrixID = int.Parse(words[6]);
                int xCoord = int.Parse(words[7]);
                int yCoord = int.Parse(words[8]);
                orderList.Add(new Order(ordernumb, place, freq, ContCount, volumCont, emptTime, matrixID, xCoord, yCoord));
                if (matrixID > highestIndex) highestIndex = matrixID;
            }
            Company[] companylist = new Company[highestIndex + 1];
            foreach (Order order in orderList)
            {
                if (companylist[order.matrixID] == null) 
					// TODO: haal overbodige spaties weg uit plaatsnamen???
                {
                    Company company = new Company(order.matrixID, order.xCoordinate, order.yCoordinate, order.placeName, new List<Order>());
					companylist[order.matrixID] = company;
                }
				else
				{
					companylist[order.matrixID].orders.Add(order);
				}
                
            }
            return companylist;
        }

        // Function that processes all the data from the dist file
        public Dictionary<int, Tuple<List<int>, List<int>>> GetAdjacencyList()
        {
            string[] input = File.ReadAllLines(localPath + distanceName);
            int inputLength = input.Length;
			//int[,] distMatrix = new int[inputLength, inputLength];
			//int[,] timeMatrix = new int[inputLength, inputLength];

			// Create a list with as key the company ID and as value a list of distances to all the other companies
			var adjacencyList = new Dictionary<int, Tuple<List<int>, List<int>>>();
			var distanceList = new Dictionary<int, List<int>>();
			var timeList = new Dictionary<int, List<int>>();

			// Take all lines one by one, and process them
			int start, end, dist, time;
            for (int i = 0; i < inputLength; i++)
            {
				// Split the current input line 
				string[] data = input[i].Split(';');
                start = int.Parse(data[0]);
                end = int.Parse(data[1]);
                dist = int.Parse(data[2]);
                time = int.Parse(data[3]);

				// Create dictionaries for both the distance and time 
				if (distanceList.ContainsKey(start))
				{
					distanceList[start].Add(dist);
				}
				else
				{
					List<int> distances = new List<int>();
					distances.Add(dist);
					distanceList.Add(start, distances);
				}
				if (timeList.ContainsKey(start))
				{
					timeList[start].Add(time);
				}
				else
				{
					List<int> times = new List<int>();
					times.Add(time);
					timeList.Add(start, times);
				}
				//distMatrix[start, end] = dist;
				//timeMatrix[start, end] = time;
			}

			// Combine them in one dictionary
			// TODO: dit kan denk ik veel beter dan nog een keer door alles loopen...
			foreach (int k in distanceList.Keys)
			{
				adjacencyList.Add(k, new Tuple<List<int>, List<int>>(distanceList[k], timeList[k]));
			}

			return adjacencyList; //new Tuple<int[,], int[,]>(distMatrix, timeMatrix);
        }
    }
}
