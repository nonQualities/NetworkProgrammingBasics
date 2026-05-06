using System;

namespace CSMASimulation
{
    class Program
    {
        static void Main(string[] args)
        {

            int numberOfNodes = 30;
            int totalTicksToSimulate = 2000;

            SimulationManager manager = new(numberOfNodes, totalTicksToSimulate);

            manager.Run();
        }
    }
}