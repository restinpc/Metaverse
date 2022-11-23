using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Components
{
    /**
     * @property application Application object.
     * @property name Component name.
     * @property parent Virtual DOM parent node.
     * @property nodes Array with a child nodes.
     * @property gameObject Unity game object.
     * @property renderId Render method executions counter (debug tool).
     * @property mapStateToProps Function to map object properties from application state container.
     */
    public class Component
    {
        public bool DEBUG = true;
        public string id;
        public App application;
        public string name;
        public Component parent;
        public Dictionary<string, Component> nodes;
        public GameObject gameObject;
        public MonoBehaviour element;
        public Type elementType;
        public int renderId = 0;
        public Func<
            Dictionary<string, Prop>,
            Dictionary<string, Prop>
        > mapStateToProps;
        public Dictionary<string, Prop> props;
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Component name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Component(
            App application,
            GameObject gameObject,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) {
            if (DEBUG && name.Length > 0)
            {
                Debug.Log("Component.constructor(" + name + ")");
            }
            this.name = name.Length > 0 ? name : "";
            this.gameObject = gameObject;
            this.application = application;
            this.parent = parent;
            if (mapStateToProps != null)
            {
                this.props = mapStateToProps(application.state);
            } else {
                this.props = new Dictionary<string, Prop> { };
            }
            this.mapStateToProps = mapStateToProps;
            this.nodes = new Dictionary<string, Component> { };
            this.application.list.Add(this.name, this);
        }
        /**
         * Method to return child node list.
         */
        public Dictionary<string, Component> getNodes()
        {
            return this.nodes;
        }

        public Dictionary<string, Prop> getUpdatedProps()
        {
            return this.mapStateToProps(this.application.state);
        }

        /**
         * Method to update component props.
         */
        public void updateProps()
        {
            this.props = this.mapStateToProps(this.application.state);
        }
        /**
         * Method to update child nodes while capturing.
         * @param fout Target HTML Element.
         */
        public void fallback(GameObject fout) {

            foreach(KeyValuePair<string, Component> el in this.getNodes())
            {
                el.Value.render(this.gameObject);
            }
        }
        /**
         * Method to output element into DOM.
         * @param stdout Parent Node.
         * 
        public void output(GameObject stdout)
        {
            try
            {
                bool flag = false;
                if (stdout != null)
                {
                    foreach (Transform child in stdout.transform.GetComponentsInChildren<Transform>())
                    {
                        if (child.name == this.gameObject.name)
                        {
                            this.gameObject.transform.SetParent(child.transform.parent);
                            child.transform.parent = null;
                            flag = true;
                        }
                    }
                    if (!flag && !this.gameObject.transform.parent)
                    {
                        this.gameObject.transform.SetParent(stdout.transform);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Component(" + this.name + ").output(" + this.renderId + ") -> " + e.Message);
            }
        }
        /**
         * Method to output html content to parent node.
         * @param stdout HTML Element to output.
         */
        public virtual void render(GameObject stdout)
        {
            this.renderId++;
            try
            {
                if (DEBUG && this.name.Length > 0)
                {
                    Debug.Log("Engine.Component(" + this.name + ").render(" + this.renderId + ")");
                }
                this.updateProps();
                foreach (var item in this.props)
                {
                    if (item.Key == "active")
                    {
                        this.gameObject.SetActive(item.Value.getBool() == true);
                    }
                }
                this.fallback(this.gameObject);
                /**
                Object fout = this.gameObject;
                Object.keys(this.props).forEach((key: string) => {
                    if (key === "innerHTML") {
                        fout.innerHTML = this.props[key];
                    } else if (this.props[key]) {
                        fout.setAttribute(key, this.props[key]);
                    }
                });
                if (this.name) {
                    fout.setAttribute("name", this.name);
                }
                this.fallback(fout);
                this.output(stdout);
                */
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Component(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }
        /**
         * Method to add new node.
         * @param child Target element.
         */
        public void addChild(Component child)
        {
            if (DEBUG && this.name.Length > 0 && child.name.Length > 0)
            {
                Debug.Log("Engine.Component(" + this.name + ").addChild(" + child.name + ")");
            }
            this.nodes.Add(child.name, child);
            if (this.getNodes().ContainsKey(child.name) && child.gameObject)
            {
                Debug.Log("it");
                Debug.Log(child.gameObject.transform);
                Debug.Log(this);
                Debug.Log(this.gameObject);
                child.gameObject.transform.SetParent(this.gameObject.transform);
            }
            if (!this.application.list.ContainsKey(child.name))
            {
                this.application.list.Add(child.name, child);
            }
        }
        /**
         * Method to remove a child node.
         * @param child Target element.
         */
        public void removeChild(Component child)
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Component(" + this.name + ").removeChild(" + child.name + ")");
            }
            Dictionary<string, Component> arr = new Dictionary<string, Component> { };
            foreach (KeyValuePair<string, Component> node in this.nodes)
            {
                if (node.Value.name != child.name)
                {
                    arr.Add(node.Value.name, node.Value);
                }
            }
            this.nodes = arr;
            child.gameObject.transform.parent = null;
            this.application.list.Remove(child.name);
        }
    }
}