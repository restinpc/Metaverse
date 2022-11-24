using Valve.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Platformer.Core.Simulation;

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
            try
            {
                if (DEBUG)
                {
                    Debug.Log("App.constructor()");
                }
                this.state = Model.defaultState();
                this.renderId = 0;
                this.list = new Dictionary<string, Components.Component> { };
                // this.render(); 
            }
            catch (Exception e)
            {
                Debug.LogError("App.constructor() -> " + e.Message);
            }
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

        /**
         * Method to get a list of all child nodes.
         * @param arr Initial array of nodes.
         */
        public List<Components.Component> tree(Dictionary<string, Components.Component> arr)
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
         * @param name
         * @param element
         */
        public Components.Component register(string name, MonoBehaviour element, Type type)
        {
            if (DEBUG)
            {
                Debug.Log("Engine.App.register(<" + name + " " + type + " />)");
            }
            Components.Component fout = this.list[name];
            fout.element = element;
            fout.elementType = type;
            return fout;
        }


        public bool matchDictionaries(Dictionary<string, Prop> dictionary1, Dictionary<string, Prop> dictionary2)
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
         */
        public void setState(Dictionary<string, Prop> state, bool logToConsole = true)
        {
            if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
            {
                Debug.Log("Before dispatch >> " + "\n" + JsonConvert.SerializeObject(this.state));
                Debug.Log("Will dispatch >> " + "\n" + JsonConvert.SerializeObject(state));
            }
            string currentScene = this.state["activeScene"].getString();
            if (state.ContainsKey("activeScene") && !currentScene.Equals(state["activeScene"].getString()))
            {
                string targetScene = state["activeScene"].getString();
                if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
                {
                    Debug.Log("Scene is changing from <" + currentScene + "> to <" + targetScene + ">");
                }
                SceneManager.LoadScene(targetScene);
            }
            Dictionary<string, Prop> newState = new Dictionary<string, Prop>();
            foreach (var field in this.state)
            {
                try
                {
                    newState[field.Key] = state[field.Key];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    newState[field.Key] = this.state[field.Key];
                }
            }
            newState["frameId"] = new Prop(this.state["frameId"].getInt() + 1);
            this.state = newState;
            if ((DEBUG && logToConsole) || (DEBUG && DEEP_DEBUG))
            {
                Debug.Log("After dispatch >> " + "\n" + JsonConvert.SerializeObject(this.state));
            }
            foreach (KeyValuePair<string, Components.Component> obj in this.list)
            {
                // string before = JsonConvert.SerializeObject(obj.Value.props);
                Dictionary<string, Prop> afterProps = obj.Value.getUpdatedProps();
                // string after = JsonConvert.SerializeObject(obj.Value.getUpdateProps());
                if (!matchDictionaries(obj.Value.props, afterProps))
                {
                    List<Components.Component> treeList = tree(this.stdin.getNodes());
                    if (treeList.IndexOf(obj.Value) >= 0 || obj.Value == this.stdin)
                    {
                        obj.Value.render(obj.Value.parent != null ? obj.Value.parent.gameObject : this.stdout);
                    }
                }
                obj.Value.updateProps();
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
    }
}