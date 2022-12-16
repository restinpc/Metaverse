using System;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEditor;

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
    public class Button : Component
    {
        UnityEngine.UI.Button button;
        UnityEngine.UI.Text textObject;
        Func<int> onClick = null;
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Button(
            App application,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<Dictionary<string, Prop>, Dictionary<string, Prop>> mapStateToProps = null,
            Func<int> onClick = null
        ) : base(application, scene, parent, name, mapStateToProps) {
            this.onClick = onClick;
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
                if (gameObject.activeSelf)
                {
                    if (button == null)
                    {
                        button = GameObject.Find(name).GetComponent<UnityEngine.UI.Button>();
                        textObject = button.gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
                        if (this.onClick != null)
                        {
                            button.onClick.AddListener(() => this.onClick());
                        }
                    }
                    textObject.text = this.props["value"].getString();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Button(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }
    }
}