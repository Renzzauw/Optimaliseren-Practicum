﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaliserenPracticum
{
	// Company object
	public class Company
	{
		// Variables
		public int xCoordinate, yCoordinate;
		public string placeName;
		
		// Constructor
		public Company(int xCoordinate, int yCoordinate, string placeName)
		{
			this.xCoordinate = xCoordinate;
			this.yCoordinate = yCoordinate;
			this.placeName = placeName;
		}
	}
}
