using BasicFrame.FSM;
using Newtonsoft.Json;

public abstract class FSMTransition
{
    public e_Transition Transition { get; set; }

    protected FSMTransition()
    {
        Construct();
    }
    protected abstract void Construct();
    public abstract bool TryTransit();
}