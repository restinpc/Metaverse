using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class ExitGameButton
    {
        public ExitGameButton(string objectName, out Components.Button component, Scene scane, Components.Component parent)
        {
            if (Engine.Model.application.DEBUG)
            {
                Debug.Log("Engine.Wrappers.ExitGameButton.constructor(" + objectName + ")");
            }
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value", new Prop("Exit Game") },
                    { "active", new Prop(
                        state["paused"].getBool()
                        || !state["started"].getBool()
                        || state["gameOver"].getBool()
                    )}
                };
            }
            UnityEngine.UI.Button button = GameObject.Find(objectName).GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => this.Quit());
            component = new Components.Button(
                Model.application,
                GameObject.Find(objectName),
                scane,
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(component);
        }

        public void Quit()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Wrappers.ExitGameButton.Quit()");
            }
            Application.Quit();
        }
    }
}