﻿using System;
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
            localPath = Directory.GetCurrentDirectory();
            distanceName = "dist.txt";
            orderningName = "ord.txt";
        }

        public Company[] makeCompany()
        {
            int highestIndex = 0;
            List<Order> orderList = new List<Order>();
            foreach (string line in File.ReadLines(Directory.GetCurrentDirectory() + "ord.txt", Encoding.UTF8))
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
            Company[] companylist = new Company[highestIndex];
            foreach (Order order in orderList)
            {
                if (companylist[order.matrixID] == null)
                {
                    Company company = new Company(order.matrixID, order.xCoordinate, order.yCoordinate, order.placeName, new List<Order>());
                }
                companylist[order.matrixID].orders.Add(order);
            }
            return companylist;
        }

        // Function that processes all the data from the dist file
        public Tuple<int[,], int[,]> GetMatrix()
        {
            string[] input = File.ReadAllLines(localPath + distanceName);
            int inputLength = input.Length;
            int[,] distMatrix = new int[inputLength, inputLength];
            int[,] timeMatrix = new int[inputLength, inputLength];
            // The distance and time from a destination to itself is 0
            for (int i = 0; i < inputLength; i++)
            {
                distMatrix[i, i] = 0;
                timeMatrix[i, i] = 0;
            }
            // Take all lines one by one, and process them
            int start, end, dist, time;
            for (int i = 0; i < inputLength; i++)
            {
                string[] data = input[i].Split(';');
                start = int.Parse(data[0]);
                end = int.Parse(data[1]);
                dist = int.Parse(data[2]);
                time = int.Parse(data[3]);
                distMatrix[start, end] = dist;
                timeMatrix[start, end] = time;
            }
            return new Tuple<int[,], int[,]>(distMatrix, timeMatrix);
        }
    }
}
