
using Platformer.Mechanics;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [System.Serializable]
    public class Game : MonoBehaviour
    {
        /// <summary>
        /// The virtual camera in the scene.
        /// </summary>
        // public Cinemachine.CinemachineVirtualCamera virtualCamera;

        /// <summary>
        /// The main component which controls the player sprite, controlled 
        /// by the user.
        /// </summary>
        public PlayerController player;

        public Valve.VR.InteractionSystem.Player vr_player;

        /// <summary>
        /// The spawn point in the scene.
        /// </summary>
        public Transform spawnPoint;

        /// <summary>
        /// A global jump modifier applied to all initial jump velocities.
        /// </summary>
        public float jumpModifier = 1.5f;

        /// <summary>
        /// A global jump modifier applied to slow down an active jump when 
        /// the user releases the jump input.
        /// </summary>
        public float jumpDeceleration = 0.5f;

        void Awake()
        {
            Model.gameModel = this;
            Model.application = new App();
            Model.loadingLevel = new Dictionary<string, Components.Component> {
                { "Loading.Game", null },
                { "Loading.Navigation", null },
                { "Loading.Caption", null }
            };
            new Wrappers.Element("Loading.Game", Model.loadingLevel);
            Model.application.stdin = Model.loadingLevel["Loading.Game"];
            new Wrappers.Element("Loading.Navigation", Model.loadingLevel, Model.application.stdin);
            new Wrappers.Caption("Loading.Caption", Model.loadingLevel, Model.loadingLevel["Loading.Navigation"]);
        }
        void Start()
        {
            Model.application.render();
        }
        void Update()
        {
            if (Model.application.DEBUG && Model.application.DEEP_DEBUG)
            {
                Debug.Log("Engine.Game.Update()");
            }
            bool isStarted = Model.application.state["started"].getBool() == true;
            bool isPaused = Model.application.state["paused"].getBool() == true;
            bool isDead = Model.application.state["dead"].getBool() == true;
            /*
            if (Input.GetButtonUp("Jump"))
            {
                if (!isStarted)
                {
                    Model.application.setState(Model.startGame());
                } else if (isDead)
                {
                    Model.application.setState(Model.revivePlayer());
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("Menu"))
            {
                if (isStarted && !isDead)
                {
                    if (!isPaused)
                    {
                        Model.application.setState(Model.pauseGame());
                    } else
                    {
                        Model.application.setState(Model.continueGame());
                    }
                }
            }
            */
        }
    }
}