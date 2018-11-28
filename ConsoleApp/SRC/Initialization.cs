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
			localPath = "d:\\Users\\Renzo\\Documents\\GitHub\\Talen-Compilers-Practica\\Optimaliseren-Practicum\\";//Directory.GetCurrentDirectory() + '\\';

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
                int freq = 1;                                //TODO: Dit wordt nog aangepast (met enum van Renzo :^|)
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
        public Tuple<Dictionary<int,int[]>, Dictionary<int, int[]>> GetAdjacencyList()
        {
            string[] input = File.ReadAllLines(localPath + distanceName);
			long allLines = input.Length;
            int inputLength = (int)Math.Sqrt(allLines);
			//int[,] distMatrix = new int[inputLength, inputLength];
			//int[,] timeMatrix = new int[inputLength, inputLength];

			// Create a list with as key the company ID and as value a list of distances to all the other companies
			var distanceList = new Dictionary<int, int[]>();
			var timeList = new Dictionary< int, int[]> ();

			for(int i = 0; i < inputLength; i++)
			{
				distanceList[i] = new int[inputLength];
				timeList[i] = new int[inputLength];
			}

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
				distanceList[start][end] = dist;
				timeList[start][end] = time;
			}

			// Combine them in one dictionary
			return new Tuple<Dictionary<int, int[]>, Dictionary<int, int[]>>(distanceList,timeList);
        }
    }
}
