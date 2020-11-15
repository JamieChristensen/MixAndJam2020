using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using GameJam.Events;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Cinemachine;


public class GameManager : SerializedMonoBehaviour
{

    public static GameManager instance;

    public FeedbackCanvas CountdownCanvas;

    public bool ShouldStep;

    public CinemachineVirtualCamera c;
    public Transform playerHips;


    [BoxGroup("Dependencies")]
    [SerializeField]
    private GridManager gridManager;

    [BoxGroup("Player stuff")]
    private int2 currentPlayerPosition;

    [BoxGroup("Step Timer")]
    [Header("Step related stuff")]
    [Tooltip("The current step in this round. For score, eventually.")]
    [ReadOnly]
    public int stepCount;

    [BoxGroup("Step Timer")]
    [SerializeField]
    private float2 stepInputInterval;

    [BoxGroup("Step Timer")]
    [SerializeField]
    public float stepDuration;

    [BoxGroup("Step Timer")]
    [ReadOnly]
    [ShowInInspector]
    private float stepTimer;

    [BoxGroup("Step Timer")]
    [ShowInInspector]
    [ReadOnly]
    private bool hasRaisedStepEventThisStep;

    [BoxGroup("Player stuff")]
    [ReadOnly]
    public int currentPathPosIndex;

    [BoxGroup("Player stuff")]
    public PlayerAction defaultAction;

    [Header("Player and Units")]
    [BoxGroup("Player stuff")]
    [SerializeField]
    public GameObject playerGO;

    [ReadOnly]
    [HideInEditorMode]
    public List<StepUnit> stepUnits;

    [Header("Events")]
    [BoxGroup("Step Timer")]
    public VoidEvent managerStepEvent;

    public PlayerAction PreviousAction;

    bool HasDonePlayerAction = false;
    bool hasFinishedInit = false;
    [SerializeField]
    float timeToWaitBeforeInit;

    [SerializeField]
    private LineRenderer playerCellVisualizer;

    [SerializeField]
    public MeleeEnemy meleeEnemyPrefab;

    [SerializeField]
    public RangedEnemy rangedEnemyPrefab;

    [Header("Ragdoll info")]
    [SerializeField]
    public List<Collider> ragdollParts = new List<Collider>();
    public Vector3 ragdollForce = new Vector3(0, 100, 0);

    [Header("Events")]
    [SerializeField]
    private VoidEvent winEvent;
    [SerializeField]
    private VoidEvent playerDeathEvent;
    bool playerIsAlive = true;

    bool hasFinishedAudioAndCountdown = false;
    [SerializeField]
    private VoidEvent StartIntroSequenceEvent;
    [SerializeField]
    [Range(4, 20)]
    private float introLength;

    public AudioManager audioManager;


    private bool isReloading = false;
    private void Start()
    {
        hasFinishedInit = false;
        ShouldStep = true;

        Vector3 cellPos = gridManager.Path[0].transform.position;
        playerGO.transform.position = new Vector3(cellPos.x, 0, cellPos.z);
    }

    private void Init()
    {

        //Get level-data and units.
        hasRaisedStepEventThisStep = false;

        stepUnits.Clear();
        stepUnits.AddRange(FindObjectsOfType<StepUnit>());


        //MovePlayerToNextPointOnPath();

    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        playerGO = GameObject.FindGameObjectWithTag("Player");
        SetRagdollParts();
        audioManager = FindObjectOfType<AudioManager>();

    }


    IEnumerator IntroSequence(float length)
    {
        int current = gridManager.LevelManager.CurrentLevel;

        audioManager.PlayCurrentClipStory(current);
        float waitTime = audioManager.speechClips[current].length;



        yield return new WaitForSeconds(waitTime);

        CountdownCanvas.SetText("3");
        CountdownCanvas.ActivateFeedback();
        yield return new WaitForSeconds(1f);

        CountdownCanvas.SetText("2");
        CountdownCanvas.ActivateFeedback();
        yield return new WaitForSeconds(1f);

        CountdownCanvas.SetText("1");
        CountdownCanvas.ActivateFeedback();
        yield return new WaitForSeconds(1f);

        CountdownCanvas.SetText("GO");
        CountdownCanvas.ActivateFeedback();

        hasFinishedAudioAndCountdown = true;
        audioManager.PlaySelectMusicTrack(current);

    }

    IEnumerator WaitAndReloadScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime;
            Time.timeScale = Mathf.Clamp01(Time.timeScale);
        }

        if (!playerIsAlive)
        {
            c.Follow = playerHips;
            c.LookAt = playerHips;
            playerDeathEvent.Raise();

            if (isReloading)
            {
                return;
            }
            StartCoroutine(WaitAndReloadScene(5f));
            return;
        }
        if (ShouldStep)
        {
            if (!hasFinishedInit)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= timeToWaitBeforeInit)
                {
                    Init();
                    Vector3 newPos = gridManager.Path[currentPathPosIndex + 1].transform.position;
                    newPos.y = 0;
                    StartCoroutine(LerpRotationToNextDestination(newPos));
                    hasFinishedInit = true;
                    StartIntroSequenceEvent?.Raise();
                    StartCoroutine(IntroSequence(introLength));
                    stepTimer = 0;
                }
                else
                {
                    return;
                }
            }

            if (!hasFinishedAudioAndCountdown)
            {
                return;
            }

            //GetPlayerInput and assign to recentInput:
            stepTimer += Time.deltaTime;
            UpdateCellVisualizer();

            if (stepTimer > stepDuration - stepInputInterval.x)
            {
                if (stepTimer >= stepDuration && !hasRaisedStepEventThisStep)
                {
                    //Debug.Log("In here");
                    managerStepEvent.Raise();
                    hasRaisedStepEventThisStep = true;
                }

                //Grace period for Input to player:
                if (stepTimer >= stepDuration + stepInputInterval.y && !HasDonePlayerAction)
                {
                    Debug.Log("Doing base action");

                    PlayerStep(defaultAction);
                    return;
                }

                if (stepTimer > stepDuration && HasDonePlayerAction)
                {
                    //stepTimer = stepDuration - stepTimer; //Instead of setting to 0, which would cause minor beat-offsets over time.
                    stepTimer -= stepDuration;
                    stepCount++;
                    hasRaisedStepEventThisStep = false;
                    HasDonePlayerAction = false;

                }
            }

        }
    }

    public IEnumerator KillPlayer()
    {
        playerIsAlive = false;
        stepTimer = -100000;
        TurnOnRagdoll(ragdollForce, playerCellVisualizer.gameObject.transform.position);
        yield break;
    }


    private void UpdateCellVisualizer()
    {
        if (playerCellVisualizer == null)
        {
            playerCellVisualizer = GetComponentInChildren<LineRenderer>();
        }

        Cell activeCell = CellPlayerIsOn();

        Vector3[] points = new Vector3[4];
        Bounds bounds = activeCell.GetComponent<MeshRenderer>().bounds;

        Vector3 center = new Vector3(playerGO.transform.position.x, 0f, playerGO.transform.position.z);
        float t = Map(Mathf.Clamp(stepTimer, 0, stepDuration), 0 - stepInputInterval.x, stepDuration, 0, 1);

        t = Mathf.Clamp(t, 0.1f, 1);

        float tWidth = Mathf.Clamp(1f - t, 0.3f, 0.6f);
        playerCellVisualizer.startWidth = tWidth;
        playerCellVisualizer.endWidth = tWidth;

        var offset = new Vector3(playerGO.transform.position.x, 0, playerGO.transform.position.z);



        points[0] = offset + new Vector3(bounds.max.x - bounds.center.x, 0f, bounds.max.z - bounds.center.z);
        points[0] = Vector3.Lerp(points[0], center, t);
        points[1] = offset + new Vector3(bounds.max.x - bounds.center.x, 0f, bounds.min.z - bounds.center.z);
        points[1] = Vector3.Lerp(points[1], center, t);
        points[2] = offset + new Vector3(bounds.min.x - bounds.center.x, 0f, bounds.min.z - bounds.center.z);
        points[2] = Vector3.Lerp(points[2], center, t);
        points[3] = offset + new Vector3(bounds.min.x - bounds.center.x, 0f, bounds.max.z - bounds.center.z);
        points[3] = Vector3.Lerp(points[3], center, t);

        playerCellVisualizer?.SetPositions(points);
    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public bool IsInRange(float time)
    {
        if (time < stepDuration - stepInputInterval.x)
        {
            return false;
        }

        if (time > stepDuration + stepInputInterval.y)
        {
            return false;
        }

        return true;
    }

    public void InRangePlayerStep(PlayerAction action)
    {
        if (IsInRange(stepTimer) && !HasDonePlayerAction)
        {
            PlayerStep(action);
        }
    }

    public void PlayerStep(PlayerAction action)
    {
        if (!playerIsAlive)
        {
            return;
        }

        HasDonePlayerAction = true;

        action.ResolvePlayerAction(this);

        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }

        PreviousAction = action;
    }

    public void MovePlayerToNextPointOnPath()
    {
        IncrementPathPosIndex();

        Vector3 newPos = gridManager.Path[currentPathPosIndex].transform.position;
        newPos.y = 0;



        //Visual movement
        if (currentPathPosIndex + 1 < gridManager.Path.Length)
        {
            return;
        }
        StartCoroutine(LerpToPositon(newPos, 0.5f));
        playerGO.GetComponent<Animator>().Play("Sprint");


    }
    private bool hasBeenRaised = false;

    public void IncrementPathPosIndex()
    {
        if (currentPathPosIndex >= gridManager.Path.Length - 1)
        {
            Debug.Log("End of path");
            if (hasBeenRaised) return;
            winEvent.Raise();
            return;
        }

        currentPathPosIndex++;
    }

    IEnumerator LerpToPositon(Vector3 pos, float percentageOfStep)
    {

        Vector3 fromPos = playerGO.transform.position;
        float timeElapsed = 0;
        float lerpDuration = stepDuration * percentageOfStep;
        while (timeElapsed < lerpDuration)
        {
            if (!playerIsAlive)
            {
                timeElapsed += 100f;
                yield return null;
            }
            playerGO.transform.position = Vector3.Lerp(fromPos, pos, timeElapsed / lerpDuration);

            timeElapsed += Time.deltaTime;

            if (timeElapsed > lerpDuration * 0.6f)
            {
                Vector3 newPos = gridManager.Path[currentPathPosIndex + 1].transform.position;
                newPos.y = 0;
                StartCoroutine(LerpRotationToNextDestination(newPos));
            }
            yield return null;
        }

    }

    IEnumerator LerpRotationToNextDestination(Vector3 pos)
    {
        Vector3 fromPos = playerGO.transform.position;
        Quaternion fromRot = playerGO.transform.rotation;
        float timeElapsed = 0;
        float lerpDuration = stepDuration * 0.2f;
        while (timeElapsed < lerpDuration)
        {
            if (!playerIsAlive)
            {
                timeElapsed += 100f;
                yield return null;
            }

            playerGO.transform.rotation = Quaternion.Lerp(fromRot,
              Quaternion.LookRotation(pos - fromPos, Vector3.up), Mathf.Clamp01(timeElapsed / lerpDuration));

            timeElapsed += Time.deltaTime;
            if (timeElapsed > lerpDuration)
            {
                playerGO.transform.rotation = Quaternion.Lerp(fromRot,
                             Quaternion.LookRotation(pos - fromPos, Vector3.up), 1);

            }
            yield return null;
        }

    }


    private Cell CellPlayerIsOn()
    {
        return gridManager.Path[currentPathPosIndex];
    }
    public int2 PlayerPositionOnGrid()
    {
        return gridManager.Path[currentPathPosIndex].point;
    }

    public void PlayerAttackAction()
    {


        IncrementPathPosIndex();

        Vector3 newPos = gridManager.Path[currentPathPosIndex].transform.position;
        newPos.y = 0;
        playerGO.transform.LookAt(newPos);
        StartCoroutine(LerpToPositon(newPos, 0.1f));

        var anim = playerGO.GetComponent<Animator>();
        anim.Play("Charge_Attack");
        //anim.speed = 3;

        int2 playerPos = PlayerPositionOnGrid();

        Debug.Log(playerPos);
        Debug.Log(gridManager.GetCellByPoint(playerPos).point);

        StepUnit disguydead;
        disguydead = gridManager.GetCellByPoint(playerPos).transform.GetComponentInChildren<StepUnit>();
        //Destroy(gridManager.GetCellByPoint(playerPos));

        Debug.Log(gridManager.GetCellByPoint(playerPos).transform.GetComponentInChildren<StepUnit>());

        disguydead?.IsKill();


    }

    public float GetStepTime()
    {
        return stepTimer;
    }


    public void TurnOnRagdoll(Vector3 force, Vector3 impactPoint)
    {
        Animator animator = playerGO.GetComponent<Animator>();


        foreach (Collider coll in ragdollParts)
        {
            Rigidbody rigidbody = coll.GetComponent<Rigidbody>();
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
            coll.isTrigger = false;

            rigidbody.AddForce(force, ForceMode.Impulse);
        }

        playerGO.transform.DetachChildren();

        animator.enabled = false;
    }

    [Button(ButtonSizes.Large)]
    private void SetRagdollParts()
    {
        Collider[] colliders = playerGO.GetComponentsInChildren<Collider>();
        Debug.Log("Amount of colliders in children: " + colliders.Length);

        foreach (Collider coll in colliders)
        {
            if (coll.gameObject != this.gameObject)
            {
                coll.isTrigger = true;
                Rigidbody collRb = coll.GetComponent<Rigidbody>();
                collRb.useGravity = false;
                collRb.isKinematic = true;
                ragdollParts.Add(coll);
            }
        }
    }

    public void PlayerDeflect()
    {
        Animator anim = playerGO.GetComponent<Animator>();
        anim.Play("Deflect");
    }
}
