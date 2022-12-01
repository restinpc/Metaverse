using UnityEngine;
using static Engine.Core.Simulation;


namespace Engine.Mechanics
{
    /// <summary>
    /// This class contains the data required for implementing token collection mechanics.
    /// It does not perform animation of the token, this is handled in a batch by the 
    /// TokenController in the scene.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class TokenInstance : MonoBehaviour
    {
        public AudioClip tokenCollectAudio;
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;
        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation, collectedAnimation;

        internal Sprite[] sprites = new Sprite[0];

        internal SpriteRenderer _renderer;

        //unique index which is assigned by the TokenController in a scene.
        internal int tokenIndex = -1;
        internal TokenController controller;
        //active frame in animation, updated by the controller.
        internal int frame = 0;
        internal bool collected = false;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
            sprites = idleAnimation;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //only exectue OnPlayerEnter if the player collides with this token.
            var player = other.gameObject.GetComponent<Valve.VR.InteractionSystem.Player>();
            if (player != null)
            {
                if (Engine.Model.application.DEBUG)
                {
                    Debug.Log("Platformer.Mechanics.TokenInstance.OnTriggerEnter2D()");
                }
                OnPlayerEnter(player);
            }
        }

        void OnPlayerEnter(Valve.VR.InteractionSystem.Player player)
        {
            if (collected) return;
            if (Engine.Model.application.DEBUG)
            {
                Debug.Log("Platformer.Mechanics.TokenInstance.OnPlayerEnter()");
            }
            //disable the gameObject and remove it from the controller update list.
            frame = 0;
            sprites = collectedAnimation;
            if (controller != null)
                collected = true;
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<Engine.Events.PlayerTokenCollision>();
            ev.token = this;
            ev.player = player;
        }
    }
}