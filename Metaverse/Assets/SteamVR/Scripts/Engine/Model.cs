using System.Collections.Generic;
using UnityEngine;
using System;

namespace Engine
{
    public static class Model
    {
        public static App application = null;
        public static Game gameModel = null;
        public static Components.Player player = null;
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

        static public Dictionary<string, Prop> setScene(string scene)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.setScene(" + scene + ")");
            }
            return new Dictionary<string, Prop>() {
                { "activeScene", new Prop(scene) }
            };
        }

        static public Dictionary<string, Prop> newGame()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.newGame()");
            }
            Dictionary<string, Prop> newState = new Dictionary<string, Prop>(defaultState());
            newState["started"] = new Prop(true);
            return newState;
        }

        static public Dictionary<string, Prop> startGame()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.startGame()");
            }
            return new Dictionary<string, Prop>() {
                { "started", new Prop(true) },
            };
        }

        static public Dictionary<string, Prop> pauseGame()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.pauseGame()");
            }
            return new Dictionary<string, Prop>() {
                { "paused", new Prop(true) }
            };
        }

        static public Dictionary<string, Prop> continueGame()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.continueGame()");
            }
            return new Dictionary<string, Prop>() {
                { "paused", new Prop(false) }
            };
        }

        static public Dictionary<string, Prop> killPlayer()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.killPlayer()");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["dead"] = new Prop(true);
            player["lives"] = new Prop(application.state["lives"].getInt() - 1);
            return new Dictionary<string, Prop>() {
                { "gameOver", new Prop(player["lives"].getInt() >= 0) },
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> revivePlayer()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.revivePlayer()");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["dead"] = new Prop(false);
            return new Dictionary<string, Prop>() {
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> victory()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.victory()");
            }
            return new Dictionary<string, Prop>() {
                { "victory", new Prop(true) }
            };
        }

        static public Dictionary<string, Prop> toggleLoading(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleLoading(" + value + ")");
            }
            return new Dictionary<string, Prop>() {
                { "loading", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleInput(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleInput(" + value + ")");
            }
            return new Dictionary<string, Prop>() {
                { "enableInput", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleSpawn(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleSpawn(" + value + ")");
            }
            Dictionary<string, Prop> player = Model.application.state["player"].getDictionary();
            player["spawn"] = new Prop(value);
            return new Dictionary<string, Prop>() {
                { "player", new Prop(player) }
            };
        }

        static public Dictionary<string, Prop> gameOver()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.gameOver()");
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