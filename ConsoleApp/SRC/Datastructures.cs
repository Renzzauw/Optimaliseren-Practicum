using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Global class for important data
	public static class DTS
	{
		public static Company[] companyList;        // List of companies
		public static int[,] distanceMatrix;        // Distance matrix
		public static int[,] timeMatrix;            // Time matrix
        public static int dayStart, dayEnd;
        public static int emptyingTime;
		public static Company maarheeze;
		public static Dictionary<int, Order> orders;
        public static Dictionary<int, Order> availableOrders;
        public static float temperature;

	}
}
