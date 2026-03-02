using UnityEngine;

public abstract class BaseState
{
    protected EnemyController enemy;
    protected FSMController controller;

    public BaseState(EnemyController _enemy, FSMController _controller)
    {
        enemy = _enemy;
        controller = _controller;
    }

    public abstract void StateEnter();      //Esegui all'inizio dello stato
    public abstract void StateUpdate();     // Esegui ad ogni frame
    public abstract void StateExit();       // Esegui all'uscita dello stato
    public abstract void CheckTransition(); // Controlla condizione di cambio stato

}