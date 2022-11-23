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
     * @property label TextMeshProUGUI element.
     */
    public class Label : Component
    {
        UnityEngine.UI.Text label;
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Label(
            App application,
            GameObject gameObject,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, gameObject, scene, parent, name, mapStateToProps)
        {
            this.label = gameObject.GetComponent<UnityEngine.UI.Text>();
        }
        /**
         * Method to output html content to parent node.
         * @param stdout HTML Element to output.
         */
        public override void render(GameObject stdout)
        {
            base.render(stdout);
            try
            {
                // Debug.Log(this.props["value"].getString());
                // this.label.text = "123";
                this.label.text = this.props["value"].getString();
                // Debug.Log(this.label.text);
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Label(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }
    }
}