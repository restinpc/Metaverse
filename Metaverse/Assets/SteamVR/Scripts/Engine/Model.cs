using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public static class Model
    {
        public static App application = null;
        public static Game gameModel = null;
        public static Components.Player player = null;
        public static Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();
        public static Dictionary<string, Components.Component> loadingScene = null;
        public static Dictionary<string, Components.Component> menuScene = null;
        public static Dictionary<string, Components.Component> mansionScene = null;
        public static Dictionary<string, Components.Component> steamVrScrene = null;

        public static Dictionary<string, Prop> defaultState()
        {
            Dictionary<string, Prop> fout = new Dictionary<string, Prop>
            {
                { "frameId", new Prop(0) },
                { "activeScene", new Prop(Scene.Loading.ToString()) },
                { "loading", new Prop(false) },
                { "started", new Prop(false) },
                { "paused", new Prop(false) },
                { "gameOver", new Prop(false) },
                { "victory", new Prop(false) },
                { "inputEnabled", new Prop(true) },
            };
            fout.Add("player", new Prop(
                new Dictionary<string, Prop>() {
                    { "position", new Prop(new Vector3(0, 0, 0)) },
                    { "move", new Prop(new Vector3(0, 0, 0)) },
                    { "hp", new Prop(100) },
                    { "tokens", new Prop(100) },
                    { "lives", new Prop(3) },
                    { "dead", new Prop(false) },
                    { "spawn", new Prop(false) }
                }
            ));
            return fout;
        }

        static public Dictionary<string, Prop> mergeActions(List<Dictionary<string, Prop>> values)
        {
            Dictionary<string, Prop> newState = new Dictionary<string, Prop>();
            for(int i = 0; i < values.Count; i++)
            {
                foreach (var field in values[i])
                {
                    newState[field.Key] = field.Value;
                }
            }
            return newState;
        }

        static public Dictionary<string, Prop> setSceneAction(string scene)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.setSceneAction(" + scene + ")");
            }
            return new Dictionary<string, Prop>() {
                { "activeScene", new Prop(scene) }
            };
        }

        static public Dictionary<string, Prop> newGameAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.newGameAction()");
            }
            Dictionary<string, Prop> newState = new Dictionary<string, Prop>(defaultState());
            newState["started"] = new Prop(true);
            return newState;
        }

        static public Dictionary<string, Prop> startGameAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.startGameAction()");
            }
            return new Dictionary<string, Prop>() {
                { "started", new Prop(true) },
            };
        }

        static public Dictionary<string, Prop> pauseGameAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.pauseGameAction()");
            }
            return new Dictionary<string, Prop>() {
                { "paused", new Prop(true) }
            };
        }

        static public Dictionary<string, Prop> continueGameAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.continueGameAction()");
            }
            return new Dictionary<string, Prop>() {
                { "paused", new Prop(false) }
            };
        }

        static public Dictionary<string, Prop> killPlayerAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.killPlayerAction()");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["dead"] = new Prop(true);
            player["lives"] = new Prop(application.state["lives"].getInt() - 1);
            return new Dictionary<string, Prop>() {
                { "gameOver", new Prop(player["lives"].getInt() >= 0) },
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> revivePlayerAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.revivePlayerAction()");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["dead"] = new Prop(false);
            return new Dictionary<string, Prop>() {
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> toggleLoadingAction(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleLoadingAction(" + value + ")");
            }
            return new Dictionary<string, Prop>() {
                { "loading", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleInputAction(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleInputAction(" + value + ")");
            }
            return new Dictionary<string, Prop>() {
                { "enableInput", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleSpawnAction(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleSpawnAction(" + value + ")");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["spawn"] = new Prop(value);
            return new Dictionary<string, Prop>() {
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> gameOverAction()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.gameOverAction()");
            }
            return new Dictionary<string, Prop>() {
                { "gameOver", new Prop(true)}
            };
        }
    }

    public enum Scene
    {
        Loading,
        Menu,
        Mansion,
        SteamVR
    };
}