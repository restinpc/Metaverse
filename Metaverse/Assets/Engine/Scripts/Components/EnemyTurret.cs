using System;
using System.Collections.Generic;
using UnityEngine;
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
     * @property EnemyTurret TextMeshProUGUI element.
     */
    public class EnemyTurret : Enemy
    {
        public enum AIState
        {
            Idle,
            Attack,
        }

        public Transform TurretPivot;
        public Transform TurretAimPoint;
        public Animator Animator;
        public TurretProps Props;
        public float AimRotationSharpness = 5f;
        public float LookAtRotationSharpness = 2.5f;
        public float DetectionFireDelay = 1f;
        public float AimingTransitionBlendTime = 1f;

        public AIState AiState { get; private set; }

        Quaternion m_RotationWeaponForwardToPivot;
        float m_TimeStartedDetection;
        float m_TimeLostDetection;
        Quaternion m_PreviousPivotAimingRotation;
        Quaternion m_PivotAimingRotation;

        const string k_AnimOnDamagedParameter = "OnDamaged";
        const string k_AnimIsActiveParameter = "IsActive";
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name EnemyTurret name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public EnemyTurret(
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
                if (this.gameObject && (
                    Animator == null
                    || Props == null
                    || TurretPivot == null
                    || TurretAimPoint == null
                ))
                {
                    Animator = gameObject.GetComponentInChildren<Animator>();
                    Props = gameObject.GetComponentInChildren<TurretProps>();
                    TurretPivot = GameObject.Find("FPS...Turret.Pivot").transform;
                    TurretAimPoint = GameObject.Find("FPS...Turret.WeaponRoot").transform;
                    // Remember the rotation offset between the pivot's forward and the weapon's forward
                    m_RotationWeaponForwardToPivot =
                        Quaternion.Inverse(this.GetCurrentWeapon().WeaponMuzzle.rotation) * TurretPivot.rotation;
                    // Start with idle
                    AiState = AIState.Idle;
                    m_TimeStartedDetection = Mathf.NegativeInfinity;
                    m_PreviousPivotAimingRotation = TurretPivot.rotation;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.EnemyTurret(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }

        public override void Update()
        {
            UpdateCurrentAiState();
        }

        public override void LateUpdate()
        {
            UpdateTurretAiming();
        }

        public override void OnDamaged(float dmg, GameObject source)
        {
            base.OnDamaged(dmg, source);
            if (Props.RandomHitSparks.Length > 0)
            {
                int n = UnityEngine.Random.Range(0, Props.RandomHitSparks.Length - 1);
                Props.RandomHitSparks[n].Play();
            }
            Animator.SetTrigger(k_AnimOnDamagedParameter);
        }

        void UpdateCurrentAiState()
        {
            // Handle logic 
            switch (AiState)
            {
                case AIState.Attack:
                    bool mustShoot = Time.time > m_TimeStartedDetection + DetectionFireDelay;
                    // Calculate the desired rotation of our turret (aim at target)
                    Vector3 directionToTarget =
                        (this.KnownDetectedTarget.transform.position - TurretAimPoint.position).normalized;
                    Quaternion offsettedTargetRotation =
                        Quaternion.LookRotation(directionToTarget) * m_RotationWeaponForwardToPivot;
                    m_PivotAimingRotation = Quaternion.Slerp(m_PreviousPivotAimingRotation, offsettedTargetRotation,
                        (mustShoot ? AimRotationSharpness : LookAtRotationSharpness) * Time.deltaTime);

                    // shoot
                    if (mustShoot)
                    {
                        Vector3 correctedDirectionToTarget =
                            (m_PivotAimingRotation * Quaternion.Inverse(m_RotationWeaponForwardToPivot)) *
                            Vector3.forward;

                        this.TryAtack(TurretAimPoint.position + correctedDirectionToTarget);
                    }

                    break;
            }
        }

        void UpdateTurretAiming()
        {
            if (TurretPivot != null)
            {
                switch (AiState)
                {
                    case AIState.Attack:
                        TurretPivot.rotation = m_PivotAimingRotation;
                        break;
                    default:
                        var a = TurretPivot.rotation.ToString();
                        // Use the turret rotation of the animation
                        TurretPivot.rotation = Quaternion.Slerp(m_PivotAimingRotation, TurretPivot.rotation,
                            (Time.time - m_TimeLostDetection) / AimingTransitionBlendTime);
                        if (a != TurretPivot.rotation.ToString())
                        {
                            Debug.LogError(a + " = " + TurretPivot.rotation.ToString());
                        }
                        break;
                }
                m_PreviousPivotAimingRotation = TurretPivot.rotation;
            }
        }

        public override void OnDetectedTarget()
        {
            base.onDetectedTarget();
            if (AiState == AIState.Idle)
            {
                AiState = AIState.Attack;
            }

            for (int i = 0; i < Props.OnDetectVfx.Length; i++)
            {
                Props.OnDetectVfx[i].Play();
            }

            if (Props.OnDetectSfx)
            {
                AudioUtility.CreateSFX(Props.OnDetectSfx, gameObject.transform.position, AudioUtility.AudioGroups.EnemyDetection, 1f);
            }

            Animator.SetBool(k_AnimIsActiveParameter, true);
            m_TimeStartedDetection = Time.time;
        }

        public override void OnLostTarget()
        {
            base.onLostTarget();
            if (AiState == AIState.Attack)
            {
                AiState = AIState.Idle;
            }

            for (int i = 0; i < Props.OnDetectVfx.Length; i++)
            {
                Props.OnDetectVfx[i].Stop();
            }

            Animator.SetBool(k_AnimIsActiveParameter, false);
            m_TimeLostDetection = Time.time;
        }
    }
}