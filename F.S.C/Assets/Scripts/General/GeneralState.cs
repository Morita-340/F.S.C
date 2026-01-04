using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Enter();  // 状態に入るときの処理
    void Update(); // 毎フレームの処理
    void Exit();   // 状態から出るときの処理
}
public class GeneralState : MonoBehaviour
{}
//基本ステート
public class IdleState : IState
{
    protected GameObject Player;
    protected StateMachine stateMachine;
    public virtual void Enter() => Debug.Log("Idle: Enter");
    public virtual void Update(){/*Debug.Log("Idle: Update");*/}
    public virtual void Exit() => Debug.Log("Idle: Exit");
}
public class MoveState : IState
{
    protected GameObject Player;
    protected StateMachine stateMachine;
    public virtual void Enter() => Debug.Log("Move: Enter");
    public virtual void Update(){/*Debug.Log("Move: Update");*/}
    public virtual void Exit() => Debug.Log("Move: Exit");
}
public class AttackState : IState
{
    protected GameObject Player;
    protected StateMachine stateMachine;
    public virtual void Enter() => Debug.Log("Attack: Enter");
    public virtual void Update(){}// => Debug.Log("Attack: Update");
    public virtual void Exit() => Debug.Log("Attack: Exit");
}
public class DefenceState : IState
{
    protected GameObject Player;
    public virtual void Enter() => Debug.Log("Defence: Enter");
    public virtual void Update(){}// => Debug.Log("Defence: Update"*/);
    public virtual void Exit() => Debug.Log("Defence: Exit");
}
//継承ステートは各機体の制御スクリプト内でしか使用しないので、そこに記入する