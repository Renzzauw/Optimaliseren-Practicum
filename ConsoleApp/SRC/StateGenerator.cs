using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	public class StateGenerator
	{
		public State removedActionState1, removedActionState2, // States for removing an action for each truck
					 AddedActionState1, AddedActionState2,     // States for adding an action for both trucks
					 SwappedWithinState1, SwappedWithinState2, // States for swapping two actions within both trucks
					 SwappedBetweenState;					   // State for swapping two actions between the two trucks

		public StateGenerator(State oldState)
		{
			// Create the neighbour states based on the passed old state
			removedActionState1 = oldState;
			removedActionState2 = oldState;
			AddedActionState1 = oldState;
			AddedActionState2 = oldState;
			SwappedWithinState1 = oldState;
			SwappedWithinState2 = oldState;
			SwappedBetweenState = oldState;
			// Change actions of the neighbour states
			CreateNeighbourStates(oldState);
		}

		private void CreateNeighbourStates(State oldState)
		{
			List<Status>[] status1, status2;
			status1 = oldState.status1;
			status2 = oldState.status2;
			removedActionState1.RemoveRandomAction(status1);
			removedActionState2.RemoveRandomAction(status2);
			//AddedActionState. TODO: nog implementeren
			SwappedWithinState1.SwapRandomActionsWithin(status1);
			SwappedWithinState2.SwapRandomActionsWithin(status2);
			SwappedBetweenState.SwapRandomActionsBetween(status1, status2);
		}

		

	}
}
