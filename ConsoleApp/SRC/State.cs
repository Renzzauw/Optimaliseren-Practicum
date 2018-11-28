using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
    // State object
    public class State : SimulatedAnnealing
    {
        // Variables
        List<Status>[] status1, status2; // The week for each truck: It contains 5 dictionaries, one for each day. Each truck gets their own status
         
		
        // Make a (random) initial state
		public State()
		{
            status1 = MakeRandomState(new GarbageTruck());
            status2 = MakeRandomState(new GarbageTruck());
        }

        public List<Status>[] MakeRandomState(GarbageTruck truck)
        {
            List<Status>[] statusList = new List<Status>[5];
            for(int i = 0; i < 5; i++)
            {
                statusList[i] = MakeRandomDay(truck);
            }

			return statusList;
        }

        public List<Status> MakeRandomDay(GarbageTruck truck)
        {
            Random random = new Random(); // A random that we will use here and there
            List<Status> day = new List<Status>();
            //day.Add(new Status(0, 600, 4, /* Insert haarmeze hier*/ , truck));
            int company;
            while (true)
            {
                company = random.Next(companyList.Length);
            }
        }



    }
    public class Status
    {
        public int startTime, endTime;
        public enum Actione { Driving, Collecting, Emptying, Nothing } // Added the extra E to avoid conflicts with System.Action
        public Actione action;
        public Company company;
        public GarbageTruck truck;

        public Status(int s, int e, int z, Company c, GarbageTruck gt)
        {
            startTime = s;
            endTime   = e;
            switch (z)
            {
                case 1:
                    action = Actione.Driving;
                    break;
                case 2:
                    action = Actione.Collecting;
                    break;
                case 3:
                    action = Actione.Emptying;
                    break;
                case 4:
                    action = Actione.Nothing;
                    break;
                default:
                    break;
            }
            company   = c;
            truck     = gt;
        }
    }

}
