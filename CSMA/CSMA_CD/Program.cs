using System;

namespace CSMASimulation
{
    class Program
    {
        static void Main(string[] args)
        {

            int numberOfNodes = 20;
            int totalTicksToSimulate = 200;

            SimulationManager manager = new(numberOfNodes, totalTicksToSimulate);

            manager.Run();
        }
    }
}