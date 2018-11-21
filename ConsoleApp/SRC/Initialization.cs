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

        public Initialization()
        {
            localPath = Directory.GetCurrentDirectory();
            distanceName = "dist.txt";
            orderningName = "ord.txt";
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