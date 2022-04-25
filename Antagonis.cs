using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.AI;
using UnityEngine;
public enum AntagonisMove {
    Patrol, Chase, Wait
}
public class Antagonis : MonoBehaviour
{
    public bool canSeePlayer;
    public ATGData aTGData;
    public Transform[] Patrol;
    public Transform target, PFOV;
    [HideInInspector] public float v_radius;
    public AntagonisMove antagonisMove;
    public LayerMask playerMask, obstructionMask;  
    private float rateWaiting, maxWaiting = 5, ratetimeToWait, rateChasing, animationMoveValue;
    private NavMeshAgent nma;
    private Animator animator;
    private int patrolIndex;

    private void Awake () {
        animator = GetComponent<Animator>();
        patrolIndex = Random.Range(0, Patrol.Length);
        nma = GetComponent<NavMeshAgent>();
        StartCoroutine(FOVRoutine());
    }
    private void Update () {
        if(animationMoveValue != nma.velocity.magnitude)
            animationMoveValue = Mathf.Lerp(animationMoveValue, nma.velocity.magnitude, 15 * Time.deltaTime);
        animator.SetFloat("Walk", animationMoveValue);
        animator.SetBool("Looking Around", antagonisMove == AntagonisMove.Wait);
        switch(antagonisMove) {
            case AntagonisMove.Chase : 
                Chasing();
            break;
            case AntagonisMove.Patrol : 
                Patroling();
            break;
            case AntagonisMove.Wait : 
                Waiting();
            break;
        }
    }

    private void FixedUpdate () {
        switch(antagonisMove) {
            case AntagonisMove.Chase : 
                if(nma.destination != target.position) nma.destination = target.position;
            break;
            case AntagonisMove.Patrol : 
                if(nma.destination != Patrol[patrolIndex].position) nma.destination = (Patrol[patrolIndex].position);
            break;
            case AntagonisMove.Wait : 
                if(nma.destination != transform.position) nma.destination = transform.position;
            break;
        }
    }

    private void Waiting () {
        if(rateWaiting > maxWaiting) {
            patrolIndex = Random.Range(0, Patrol.Length);
            SwitchMoveMode("Patrol");
        } else {
            rateWaiting += Time.deltaTime;
        }
        v_radius = aTGData.radius;
    }

    private void Chasing () {
        nma.speed = aTGData.chaseSpeed;
        v_radius = aTGData.radius * 0.65f; 
        if(canSeePlayer == false) {
            if(rateChasing < aTGData.chaseTime) {
                rateChasing += Time.deltaTime;
            } else {
                SwitchMoveMode("Wait");
            }
        }
    }

    private void Patroling () {
        nma.speed = aTGData.walkSpeed;
        v_radius = aTGData.radius;
        if(Vector3.Distance(transform.position, new Vector3(Patrol[patrolIndex].position.x, transform.position.y, Patrol[patrolIndex].position.z)) < 0.15f) {
            SwitchMoveMode("Wait");
        } 

        if(ratetimeToWait > Random.Range(aTGData.timeToWait/2,aTGData.timeToWait)) {
            SwitchMoveMode("Wait");
        } else {
            ratetimeToWait += Time.deltaTime;
        }
    }

    private IEnumerator FOVRoutine () {
        WaitForSeconds wait = new WaitForSeconds(0.3f);
        while(true) {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
    private void FieldOfViewCheck () {
        Collider[] radiusChecks = Physics.OverlapSphere(transform.position, 1f, playerMask);
        if(radiusChecks.Length > 0) {
            SwitchMoveMode("Chase");
        }
        Collider[] rangeChecks = Physics.OverlapSphere(PFOV.position, v_radius, playerMask);
        if(rangeChecks.Length > 0) {
            target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - PFOV.position).normalized;
            if(Vector3.Angle(transform.forward, directionToTarget) < aTGData.angle / 2) {
                float distanceToTarget = Vector3.Distance(PFOV.position, target.position);
                if(!Physics.Raycast(PFOV.position, directionToTarget, distanceToTarget, obstructionMask)) {
                    SwitchMoveMode("Chase");
                    canSeePlayer = true;
                } else {
                    canSeePlayer = false;
                }
            } else 
                canSeePlayer = false;
        } else if(canSeePlayer)
            canSeePlayer = false;
    }

    private void SwitchMoveMode (string act) {
        if(act == "Chase") {
            rateWaiting = ratetimeToWait = rateChasing = 0;
            antagonisMove = AntagonisMove.Chase;
        } else if(act == "Wait") {
            rateWaiting = ratetimeToWait = rateChasing = 0;
            antagonisMove = AntagonisMove.Wait;
        } else if(act == "Patrol") {
            rateWaiting = rateChasing = 0;
            antagonisMove = AntagonisMove.Patrol;
        }
    }
}
