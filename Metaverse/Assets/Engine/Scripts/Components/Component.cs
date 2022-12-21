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
        public string id;
        public App application;
        public string name;
        public Component parent;
        public Dictionary<string, Component> nodes;
        public GameObject gameObject;
        public Scene scene;
        public Type elementType;
        public int renderId = 0;
        public Func<
            Dictionary<string, Prop>,
            Dictionary<string, Prop>
        > mapStateToProps;
        public Dictionary<string, Prop> props = new Dictionary<string, Prop>() {
            { "visible", null }
        };
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
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) {
            if (application.DEBUG && name.Length > 0)
            {
                Debug.Log("Component.constructor(" + name + ")");
            }
            this.name = name.Length > 0 ? name : "";
            this.scene = scene;
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
        void updateProps()
        {
            this.props = this.mapStateToProps(this.application.state);
        }
        /**
         * Method to update child nodes while capturing.
         * @param fout Target HTML Element.
         */
        void fallback(GameObject fout) {

            foreach(KeyValuePair<string, Component> el in this.getNodes())
            {
                el.Value.render(this.gameObject);
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
                if (this.application.DEBUG && this.name.Length > 0)
                {
                    Debug.Log("Engine.Component(" + this.name + ").render(" + this.renderId + ")");
                }
                if (!gameObject)
                {
                    gameObject = GameObject.Find(this.name);
                }
                this.updateProps();
                foreach (var item in this.props)
                {
                    if (item.Key == "visible")
                    {
                        gameObject.SetActive(item.Value.getBool() == true);
                    }
                }
                this.fallback(gameObject);
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
            if (this.application.DEBUG && this.name.Length > 0 && child.name.Length > 0)
            {
                Debug.Log("Engine.Component(" + this.name + ").addChild(" + child.name + ")");
            }
            this.nodes.Add(child.name, child);
            if (this.getNodes().ContainsKey(child.name) && child.gameObject)
            {
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
            if (this.application.DEBUG && this.name.Length > 0)
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

        public virtual void Start() {}

        public virtual void Update() {}

        public virtual void FixedUpdate() {}

        public virtual void LateUpdate(){}

        public virtual void OnGUI() {}

        public virtual void OnDisable() {}
        public virtual void OnEnable() {}
    }
}