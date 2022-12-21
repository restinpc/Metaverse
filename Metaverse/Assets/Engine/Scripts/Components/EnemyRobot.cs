using System;
using System.Collections.Generic;
using UnityEngine;
using Engine.Props;
using Engine.Core;

namespace Engine.Components
{
    /**
     * @property application Application object.
     * @property name Component name.
     * @property parent Virtual DOM parent node.
     * @property nodes Array with a child nodes.
     * @property gameObject Unity game object.
     * @property renderId Render method executions counter (debug tool).
     * @property mapStateToProps Function to map object properties from application state container.
     * @property label TextMeshProUGUI element.
     */
    public class EnemyRobot : Enemy
    {
        public enum AIState
        {
            Patrol,
            Follow,
            Attack,
        }

        public Animator Animator;
        public RobotProps Props;

        [Tooltip("Fraction of the enemy's attack range at which it will stop moving towards target while attacking")]
        [Range(0f, 1f)]
        public float AttackStopDistanceRatio = 0.5f;
        public MinMaxFloat PitchDistortionMovementSpeed;

        public AIState AiState { get; private set; }
        AudioSource m_AudioSource;

        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimAlertedParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";

        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public EnemyRobot(
            App application,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, scene, parent, name, mapStateToProps) { }
        /**
         * Method to output html content to parent node.
         * @param stdout HTML Element to output.
         */
        public override void render(GameObject stdout)
        {
            base.render(stdout);
            try
            {
                if (this.gameObject && (Animator == null || Props == null))
                {
                    Animator = gameObject.GetComponentInChildren<Animator>();
                    Props = gameObject.GetComponentInChildren<RobotProps>();
                    PatrolPath = Props.PatrolPath;
                    SetPathDestinationToClosestNode();
                    // Start patrolling
                    AiState = AIState.Patrol;
                    // adding a audio source to play the movement sound on it
                    m_AudioSource = gameObject.GetComponent<AudioSource>();
                    DebugUtility.HandleErrorIfNullGetComponent<AudioSource, GameObject>(m_AudioSource, null, gameObject);
                    m_AudioSource.clip = Props.MovementSound;
                    m_AudioSource.Play();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Label(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
        }

        public override void Update()
        {
            base.Update();
            UpdateAiStateTransitions();
            UpdateCurrentAiState();

            float moveSpeed = NavMeshAgent.velocity.magnitude;

            // Update animator speed parameter
            Animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);

            // changing the pitch of the movement sound depending on the movement speed
            m_AudioSource.pitch = Mathf.Lerp(PitchDistortionMovementSpeed.Min, PitchDistortionMovementSpeed.Max,
                moveSpeed / NavMeshAgent.speed);
        }

        void UpdateAiStateTransitions()
        {
            // Handle transitions 
            switch (AiState)
            {
                case AIState.Follow:
                    // Transition to attack when there is a line of sight to the target
                    if (IsSeeingTarget && IsTargetInAttackRange)
                    {
                        AiState = AIState.Attack;
                        SetNavDestination(gameObject.transform.position);
                    }

                    break;
                case AIState.Attack:
                    // Transition to follow when no longer a target in attack range
                    if (!IsTargetInAttackRange)
                    {
                        AiState = AIState.Follow;
                    }

                    break;
            }
        }

        void UpdateCurrentAiState()
        {
            // Handle logic 
            switch (AiState)
            {
                case AIState.Patrol:
                    UpdatePathDestination();
                    SetNavDestination(GetDestinationOnPath());
                    break;
                case AIState.Follow:
                    SetNavDestination(KnownDetectedTarget.transform.position);
                    OrientTowards(KnownDetectedTarget.transform.position);
                    OrientWeaponsTowards(KnownDetectedTarget.transform.position);
                    break;
                case AIState.Attack:
                    if (Vector3.Distance(KnownDetectedTarget.transform.position,
                            DetectionModule.DetectionSourcePoint.position)
                        >= (AttackStopDistanceRatio * DetectionModule.AttackRange))
                    {
                        SetNavDestination(KnownDetectedTarget.transform.position);
                    }
                    else
                    {
                        SetNavDestination(gameObject.transform.position);
                    }

                    OrientTowards(KnownDetectedTarget.transform.position);
                    TryAtack(KnownDetectedTarget.transform.position);
                    break;
            }
        }

        void OnAttack()
        {
            base.onAttack();
            Animator.SetTrigger(k_AnimAttackParameter);
        }

        public override void OnDetectedTarget()
        {
            base.onDetectedTarget();
            if (AiState == AIState.Patrol)
            {
                AiState = AIState.Follow;
            }
            for (int i = 0; i < Props.OnDetectVfx.Length; i++)
            {
                Props.OnDetectVfx[i].Play();
            }
            if (Props.OnDetectSfx)
            {
                AudioUtility.CreateSFX(Props.OnDetectSfx, gameObject.transform.position, AudioUtility.AudioGroups.EnemyDetection, 1f);
            }
            Animator.SetBool(k_AnimAlertedParameter, true);
        }

        public override void OnLostTarget()
        {
            base.OnLostTarget();
            if (AiState == AIState.Follow || AiState == AIState.Attack)
            {
                AiState = AIState.Patrol;
            }
            for (int i = 0; i < Props.OnDetectVfx.Length; i++)
            {
                Props.OnDetectVfx[i].Stop();
            }
            Animator.SetBool(k_AnimAlertedParameter, false);
        }

        public override void OnDamaged(float damage, GameObject damageSource)
        {
            base.OnDamaged(damage, damageSource);
            if (Props.RandomHitSparks.Length > 0)
            {
                int n = UnityEngine.Random.Range(0, Props.RandomHitSparks.Length - 1);
                Props.RandomHitSparks[n].Play();
            }
            Animator.SetTrigger(k_AnimOnDamagedParameter);
        }
    }
}