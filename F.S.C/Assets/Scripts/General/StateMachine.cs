using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public void ChangeState(IState newState)
    {
        if(currentState?.GetType() == newState.GetType())return;
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    
    }
    public void Update(){
        currentState?.Update();
    }
}
