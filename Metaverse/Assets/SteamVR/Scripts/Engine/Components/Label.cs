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
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, scene, parent, name, mapStateToProps)
        {
            base.constructor();
        }
        /**
         * Method to output html content to parent node.
         * @param stdout HTML Element to output.
         */
        public override void render(GameObject stdout)
        {
            base.render(stdout);
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Label(" + this.name + ").render(" + this.renderId + ")");
            }
            try
            {
                gameObject.GetComponent<UnityEngine.UI.Text>().text = this.props["value"].getString();
                Debug.Log(gameObject.GetComponent<UnityEngine.UI.Text>().text);
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Label(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }
    }
}