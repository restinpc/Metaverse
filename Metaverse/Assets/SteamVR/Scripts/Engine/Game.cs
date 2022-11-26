
using Platformer.Mechanics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Platformer.Core.Simulation;

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


        void Awake()
        {
            Debug.Log("Engine.Game.Awake()");
            Model.gameModel = this;
            if (Model.application == null)
            {
                Model.application = new App();
            }
            activeScene = Model.application.state["activeScene"].getString();
            // Loading Scene
            if (activeScene == Scene.Loading.ToString())
            {
                if (Model.loadingScene == null)
                {
                    Model.loadingScene = new Dictionary<string, Components.Component> {
                        { "Loading.Game", null },
                        { "Loading.Navigation", null },
                        { "Loading.Caption", null },
                        { "Loading.Text", null },
                        { "Loading.Menu", null }
                    };
                    new Wrappers.Element(
                        "Loading.Game",
                        Model.loadingScene,
                        Scene.Loading
                    );
                    new Wrappers.Element(
                        "Loading.Navigation",
                        Model.loadingScene,
                        Scene.Loading,
                        Model.loadingScene["Loading.Game"]
                    );
                    new Wrappers.LoadingCaption(
                        "Loading.Caption",
                        Model.loadingScene,
                        Scene.Loading,
                        Model.loadingScene["Loading.Navigation"]
                    );
                    new Wrappers.LoadingText(
                        "Loading.Text",
                        Model.loadingScene,
                        Scene.Loading,
                        Model.loadingScene["Loading.Navigation"]
                    );
                    new Wrappers.NewGameButton(
                        "Loading.Menu",
                        Model.loadingScene,
                        Scene.Loading,
                        Model.loadingScene["Loading.Navigation"]
                    );
                }
                Model.application.stdin = Model.loadingScene["Loading.Game"];
            }
            // Menu Scene
            else if (activeScene == Scene.Menu.ToString())
            {
                if (Model.menuScene == null)
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
                    new Wrappers.Element(
                        "Menu.Navigation",
                        Model.menuScene,
                        Scene.Menu,
                        Model.menuScene["Menu.Game"]
                    );
                    new Wrappers.LabelWrapper(
                        "Menu.Caption",
                        Model.menuScene,
                        Scene.Menu,
                        Model.menuScene["Menu.Navigation"],
                        "Main Menu"
                    );
                }
                Model.application.stdin = Model.menuScene["Menu.Game"];
            }
            // Mansion Scene
            else if (activeScene == Scene.Mansion.ToString())
            {
                if (Model.mansionScene == null)
                {
                    Model.mansionScene = new Dictionary<string, Components.Component> {
                        { "Mansion.Game", null },
                    };
                    new Wrappers.Element(
                        "Mansion.Game",
                        Model.mansionScene,
                        Scene.Mansion
                    );
                }
                Model.application.stdin = Model.mansionScene["Mansion.Game"];
            }
        }
        void Start()
        {
            if (activeScene != Scene.Loading.ToString() && !Model.application.state["started"].getBool())
            {
                Model.application.setState(Model.startGame());
            } else
            {
                Model.application.render();
            }
        }

        public void ChangeScene(string targetScene)
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Game.ChangeScene(" + targetScene + ")");
            }
            string currentSceneName = SceneManager.GetActiveScene().name;
            bool flag = true;
            if (targetScene != "Loading")
            {
                if (currentSceneName != "Loading")
                {
                    flag = false;
                    Schedule<Gameplay.SceneChange>(1.5f).targetScene = targetScene;
                    Model.application.setState(
                        Model.mergeActions(
                            new List<Dictionary<string, Prop>>() {
                                Model.setScene(Scene.Loading.ToString()),
                                Model.toggleLoading(true)
                            }
                        )
                    );
                }
            }
            if (flag)
            {
                Model.application.setState(
                    Model.mergeActions(
                        new List<Dictionary<string, Prop>>() {
                            Model.setScene(targetScene),
                            Model.toggleLoading(false)
                        }
                    )
                );
            }
        }

        void Update()
        {
            if (Model.application.DEBUG && Model.application.DEEP_DEBUG)
            {
                Debug.Log("Engine.Game.Update()");
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