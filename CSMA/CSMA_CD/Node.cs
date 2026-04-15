using System;

public enum NodeState
{
    Sensing,
    Transmitting,
    BackingOff
}

public enum ChannelState
{
    Idle,
    Busy,
    Collision
}

public class Node
{
    public NodeState State { get; private set; }
    public int CollisionCount { get; private set; }
    public int BackOffTimer { get; private set; }

    private readonly Random _rng = new();

    public Node()
    {
        State = NodeState.Sensing;
        CollisionCount = 0;
        BackOffTimer = 0;
    }

    public void Tick(ChannelState currentChannelState)
    {
        switch (State, currentChannelState)
        {
            case (NodeState.Sensing, ChannelState.Idle):
                State = NodeState.Transmitting;
                break;

            case (NodeState.Sensing, ChannelState.Busy):
            case (NodeState.Sensing, ChannelState.Collision):
                CalculateExponentialBackoff();
                break;

            case (NodeState.BackingOff, _):
                BackOffTimer--;
                if (BackOffTimer <= 0)
                {
                    State = NodeState.Sensing;
                }
                break;

            case (NodeState.Transmitting, ChannelState.Collision):
                CalculateExponentialBackoff();
                break;

            case (NodeState.Transmitting, ChannelState.Busy):
            case (NodeState.Transmitting, ChannelState.Idle):
                
                CollisionCount = 0;
                State = NodeState.Sensing;
                break;
        }
    }

   
    private void CalculateExponentialBackoff()
    {
        State = NodeState.BackingOff;
        CollisionCount++;

        int maxK = Math.Min(CollisionCount, 10);
        
        int maxContentionWindow = 1 << maxK; //bitwise shift to calculate 2^k
        int R = _rng.Next(0, maxContentionWindow); 

        BackOffTimer = R > 0 ? R : 1; 
    }
}