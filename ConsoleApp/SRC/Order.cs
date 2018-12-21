namespace OptimaliserenPracticum
{
	public class Order
	{
        // Variables
        // These variables are the same as the ones that are in the .txt file
        public int orderNumber, frequency, containerCount, volumePerContainer, matrixID, xCoordinate, yCoordinate; 
		public string placeName;
		public float emptyingTime; // Emptyingtime has been adjusted so that it is in seconds
		public bool ordersDone = false; // Bool that checks if the order has been forfilled

		// Constructor, set all of the above variables
		public Order(int orderNumber, string placeName, int frequency, int containerCount, int volumePerContainer, float emptyingTime, int matrixID, int xCoordinate, int yCoordinate)
		{
			this.orderNumber = orderNumber;
			this.placeName = placeName;
			this.frequency = frequency;
			this.containerCount = containerCount;
			this.volumePerContainer = volumePerContainer;
			this.emptyingTime = emptyingTime;
			this.matrixID = matrixID;
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
		}
	}
}
