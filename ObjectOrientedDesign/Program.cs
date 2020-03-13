using ObjectOrientedDesign.DeckOfCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectOrientedDesign
{
    class Program
    {
        static void Main(string[] args)
        {
            ElevatorSimulator.ElevatorSimulator elevatorSimulator = new ElevatorSimulator.ElevatorSimulator(12, 2, 1, 2);
            elevatorSimulator.Start();
            elevatorSimulator.CallElevatorFromFloor(1);
            elevatorSimulator.PressButtonInElevator(12);
            elevatorSimulator.CallElevatorFromFloor(5);
            Console.ReadLine();
        }
    }
}
