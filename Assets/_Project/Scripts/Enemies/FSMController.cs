using UnityEngine;

public class FSMController : MonoBehaviour
{
    private BaseState currentState;

    public void ChangeState(BaseState newState)
    {
        currentState?.StateExit();
        currentState = newState;
        currentState.StateEnter();
    }

    private void Update()
    {
        if (currentState != null)
        {
            currentState.StateUpdate();
            currentState.CheckTransition();
        }
    }
}
