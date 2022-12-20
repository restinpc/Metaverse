using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Engine.Mechanics;
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
    public class Enemy : Component
    {
        /* @EnemyController */
        [System.Serializable]
        public struct RendererIndexData
        {
            public Renderer Renderer;
            public int MaterialIndex;

            public RendererIndexData(Renderer renderer, int index)
            {
                Renderer = renderer;
                MaterialIndex = index;
            }
        }

        [Header("Parameters")]
        [Tooltip("The Y height at which the enemy will be automatically killed (if it falls off of the level)")]
        public float SelfDestructYHeight = -20f;

        [Tooltip("The distance at which the enemy considers that it has reached its current path destination point")]
        public float PathReachingRadius = 2f;

        [Tooltip("The speed at which the enemy rotates")]
        public float OrientationSpeed = 10f;

        [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
        public float DeathDuration = 0f;


        [Header("Weapons Parameters")]
        [Tooltip("Allow weapon swapping for this enemy")]
        public bool SwapToNextWeapon = false;

        [Tooltip("Time delay between a weapon swap and the next attack")]
        public float DelayAfterWeaponSwap = 0f;

        [Header("Eye color")]
        [Tooltip("Material for the eye color")]
        public Material EyeColorMaterial;

        [Tooltip("The default color of the bot's eye")]
        [ColorUsageAttribute(true, true)]
        public Color DefaultEyeColor;

        [Tooltip("The attack color of the bot's eye")]
        [ColorUsageAttribute(true, true)]
        public Color AttackEyeColor;

        [Header("Flash on hit")]
        [Tooltip("The material used for the body of the hoverbot")]
        public Material BodyMaterial;

        [Tooltip("The gradient representing the color of the flash on hit")]
        [GradientUsageAttribute(true)]
        public Gradient OnHitBodyGradient;

        [Tooltip("The duration of the flash on hit")]
        public float FlashOnHitDuration = 0.5f;

        [Header("Sounds")]
        [Tooltip("Sound played when recieving damages")]
        public AudioClip DamageTick;

        [Header("VFX")]
        [Tooltip("The VFX prefab spawned when the enemy dies")]
        public GameObject DeathVfx;

        [Tooltip("The point at which the death VFX is spawned")]
        public Transform DeathVfxSpawnPoint;

        [Header("Loot")]
        [Tooltip("The object this enemy can drop when dying")]
        public GameObject LootPrefab;

        [Tooltip("The chance the object has to drop")]
        [Range(0, 1)]
        public float DropRate = 1f;

        [Header("Debug Display")]
        [Tooltip("Color of the sphere gizmo representing the path reaching range")]
        public Color PathReachingRangeColor = Color.yellow;

        [Tooltip("Color of the sphere gizmo representing the attack range")]
        public Color AttackRangeColor = Color.red;

        [Tooltip("Color of the sphere gizmo representing the detection range")]
        public Color DetectionRangeColor = Color.blue;

        public UnityAction onAttack;
        public UnityAction onDetectedTarget;
        public UnityAction onLostTarget;
        public UnityAction onDamaged;

        List<RendererIndexData> m_BodyRenderers = new List<RendererIndexData>();
        MaterialPropertyBlock m_BodyFlashMaterialPropertyBlock;
        float m_LastTimeDamaged = float.NegativeInfinity;

        RendererIndexData m_EyeRendererData;
        MaterialPropertyBlock m_EyeColorMaterialPropertyBlock;

        public PatrolPath PatrolPath { get; set; }
        public GameObject KnownDetectedTarget => DetectionModule.KnownDetectedTarget;
        public bool IsTargetInAttackRange => DetectionModule.IsTargetInAttackRange;
        public bool IsSeeingTarget => DetectionModule.IsSeeingTarget;
        public bool HadKnownTarget => DetectionModule.HadKnownTarget;
        public NavMeshAgent NavMeshAgent { get; private set; }
        public DetectionModule DetectionModule { get; private set; }

        int m_PathDestinationNodeIndex;
        Collider[] m_SelfColliders;
        bool m_WasDamagedThisFrame;
        float m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
        int m_CurrentWeaponIndex;
        WeaponController m_CurrentWeapon;
        WeaponController[] m_Weapons;
        NavigationModule m_NavigationModule;

        /* @Health */
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        bool m_IsDead;

        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Enemy(
            App application,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, scene, parent, name, mapStateToProps) {
            CurrentHealth = MaxHealth;
        }

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
                    NavMeshAgent == null
                    || PatrolPath == null
                ))
                {
                    PatrolPath = GameObject.FindObjectOfType<PatrolPath>();
                    DebugUtility.HandleErrorIfNullFindObject<PatrolPath, GameObject>(PatrolPath, null);

                    NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
                    m_SelfColliders = gameObject.GetComponentsInChildren<Collider>();

                    // Find and initialize all weapons
                    FindAndInitializeAllWeapons();
                    var weapon = GetCurrentWeapon();
                    weapon.ShowWeapon(true);

                    var detectionModules = gameObject.GetComponentsInChildren<DetectionModule>();
                    DebugUtility.HandleErrorIfNoComponentFound<DetectionModule, GameObject>(detectionModules.Length, null,
                        gameObject);

                    DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, GameObject>(detectionModules.Length,
                        null, gameObject);
                    // Initialize detection module
                    DetectionModule = detectionModules[0];
                    DetectionModule.onDetectedTarget += OnDetectedTarget;
                    DetectionModule.onLostTarget += OnLostTarget;
                    onAttack += DetectionModule.OnAttack;

                    var navigationModules = gameObject.GetComponentsInChildren<NavigationModule>();
                    DebugUtility.HandleWarningIfDuplicateObjects<DetectionModule, GameObject>(detectionModules.Length,
                        null, gameObject);
                    // Override navmesh agent data
                    if (navigationModules.Length > 0)
                    {
                        m_NavigationModule = navigationModules[0];
                        NavMeshAgent.speed = m_NavigationModule.MoveSpeed;
                        NavMeshAgent.angularSpeed = m_NavigationModule.AngularSpeed;
                        NavMeshAgent.acceleration = m_NavigationModule.Acceleration;
                    }

                    foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
                    {
                        for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                        {
                            if (renderer.sharedMaterials[i] == EyeColorMaterial)
                            {
                                m_EyeRendererData = new RendererIndexData(renderer, i);
                            }

                            if (renderer.sharedMaterials[i] == BodyMaterial)
                            {
                                m_BodyRenderers.Add(new RendererIndexData(renderer, i));
                            }
                        }
                    }

                    m_BodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();

                    // Check if we have an eye renderer for this enemy
                    if (m_EyeRendererData.Renderer != null)
                    {
                        m_EyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                        m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                        m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                            m_EyeRendererData.MaterialIndex);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Enemy(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public virtual void OnDamaged(float damage, GameObject damageSource)
        {
            // test if the damage source is the player
            if (damageSource/* && !damageSource.GetComponent<EnemyController>()*/)
            {
                // pursue the player
                DetectionModule.OnDamaged(damageSource);

                onDamaged?.Invoke();
                m_LastTimeDamaged = Time.time;

                // play the damage tick sound
                if (DamageTick && !m_WasDamagedThisFrame)
                    AudioUtility.CreateSFX(DamageTick, gameObject.transform.position, AudioUtility.AudioGroups.DamageTick, 0f);

                m_WasDamagedThisFrame = true;
            }
        }

        public virtual void OnLostTarget()
        {
            onLostTarget.Invoke();

            // Set the eye attack color and property block if the eye renderer is set
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", DefaultEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                    m_EyeRendererData.MaterialIndex);
            }
        }

        public virtual void OnDetectedTarget()
        {
            onDetectedTarget.Invoke();

            // Set the eye default color and property block if the eye renderer is set
            if (m_EyeRendererData.Renderer != null)
            {
                m_EyeColorMaterialPropertyBlock.SetColor("_EmissionColor", AttackEyeColor);
                m_EyeRendererData.Renderer.SetPropertyBlock(m_EyeColorMaterialPropertyBlock,
                    m_EyeRendererData.MaterialIndex);
            }
        }

        public void OrientTowards(Vector3 lookPosition)
        {
            Vector3 lookDirection = Vector3.ProjectOnPlane(lookPosition - gameObject.transform.position, Vector3.up).normalized;
            if (lookDirection.sqrMagnitude != 0f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                gameObject.transform.rotation =
                    Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
            }
        }

        bool IsPathValid()
        {
            return PatrolPath && PatrolPath.PathNodes.Count > 0;
        }

        public void ResetPathDestination()
        {
            m_PathDestinationNodeIndex = 0;
        }

        public void SetPathDestinationToClosestNode()
        {
            if (IsPathValid())
            {
                int closestPathNodeIndex = 0;
                for (int i = 0; i < PatrolPath.PathNodes.Count; i++)
                {
                    float distanceToPathNode = PatrolPath.GetDistanceToNode(gameObject.transform.position, i);
                    if (distanceToPathNode < PatrolPath.GetDistanceToNode(gameObject.transform.position, closestPathNodeIndex))
                    {
                        closestPathNodeIndex = i;
                    }
                }

                m_PathDestinationNodeIndex = closestPathNodeIndex;
            }
            else
            {
                m_PathDestinationNodeIndex = 0;
            }
        }

        public Vector3 GetDestinationOnPath()
        {
            if (IsPathValid())
            {
                return PatrolPath.GetPositionOfPathNode(m_PathDestinationNodeIndex);
            }
            else
            {
                return gameObject.transform.position;
            }
        }

        public void SetNavDestination(Vector3 destination)
        {
            if (NavMeshAgent)
            {
                NavMeshAgent.SetDestination(destination);
            }
        }

        public void UpdatePathDestination(bool inverseOrder = false)
        {
            if (IsPathValid())
            {
                // Check if reached the path destination
                if ((gameObject.transform.position - GetDestinationOnPath()).magnitude <= PathReachingRadius)
                {
                    // increment path destination index
                    m_PathDestinationNodeIndex =
                        inverseOrder ? (m_PathDestinationNodeIndex - 1) : (m_PathDestinationNodeIndex + 1);
                    if (m_PathDestinationNodeIndex < 0)
                    {
                        m_PathDestinationNodeIndex += PatrolPath.PathNodes.Count;
                    }

                    if (m_PathDestinationNodeIndex >= PatrolPath.PathNodes.Count)
                    {
                        m_PathDestinationNodeIndex -= PatrolPath.PathNodes.Count;
                    }
                }
            }
        }

        void OnDie()
        {
            // spawn a particle system when dying
            var vfx = UnityEngine.Object.Instantiate(DeathVfx, DeathVfxSpawnPoint.position, Quaternion.identity);
            UnityEngine.Object.Destroy(vfx, 5f);

            // loot an object
            if (TryDropItem())
            {
                UnityEngine.Object.Instantiate(LootPrefab, gameObject.transform.position, Quaternion.identity);
            }

            // this will call the OnDestroy function
            UnityEngine.Object.Destroy(gameObject, DeathDuration);
        }

        void OnDrawGizmosSelected()
        {
            // Path reaching range
            Gizmos.color = PathReachingRangeColor;
            Gizmos.DrawWireSphere(gameObject.transform.position, PathReachingRadius);

            if (DetectionModule != null)
            {
                // Detection range
                Gizmos.color = DetectionRangeColor;
                Gizmos.DrawWireSphere(gameObject.transform.position, DetectionModule.DetectionRange);

                // Attack range
                Gizmos.color = AttackRangeColor;
                Gizmos.DrawWireSphere(gameObject.transform.position, DetectionModule.AttackRange);
            }
        }

        public void OrientWeaponsTowards(Vector3 lookPosition)
        {
            for (int i = 0; i < m_Weapons.Length; i++)
            {
                // orient weapon towards player
                Vector3 weaponForward = (lookPosition - m_Weapons[i].WeaponRoot.transform.position).normalized;
                m_Weapons[i].transform.forward = weaponForward;
            }
        }

        public bool TryAtack(Vector3 enemyPosition)
        {

            OrientWeaponsTowards(enemyPosition);

            if ((m_LastTimeWeaponSwapped + DelayAfterWeaponSwap) >= Time.time)
                return false;

            // Shoot the weapon
            bool didFire = GetCurrentWeapon().HandleShootInputs(false, true, false);

            if (didFire && onAttack != null)
            {
                onAttack.Invoke();

                if (SwapToNextWeapon && m_Weapons.Length > 1)
                {
                    int nextWeaponIndex = (m_CurrentWeaponIndex + 1) % m_Weapons.Length;
                    SetCurrentWeapon(nextWeaponIndex);
                }
            }

            return didFire;
        }

        public bool TryDropItem()
        {
            if (DropRate == 0 || LootPrefab == null)
                return false;
            else if (DropRate == 1)
                return true;
            else
                return (UnityEngine.Random.value <= DropRate);
        }

        void FindAndInitializeAllWeapons()
        {
            // Check if we already found and initialized the weapons
            if (m_Weapons == null)
            {
                m_Weapons = gameObject.GetComponentsInChildren<WeaponController>();
                DebugUtility.HandleErrorIfNoComponentFound<WeaponController, GameObject>(m_Weapons.Length, null,
                    gameObject);

                for (int i = 0; i < m_Weapons.Length; i++)
                {
                    m_Weapons[i].Owner = gameObject;
                }
            }
        }

        public WeaponController GetCurrentWeapon()
        {
            FindAndInitializeAllWeapons();
            // Check if no weapon is currently selected
            if (m_CurrentWeapon == null)
            {
                // Set the first weapon of the weapons list as the current weapon
                SetCurrentWeapon(0);
            }

            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, GameObject>(m_CurrentWeapon, null,
                gameObject);

            return m_CurrentWeapon;
        }

        void SetCurrentWeapon(int index)
        {
            m_CurrentWeaponIndex = index;
            m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex];
            if (SwapToNextWeapon)
            {
                m_LastTimeWeaponSwapped = Time.time;
            }
            else
            {
                m_LastTimeWeaponSwapped = Mathf.NegativeInfinity;
            }
        }

    }
}