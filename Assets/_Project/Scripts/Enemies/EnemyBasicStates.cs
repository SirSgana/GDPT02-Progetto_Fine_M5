using System.Threading;
using UnityEngine;


//  PRIMO STATO IDLE
public class IdleState : BaseState
{
    private float timer = 5f;

    public IdleState(EnemyController e, FSMController c) : base(e, c) { }

    public override void StateEnter()
    {
        enemy.agent.isStopped = true;

        //Riga aggiunta per leggere SO_EnemyData altrimenti poteva stare anche senza e funzionava per via "Hard coding",
        timer = enemy.stats.rotationInterval; //ma così ora timer equivale ad una stats dell'SO
    }

    public override void StateUpdate()
    {
        
        if (enemy.isDog) return; //Aggiungo la voce temporanea per testare il Dog poi dovrò fare una classe SO Base e utilizzare l'EREDITARIETà

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            enemy.transform.Rotate(0, 180, 0);
            timer = enemy.stats.rotationInterval;
        }
    }
    public override void StateExit() => enemy.agent.isStopped = false;

    public override void CheckTransition()
    {
        if (enemy.CanSeePlayer())
        {
            controller.ChangeState(new ChaseState(enemy, controller));
        }

        if (enemy.isDog)
        {
            controller.ChangeState(new PatrolState(enemy, controller));
        }
    }
}

// SECONDO STATO CHASING
public class ChaseState : BaseState
{
    public ChaseState(EnemyController e, FSMController c) : base(e, c) { }

    public override void StateEnter() { }

    public override void StateUpdate() => enemy.agent.SetDestination(enemy.player.position);

    public override void StateExit() { }

    public override void CheckTransition()
    {
        if (!enemy.CanSeePlayer())
        {
            controller.ChangeState(new LookAroundState(enemy, controller));
        }
    }
}

// TERZO STATO LOOKAROUND
public class LookAroundState : BaseState
{

    private float rotationSpeed; //Inserisco questo float come contenitore per poi agganciarmi all'SO_EnemyData nello StateEnter
    private float waitTimer; //Inserisco questo float come contenitore per poi agganciarmi all'SO_EnemyData nello StateEnter
    private Quaternion originalRotation;
    private Quaternion targetRotation; //Per il calcolo di eulero
    private int lookStep = 0; //0 = Pausa, 1 = Sinistra, 2 = Destra e 3 = Finito //Variabile utile per gestire gli if

    public LookAroundState(EnemyController e, FSMController c) : base(e, c) {}
    
    public override void StateEnter() 
    {

        enemy.agent.isStopped = true;
        originalRotation = enemy.transform.rotation; //Così la sua rotazione 0 sarà l'ultima direzione dove stava il player
        rotationSpeed = enemy.stats.rotationSpeed; //Parametro regolabile dallo scriptable object
        waitTimer = enemy.stats.waitTimer; //Parametro regolabile dallo scriptable object
        lookStep = 0;

        //Calcolo la prima destinazione di rotazione (90 gradi a sinistra)
        targetRotation = originalRotation * Quaternion.Euler(0, -90, 0);
    }

    public override void StateUpdate()
    {
        //Implentato lo switch per migliorare la senzazione che dà l'enemy e farlo sembrare vero cioè: 
        switch (lookStep)
        {
            case 0: //Pausa dove l'orso ha perso il player ed aspetta 1 secondo
                
                waitTimer -= Time.deltaTime;

                if (waitTimer <= 0)
                {
                    //Finito il secondo inizia a guardarsi verso sinistra
                    lookStep = 1;
                    targetRotation = originalRotation * Quaternion.Euler(0, -90, 0);
                }
                break;

            case 1:
                //Questo mi serve per una rotazione graduale
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                //Se l'angolo è quasi arrivato all'obbiettivo di -90° (quel <1) lo considero come finito. Evito blocchi così
                if (Quaternion.Angle(enemy.transform.rotation, targetRotation) < 1f)
                {
                    lookStep = 2;
                    //Calcolo un nuovo obbiettivo di rotazione cioè +90°
                    targetRotation = originalRotation * Quaternion.Euler(0, 90, 0);
                }
                break;

            case 2:
                //Opposto del case 1
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(enemy.transform.rotation, targetRotation) < 1f)
                {
                    lookStep = 3; //Fine della ricerca 
                }
                break;

            case 3: break;

        }
    }

    public override void StateExit() => enemy.agent.isStopped = false;

    public override void CheckTransition()
    {
        if (enemy.CanSeePlayer())
        {
            controller.ChangeState(new ChaseState(enemy, controller));
        }
        else if (lookStep == 3)
        {
            controller.ChangeState(new ReturnState(enemy, controller));
        }
    }
}

// QUARTO STATO RETURN
public class ReturnState : BaseState
{
    private float rotationSpeed;

    public ReturnState(EnemyController e, FSMController c) : base(e, c) { }

    public override void StateEnter()
    {
        enemy.agent.SetDestination(enemy.initialPosition);
        rotationSpeed = enemy.stats.rotationSpeed;
    }

    public override void StateUpdate()
    {
        //Se l'orso è quasi arrivato alla base, inizio a ruotarlo verso la rotazione iniziale
        //Ho implementato questo e la rotationSpeed altrimenti l'enemy all'arrivo della posizione salva una nuova rotazione
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
        {
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.initialRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public override void StateExit() { }

    public override void CheckTransition()
    {
        if (enemy.CanSeePlayer())
        {
            controller.ChangeState(new ChaseState(enemy, controller));
        }

        //Transazione all'idle controllo sia distanza che rotazione
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.2f)
        {
            if (Quaternion.Angle(enemy.transform.rotation, enemy.initialRotation) < 0.5f)
            {
                controller.ChangeState(new IdleState(enemy, controller));
            }
        }
    }

}

//NUOVO STATO PATROL (forse lo potrò togliere con l'EREDITARIETà? ci devo guardare

    public class PatrolState : BaseState
    {
        public PatrolState(EnemyController e, FSMController c) : base(e, c) { }

        public override void StateEnter()
        {
            enemy.agent.isStopped = false;

            //Se non ho impostato nessun waypoint torna in Idle 
            if (enemy.waypoint == null || enemy.waypoint.Length == 0)
            {
                controller.ChangeState(new IdleState(enemy, controller));
            }
        }

        public override void StateUpdate()
        {
            //Muoviti verso il waypoint attuale
            Vector3 target = enemy.waypoint[enemy.currentWaypointIndex].position;
            enemy.agent.SetDestination(target);
        }

        public override void CheckTransition()
        {
            //Se vedo il player cambio stato in Inseguimento
            if (enemy.CanSeePlayer())
            {
                controller.ChangeState(new ChaseState(enemy, controller));
                return;
            }

            //Se ho finito il waypoint passo al prossimo
            if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.waypointThreshold)
            {
                //Incremento l'indice e torna a 0 se finisce l'array
                enemy.currentWaypointIndex = (enemy.currentWaypointIndex + 1) % enemy.waypoint.Length;

                //Torno in idle per una sosta prima del prossimo punto
                controller.ChangeState(new IdleState(enemy, controller));
            }
        }

        public override void StateExit() { }
    }