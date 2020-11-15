using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Events;

public class RangedEnemy : StepUnit
{
    public int ShootStep;

    public int IndicatorSteps;

    public VoidEvent StartPrepareEvent;
    public VoidEvent ShootEvent;
    public VoidEvent ShootHitEvent;
    public VoidEvent ShootDeflectEvent;

    public PlayerAction CounterAction;

    [SerializeField]
    private GameObject Bullet;

    // Start is called before the first frame update
    public override void OnStep()
    {
        base.OnStep();

        Debug.Log("Ranged Enemey On Step");

        if (IsShootStep())
        {
            ShootEvent?.Raise();

            if (GameManager.instance.PreviousAction != CounterAction)
            {
                Debug.Log("DIEE LMAO");
                StartCoroutine(RayAnimation(true));
                
                // DIE BOY 
            } else
            {
                Debug.Log("SHOOT BOOM");
                StartCoroutine(RayAnimation(false));
            }
        }

        if (IsPrepareStep())
        {
            StartPrepareEvent?.Raise();
        }

    }

    IEnumerator RayAnimation(bool IsKill)
    {
        float ToPlayerDuration = GameManager.instance.stepDuration * 0.2f;

        var BulletObject = Instantiate(Bullet, gameObject.transform.position, gameObject.transform.rotation);
        for (var time = 0f; time < ToPlayerDuration; time += Time.deltaTime)
        {
            var NormalizedTime = time / ToPlayerDuration;
            BulletObject.transform.position = Vector3.Lerp(transform.position, GameManager.instance.playerGO.transform.position, NormalizedTime);
            yield return null;
        }

        if (IsKill)
        {
            GameManager.instance.ShouldStep = false;
            Time.timeScale = 0.1f;

            var NewDirection = GameManager.instance.playerGO.transform.position - transform.position;
            var bulletRB = Bullet.GetComponent<Rigidbody>();
            bulletRB.velocity = NewDirection * 0.2f;

            yield return new WaitForSeconds(GameManager.instance.stepDuration * 0.2f);

            Time.timeScale = 1f;
            ShootHitEvent?.Raise();
            GameManager.instance.StartCoroutine(GameManager.instance.KillPlayer());
        } 
        else { 
            BulletObject.transform.position = GameManager.instance.playerGO.transform.position;
            ShootDeflectEvent?.Raise();

            var NewDirection = transform.position - GameManager.instance.playerGO.transform.position;
            var bulletRB = Bullet.GetComponent<Rigidbody>();
            bulletRB.velocity = NewDirection * 0.2f;
        }

        

    }

    public virtual bool IsShootStep() => GameManager.instance.stepCount == ShootStep;


    public virtual bool IsPrepareStep() => GameManager.instance.stepCount == ShootStep - IndicatorSteps;
}
