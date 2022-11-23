
using Platformer.Mechanics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public string activeScene;

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
            if (Model.application == null)
            {
                Model.application = new App();
            }
            activeScene = Model.application.state["activeScene"].getString();
            if (activeScene == Scene.Loading.ToString() && Model.loadingScene == null)
            {
                Model.loadingScene = new Dictionary<string, Components.Component> {
                    { "Loading.Game", null },
                    { "Loading.Navigation", null },
                    { "Loading.Caption", null }
                };
                new Wrappers.Element(
                    "Loading.Game",
                    Model.loadingScene,
                    Scene.Loading
                );
                Model.application.stdin = Model.loadingScene["Loading.Game"];
                new Wrappers.Element(
                    "Loading.Navigation",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Game"]
                );
                new Wrappers.Caption(
                    "Loading.Caption",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
            }
            else if (activeScene == Scene.Menu.ToString() && Model.menuScene == null)
            {
                Model.menuScene = new Dictionary<string, Components.Component> {
                    { "Menu.Game", null },
                    { "Menu.Navigation", null },
                    { "Menu.Caption", null }
                };
                new Wrappers.Element(
                    "Menu.Game",
                    Model.menuScene,
                    Scene.Menu
                );
                Model.application.stdin = Model.menuScene["Menu.Game"];
                new Wrappers.Element(
                    "Menu.Navigation",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Game"]
                );
                new Wrappers.Caption(
                    "Menu.Caption",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"]
                );
            }
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
            string targetScene = Model.application.state["activeScene"].getString();
            if (!targetScene.Equals(activeScene))
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadSceneAsync(targetScene);
                SceneManager.UnloadSceneAsync(currentSceneName);
                activeScene = targetScene;
            }
            /*
            bool isStarted = Model.application.state["started"].getBool() == true;
            bool isPaused = Model.application.state["paused"].getBool() == true;
            bool isDead = Model.application.state["dead"].getBool() == true;
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