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
		// Variables
		string localPath, distanceName, orderningName;
        public List<Order> OrderList = new List<Order>();

		// Constructor
        public Initialization()
        {
			localPath = Directory.GetCurrentDirectory() + '\\';// TODO: Even fiksen dat hij in de juiste map zit
			distanceName = "dist.txt";
            orderningName = "ord.txt";
        }

		// Creates a list of all the companies
        public Company[] MakeCompanies()
        {
            int highestIndex = 0;							// Highest matrix index
            List<Order> orderList = new List<Order>();      // List of orders

			// Loop through the list of orders and split the input
			foreach (string line in File.ReadLines(localPath + orderningName, Encoding.UTF8))
            {
                string[] words = line.Split(';');			// The splitted input line
                int ordernumb = int.Parse(words[0]);		// Order number
                string place = words[1].Trim();				// Placename
                int freq = (int)Char.GetNumericValue(words[2][0]);                     // TODO: Dit wordt nog aangepast (met enum van Renzo :^|)
                int ContCount = int.Parse(words[3]);		// Container count
                int volumCont = int.Parse(words[4]);		// Volume container
                float emptTime = (float.Parse(words[5]) * 60f);		// Empty time, DIT IS CONVERTED NAAR SECONDEN
                int matrixID = int.Parse(words[6]);			// Matrix ID
                int xCoord = int.Parse(words[7]);			// X coordinate company
                int yCoord = int.Parse(words[8]);           // Y coordinate company
				// Create an order and add it to the company it belongs to
				orderList.Add(new Order(ordernumb, place, freq, ContCount, volumCont, emptTime, matrixID, xCoord, yCoord));
				// Look for the highest matrix ID
				if (matrixID > highestIndex) highestIndex = matrixID;
            }			
            Company[] companylist = new Company[highestIndex + 1]; // List of companies
			// Loop through all orders
			foreach (Order order in orderList)
            {
				// Current company does not exist, create it and add the order
				if (companylist[order.matrixID] == null) 
                {
                    Company company = new Company(order.matrixID, order.xCoordinate, order.yCoordinate, order.placeName, new List<Order>());
					companylist[order.matrixID] = company;
                }
				// Add the order to the company
				companylist[order.matrixID].orders.Add(order);                
            }
			// Return the list of companies
            return companylist;
        }

        // Function that processes all the data from the dist file
        public Tuple<int[,],int[,]> GetAdjacencyList()
        {
            string[] input = File.ReadAllLines(localPath + distanceName);
			long allLines = input.Length;
            int inputLength = (int)Math.Sqrt(allLines);
			int[,] distMatrix = new int[inputLength, inputLength];
			int[,] timeMatrix = new int[inputLength, inputLength];
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

			// Combine them in one dictionary
			return new Tuple<int[,], int[,]>(distMatrix,timeMatrix);
        }
    }
}
