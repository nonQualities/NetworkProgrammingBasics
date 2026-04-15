public class SimulationManager
{
    private readonly List<Node> _nodes;
    private readonly SharedMedium _medium;
    private int _currentTick;
    private readonly int _maxTicks; //

    public SimulationManager(int nodeCount, int maxTicks)
    {
        _nodes = [];
        _medium = new SharedMedium();
        _currentTick = 0;
        _maxTicks = maxTicks;

        for (int i = 0; i < nodeCount; i++)
        {
            _nodes.Add(new Node()); 
        }
    }

    public void Run()
    {
        Console.CursorVisible = false;
        Console.Clear();
        DrawStaticUI();

   
        while (_currentTick < _maxTicks)
        {
            _medium.PrepareForNewTick();

            foreach (var node in _nodes)
            {
                node.Tick(_medium.CurrentState);
                if (node.State == NodeState.Transmitting)
                {
                    _medium.RegisterTransmission();
                }
            }

           
            _medium.ResolveState();
            RenderDynamicUI();
            _currentTick++;
            Thread.Sleep(300); 
        }

        Console.SetCursorPosition(0, _nodes.Count + 10);
        Console.WriteLine("\nSimulation Complete. Press any key to exit.");
        Console.ReadKey();
    }

    private void DrawStaticUI()
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==================================================");
        Console.WriteLine("        CSMA DISCRETE EVENT SIMULATOR             ");
        Console.WriteLine("==================================================");
        Console.ResetColor();
        
        Console.SetCursorPosition(0, 4);
        Console.WriteLine("SHARED MEDIUM: [                          ]");
        
        Console.SetCursorPosition(0, 6);
        Console.WriteLine("--- NODE STATES ---");
    }

    private void RenderDynamicUI()
    {
        Console.SetCursorPosition(35, 1);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"TICK: {_currentTick:D5}");
        Console.ResetColor();

        Console.SetCursorPosition(16, 4);
        switch (_medium.CurrentState)
        {
            case ChannelState.Idle:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("       IDLE       ");
                break;
            case ChannelState.Busy:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" >> TRANSMITTING <<");
                break;
            case ChannelState.Collision:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" !! COLLISION !!  ");
                break;
        }
        Console.ResetColor();

        int startRow = 8;
        for (int i = 0; i < _nodes.Count; i++)
        {
            Console.SetCursorPosition(0, startRow + i);
            Console.Write($"Node {i:D2}: ");

            switch (_nodes[i].State)
            {
                case NodeState.Sensing:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("SENSING      ");
                    break;
                case NodeState.Transmitting:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("TRANSMITTING ");
                    break;
                case NodeState.BackingOff:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"BACKING OFF {_nodes[i].BackOffTimer}  "); 
                    break;
            }
            Console.ResetColor();
        }


        int statRow = startRow + _nodes.Count + 2;
        Console.SetCursorPosition(0, statRow);
        Console.WriteLine("--- STATISTICS ---");
        Console.SetCursorPosition(0, statRow + 1);
        Console.Write($"Total Collisions: {_medium.TotalCollisions,-5}");
        Console.SetCursorPosition(0, statRow + 2);
        Console.Write($"Successful Tx:    {_medium.SuccessfulPackets,-5}");
    }
}