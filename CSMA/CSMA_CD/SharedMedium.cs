
public class SharedMedium
{
    public ChannelState CurrentState { get; private set; } = ChannelState.Idle;
    
    private int _transmittingNodesThisTick = 0; //how many nodes are transmitting in the current tick
    public int TotalCollisions { get; private set; } = 0;
    public int SuccessfulPackets { get; private set; } = 0;

    public void PrepareForNewTick()
    {
        _transmittingNodesThisTick = 0;
    }

   
    public void RegisterTransmission()
    {
        _transmittingNodesThisTick++;
    }

    public void ResolveState()
    {
        if (_transmittingNodesThisTick == 0)
        {
            CurrentState = ChannelState.Idle;
        }
        else if (_transmittingNodesThisTick == 1)
        {
            CurrentState = ChannelState.Busy;
            SuccessfulPackets++; 
        }
        else
        {
            CurrentState = ChannelState.Collision;
            TotalCollisions++;
        }
    }
}