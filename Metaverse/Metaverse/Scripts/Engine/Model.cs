using System.Collections.Generic;
using UnityEngine;
using System;

namespace Engine
{
    public static class Model
    {
        public static App application = null;
        public static Game gameModel = null;
        public static Components.Label caption;
        public static Components.Component canvas;
        public static Components.Component splash;
        public static Components.Player player;
        public static Components.Button newGameButton;
        public static Components.Button exitGameButton;
        public static Dictionary<string, Components.Component> loadingLevel;

        public static Dictionary<string, Prop> defaultState()
        {
            Dictionary<string, Prop> fout = new Dictionary<string, Prop>
            {
                { "started", new Prop(false) },
                { "paused", new Prop(false) },
                { "gameOver", new Prop(false) },
                { "frameId", new Prop(0) },
                { "lives", new Prop(3) },
                { "dead", new Prop(false) },
                { "victory", new Prop(false) },
                { "inputEnabled", new Prop(true) },
                { "collider2d", new Prop(true) },
                { "spawn", new Prop(false) }
            };
            /*
            Dictionary<string, Prop> enemies = new Dictionary<string, Prop>();
            for (int i = 1; i <= 16; i++)
            {
                Dictionary<string, Prop> enemy = new Dictionary<string, Prop> { };
                enemy.Add("id", new Prop(i));
                enemy.Add("name", new Prop("Enemy" + i.ToString()));
                enemy.Add("move", new Prop(new Vector2(0, 0)));
            }
            fout.Add("enemies", new Prop(enemies));
            */
            return fout;
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
                { "started", new Prop(true) }
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
            return new Dictionary<string, Prop>() {
                { "lives", new Prop(application.state["lives"].getInt() - 1)},
                { "dead", new Prop(true) }
            };
        }

        static public Dictionary<string, Prop> revivePlayer()
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.revivePlayer()");
            }
            return new Dictionary<string, Prop>() {
                { "dead", new Prop(false) },
                { "paused", new Prop(false) }
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

        static public Dictionary<string, Prop> toggleInput(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleInput()");
            }
            return new Dictionary<string, Prop>() {
                { "enableInput", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleCollider2d(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleCollider2d()");
            }
            return new Dictionary<string, Prop>() {
                { "collider2d", new Prop(value) }
            };
        }

        static public Dictionary<string, Prop> toggleSpawn(bool value)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.toggleSpawn()");
            }
            return new Dictionary<string, Prop>() {
                { "spawn", new Prop(value) }
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

        static public Dictionary<string, Prop> initEnemy(string enemyName, Components.Component component)
        {
            if (application.DEBUG)
            {
                Debug.Log("Engine.Model.initEnemies(" + enemyName + ")");
            }
            Dictionary<string, Prop> enemies = Model.application.state["enemies"].getDictionary();
            enemies[enemyName].getDictionary()["component"] = new Prop(component);
            return new Dictionary<string, Prop>() {
                { "enemies", new Prop(enemies) }
            };
        }

        static public Dictionary<string, Prop> setEnemyMove(string enemyName, Vector2 move)
        {
            if (application.DEBUG && application.DEEP_DEBUG)
            {
                Debug.Log("Engine.Model.setEnemyMove("+ enemyName + ")");
            }
            Dictionary<string, Prop> enemies = Model.application.state["enemies"].getDictionary();
            enemies[enemyName].getDictionary()["move"] = new Prop(move);
            return new Dictionary<string, Prop>() {
                { "enemies", new Prop(enemies) }
            };
        }
    }
}