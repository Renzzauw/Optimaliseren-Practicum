using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Enumerator
	enum CollectionCounts { Never, Once, Twice };
    // maandag - donderdag
    // dinsdag - vrijdag

	// Company object
	public class Company
	{
		// Variables
		public int companyIndex;				// Identifier for this company
		public int xCoordinate, yCoordinate;    // Position of the company
		public string placeName;				// Name of the place where this company is
		public List<Order> orders;              // The list of orders of the company
        public int visitDays = 0;               // Binary representation of the days the company has been visited 
        public byte visitCount = 0;             // Amount of times the company has been visited
        private Random r;                       // Random that is used for picking an order

        // Constructor
        public Company(int companyIndex, int xCoordinate, int yCoordinate, string placeName, List<Order> orders)
		{
			this.companyIndex = companyIndex;
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
			this.placeName = placeName;
			this.orders = orders;
            r = new Random();
		}

        public bool HasOrders()
        {
            foreach (Order order in orders)
            {
                if (!order.ordersDone && order.frequency == 1) return true;
            }
            return false;
        }

        public Order RandomOrder()
        {
            int rand;
            Order ord;
            while (true) {
                rand = r.Next(orders.Count);
                ord = orders[rand];
                    //VERWIJDER ord.frequency > 1 RAAR.
                if (ord.ordersDone || ord.frequency > 1) continue;
                return ord;
            }
        }
        // When visiting the company, set the bit of this day to 1
        // dayIndex: monday = 0, tuesday = 1, ..., friday = 4
        public void VisitCompany(int dayIndex)
        {
            // Shift dayIndex to the right position (shift fully to the left first, then to the right by the dayIndex)
            int bit = 16 >> dayIndex;
            // Perform OR operation to set the bit to true
            visitDays = visitDays | bit;
            // Increment visit counter
            visitCount++;
        }

        // Check if the company has been visited on a day
        public bool IsDayVisited(int dayIndex)
        {
            int checker = visitDays >> dayIndex;
            return !(checker % 2 == 0);
        }

        public int getCIndex()
        {
            if (companyIndex == 287) return 0;
            else return companyIndex + 1;
        }





    }
}
