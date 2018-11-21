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
        string localPath;
        string distPath;
        public Initialization()
        {
            localPath = Directory.GetCurrentDirectory();
            distPath = "dist.txt";
        }

        public Tuple<int[,], int[,]> GetMatrix()
        {
            string[] input = File.ReadAllLines(localPath + distPath);
            int inputLength = input.Length;
            int start, end, dist, time;
            int[,] distMatrix = new int[inputLength, inputLength];
            int[,] timeMatrix = new int[inputLength, inputLength];
            // The dist/time from each company to itself = 0;
            for (int i = 0; i < inputLength; i++)
            {
                distMatrix[i, i] = 0;
                timeMatrix[i, i] = 0;
            }
            // Read each line one by one and process it
            for (int i = 0; i < inputLength; i++)
            {
                string[] line = input[i].Split(';');
                start = int.Parse(line[0]);
                end = int.Parse(line[1]);
                dist = int.Parse(line[2]);
                time = int.Parse(line[3]);
                distMatrix[start, end] = dist;
                timeMatrix[start, end] = time;
            }
            return new Tuple<int[,], int[,]>(distMatrix, timeMatrix);
        }
    }
}
