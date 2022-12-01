
using Engine.Core;
using Engine.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Engine.Core.Simulation;

namespace Engine
{
    [System.Serializable]
    public class Game : MonoBehaviour
    {
        public Valve.VR.InteractionSystem.Player player;
        private string activeScene;
        public AudioSource audioSource;
        public AudioClip[] audioClipArray;

        void Awake()
        {
            Debug.Log("Engine.Game.Awake(" + SceneManager.GetActiveScene().name + ")");
            if (Model.application == null)
            {
                Model.application = new App();
            }
            Model.gameModel = this;
            activeScene = Model.application.state["activeScene"].getString();
            if (activeScene == Scene.Loading.ToString())
            {
                initLoadingScene();
                Model.application.stdin = Model.loadingScene["Loading.Game"];
            }
            else if (activeScene == Scene.Menu.ToString())
            {
                initMenuScene();
                Model.application.stdin = Model.menuScene["Menu.Game"];
            }
            else if (activeScene == Scene.Mansion.ToString())
            {
                initMansionScene();
                Model.application.stdin = Model.mansionScene["Mansion.Game"];
            }
            else if (activeScene == Scene.SteamVR.ToString())
            {
                initSteamVrScene();
                Model.application.stdin = Model.steamVrScrene["SteamVR.Game"];
            }
        }

        void Start()
        {
            bool isStarted = Model.application.state["started"].getBool();
            if ((SceneManager.GetActiveScene().name != Scene.Loading.ToString() || !isStarted) && audioClipArray != null && audioClipArray.Length > 0)
            {
                audioSource.PlayOneShot(audioClipArray[0]);
            }
            if (activeScene != Scene.Loading.ToString())
            {
                if (!isStarted)
                {
                    Model.application.setState(Model.startGameAction());
                }
            } else
            {
                Model.application.render();
            }

            var ev = Simulation.Schedule<Events.ZoomInCamera>();
        }

        void initLoadingScene()
        {
            if (Model.loadingScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initLoadingScene()");
                }
                Model.loadingScene = new Dictionary<string, Components.Component> {
                        { "Loading.Game", null },
                        { "Loading.Player", null },
                        { "Loading.Navigation", null },
                        { "Loading.Caption", null },
                        { "Loading.Text", null },
                        { "Loading.Menu", null },
                        { "Loading.Test", null }
                    };
                new Wrappers.LoadingWrapper(
                    "Loading.Game",
                    Model.loadingScene,
                    Scene.Loading
                );
                new Wrappers.ComponentWrapper(
                    "Loading.Player",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Player"]
                );
                new Wrappers.ComponentWrapper(
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
                new Wrappers.TestButton(
                    "Loading.Test",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
            }
        }

        void initMenuScene()
        {
            if (Model.menuScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initMenuScene()");
                }
                Model.menuScene = new Dictionary<string, Components.Component> {
                        { "Menu.Game", null },
                        { "Menu.Navigation", null },
                        { "Menu.Caption", null },
                        { "Menu.Quit", null }
                    };
                new Wrappers.GameWrapper(
                    "Menu.Game",
                    Model.menuScene,
                    Scene.Menu
                );
                new Wrappers.ComponentWrapper(
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
                new Wrappers.ExitGameButton(
                    "Menu.Quit",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"]
                );
            }
        }

        void initMansionScene()
        {
            if (Model.mansionScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initMansionScene()");
                }
                Model.mansionScene = new Dictionary<string, Components.Component> {
                    { "Mansion.Game", null },
                };
                new Wrappers.GameWrapper(
                    "Mansion.Game",
                    Model.mansionScene,
                    Scene.Mansion
                );
            }
        }

        void initSteamVrScene()
        {
            if (Model.steamVrScrene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initSteamVrScene()");
                }
                Model.steamVrScrene = new Dictionary<string, Components.Component> {
                        { "SteamVR.Game", null },
                    };
                new Wrappers.GameWrapper(
                    "SteamVR.Game",
                    Model.steamVrScrene,
                    Scene.SteamVR
                );
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
                    Schedule<Events.SceneChange>(1.5f).targetScene = targetScene;
                    Model.application.setState(
                        Model.mergeActions(
                            new List<Dictionary<string, Prop>>() {
                                Model.setSceneAction(Scene.Loading.ToString()),
                                Model.toggleLoadingAction(true)
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
                            Model.setSceneAction(targetScene),
                            Model.toggleLoadingAction(false)
                        }
                    )
                );
            }
        }

        public IEnumerator zoomOutCamera(Camera camera, int from, int to, float delay, Func<Camera, int> callback = null)
        {
            yield return new WaitForSecondsRealtime(delay);
            camera.fieldOfView = from;
            if (from < to)
            {
                yield return zoomOutCamera(camera, from + 2, to, delay, callback);
            }
            else
            {
                StopCoroutine(Model.coroutines["zoomOutCamera"]);
                if (callback != null)
                {
                    callback(camera);
                }
            }
        }

        public IEnumerator zoomInCamera(Camera camera, int from, int to, float delay, Func<Camera, int> callback = null)
        {
            yield return new WaitForSecondsRealtime(delay);
            camera.fieldOfView = from;
            if (from > to)
            {
                yield return zoomInCamera(camera, from - 2, to, delay, callback);
            }
            else
            {
                StopCoroutine(Model.coroutines["zoomInCamera"]);
                if (callback != null)
                {
                    callback(camera);
                }
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