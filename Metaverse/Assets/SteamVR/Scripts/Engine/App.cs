using Valve.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Engine
{
    /**
     * @property stdout
     * @property stdin
     */
    public class App
    {
        public bool DEBUG = true;
        public bool DEEP_DEBUG = false;
        public GameObject stdout;
        public Components.Component stdin;
        public Dictionary<string, Components.Component> list;
        public int renderId;
        public Dictionary<string, Prop> state;
        public App()
        {
            if (DEBUG)
            {
                Debug.Log("App.constructor()");
            }
            this.state = Model.defaultState();
            this.renderId = 0;
            this.list = new Dictionary<string, Components.Component> { };
        }

        /**
         * Method to get a list of all child nodes.
         * @param arr Initial array of nodes.
         */
        private List<Components.Component> tree(Dictionary<string, Components.Component> arr)
        {
            try
            {
                bool flag = false;
                Dictionary<string, Components.Component> fout = new Dictionary<string, Components.Component>(arr);
                foreach (KeyValuePair<string, Components.Component> el in arr)
                {
                    Dictionary<string, Components.Component> nodes = el.Value.getNodes();
                    foreach (KeyValuePair<string, Components.Component> node in nodes)
                    {
                        if (!arr.ContainsKey(node.Key))
                        {
                            flag = true;
                            fout.Add(node.Key, node.Value);
                        }
                    }
                }
                if (flag)
                {
                    return this.tree(fout);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.App.tree() -> " + e.Message);
            }
            return new List<Components.Component>(arr.Values);
        }

        /**
         * Method to get match 2 dictionaries by values.
         * @param dictionary1.
         * @param dictionary2.
         */
        private bool matchDictionaries(Dictionary<string, Prop> dictionary1, Dictionary<string, Prop> dictionary2)
        {
            bool flag = false;
            foreach (var prop in dictionary1)
            {
                if (prop.Value.getType() == typeof(bool) && prop.Value.getBool() != dictionary2[prop.Key].getBool())
                {
                    flag = true;
                    break;
                }
                else if (prop.Value.getType() == typeof(int) && prop.Value.getInt() != dictionary2[prop.Key].getInt())
                {
                    flag = true;
                    break;
                }
                else if (prop.Value.getType() == typeof(string) && prop.Value.getString() != dictionary2[prop.Key].getString())
                {
                    flag = true;
                    break;
                }
                else if (prop.Value.getType() == typeof(Vector3) && prop.Value.getVector3() != dictionary2[prop.Key].getVector3())
                {
                    flag = true;
                    break;
                }
                else if (prop.Value.getType() == typeof(Dictionary<string, Prop>) && prop.Value.getDictionary() != dictionary2[prop.Key].getDictionary())
                {
                    flag = this.matchDictionaries(prop.Value.getDictionary(), dictionary2[prop.Key].getDictionary());
                    if (flag)
                    {
                        break;
                    }
                }
                else if (prop.Value.getType() == typeof(Component) && prop.Value.getComponent() != dictionary2[prop.Key].getComponent())
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /**
         * @param state
         * @param logToConsole Flag to force output debug data to console
         */
        public void setState(Dictionary<string, Prop> state, bool logToConsole = true)
        {
            if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
            {
                Debug.Log($"Before dispatch >> " + "\n<color=orange>" + JsonConvert.SerializeObject(this.state) + "</color>");
                Debug.Log($"Will dispatch >> " + "\n<color=yellow>" + JsonConvert.SerializeObject(state) + "</color>");
            }
            bool rerender = true;
            string currentScene = this.state["activeScene"].getString();
            if (state.ContainsKey("activeScene") && !currentScene.Equals(state["activeScene"].getString()))
            {
                string targetScene = state["activeScene"].getString();
                if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
                {
                    Debug.Log("Scene is changing from <" + currentScene + "> to <" + targetScene + ">");
                }
                SceneManager.LoadScene(targetScene);
                rerender = false;
            }
            Dictionary<string, Prop> newState = new Dictionary<string, Prop>();
            foreach (var field in this.state)
            {
                if (state.ContainsKey(field.Key))
                {
                    newState[field.Key] = state[field.Key];
                } else
                {
                    newState[field.Key] = this.state[field.Key];
                }
            }
            newState["frameId"] = new Prop(this.state["frameId"].getInt() + 1);
            this.state = newState;
            if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
            {
                Debug.Log($"After dispatch >> " + "\n<color=orange>" + JsonConvert.SerializeObject(this.state) + "</color>");
            }
            foreach (KeyValuePair<string, Components.Component> obj in this.list)
            {
                Dictionary<string, Prop> afterProps = obj.Value.getUpdatedProps();
                if (!matchDictionaries(obj.Value.props, afterProps) && rerender)
                {
                    List<Components.Component> treeList = tree(this.stdin.getNodes());
                    if (treeList.IndexOf(obj.Value) >= 0 || obj.Value == this.stdin)
                    {
                        obj.Value.render(obj.Value.parent != null ? obj.Value.parent.gameObject : this.stdout);
                    }
                }
            }
        }

        public Components.Component getComponentByName(string name)
        {
            if (this.list.ContainsKey(name))
            {
                return this.list[name];
            }
            return null;
        }

        /**
         * Method to rebuild while Virtual DOM
         */
        public void render()
        {
            this.renderId++;
            try
            {
                if (DEBUG)
                {
                    Debug.Log("App.render(" + this.renderId + ")");
                }
                this.stdin.render(this.stdout);
            }
            catch (Exception e)
            {
                Debug.LogError("App.render(" + this.renderId + ") -> " + e.Message);
            }
        }
    }
}