using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
    // State object
    public class State
    {
        // Variables
        Company[] companyList;
        Company hq;
        enum Action {Driving, Emptying, Collecting, Nothing}
        Dictionary<decimal, Status>[] status1, status2; // The week for each truck: It contains 5 dictionaries, one for each day. Each truck gets their own status
        Dictionary<decimal, Status> day; // The status struct keeps track of what the truck is doing at that time moment
		
		// Make a (random) initial state
		public State()
		{
            status1 = status2 = new Dictionary<decimal, Status>[5];

		}


		public void GetCompanies()
		{

		}


	}
    public struct Status
    {
        Action action;
        Company company;
        GarbageTruck truck;
    }

}
