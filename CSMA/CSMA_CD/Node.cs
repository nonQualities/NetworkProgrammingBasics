using System;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
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

    private readonly MersenneTwister _rng;
    private readonly double _pTransmit = 0.3;

   
    public Node()
    {
        State = NodeState.Sensing;
        CollisionCount = 0;
        BackOffTimer = 0;
        _rng = new MersenneTwister(RandomSeed.Robust());
    }

     public Node(double pTransmit = 0.3)
    {
        State = NodeState.Sensing;
        CollisionCount = 0;
        BackOffTimer = 0;
        
        _rng = new MersenneTwister(RandomSeed.Robust());
        _pTransmit = pTransmit;
    }

    public void OnePersistentTick(ChannelState currentChannelState)
    {
        switch (State, currentChannelState)
        {
            case (NodeState.Sensing, ChannelState.Idle):
                State = NodeState.Transmitting;
                break;

            case (NodeState.Sensing, ChannelState.Busy):
            case (NodeState.Sensing, ChannelState.Collision):
                ExponentialBackOff();
                break;

            case (NodeState.BackingOff, _):
                BackOffTimer--;
                if (BackOffTimer <= 0)
                {
                    State = NodeState.Sensing;
                }
                break;

            case (NodeState.Transmitting, ChannelState.Collision):
                ExponentialBackOff();
                break;

            case (NodeState.Transmitting, ChannelState.Busy):
            case (NodeState.Transmitting, ChannelState.Idle):
                
                CollisionCount = 0;
                State = NodeState.Sensing;
                break;
        }
    }


    public void PPersistentTick(ChannelState currentChannelState)
    {
        switch (State, currentChannelState)
        {
            case (NodeState.Sensing, ChannelState.Idle):
                double chance = _rng.NextDouble();
                if(chance <=_pTransmit)
                    State = NodeState.Transmitting;
                else
                {
                    BackOffTimer = 1; 
                    State = NodeState.BackingOff;
                }
                break;

            case (NodeState.Sensing, ChannelState.Busy):
            case (NodeState.Sensing, ChannelState.Collision):
                
                ExponentialBackOff();
                break;

            case (NodeState.BackingOff, _):
                BackOffTimer--;
                if (BackOffTimer <= 0)
                {
                    State = NodeState.Sensing;
                }
                break;

            case (NodeState.Transmitting, ChannelState.Collision):
                ExponentialBackOff();
                break;

            case (NodeState.Transmitting, ChannelState.Busy):
            case (NodeState.Transmitting, ChannelState.Idle):
                
                CollisionCount = 0;
                State = NodeState.Sensing;
                break;
        }
        
    }
   
    private void ExponentialBackOff()
    {
       State = NodeState.BackingOff;
        CollisionCount++;

        int maxK = Math.Min(CollisionCount, 10);
        int maxContentionWindow = 1 << maxK; 
        
        int R = _rng.Next(0, maxContentionWindow); 

        BackOffTimer = R > 0 ? R : 1;
    }

}