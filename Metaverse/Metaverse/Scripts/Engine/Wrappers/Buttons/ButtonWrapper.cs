using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class ButtonWrapper
    {
        public ButtonWrapper(string objectName, out Components.Button component, Components.Component parent)
        {
            if (parent.application.DEBUG)
            {
                Debug.Log("Engine.Wrappers.ButtonWrapper.constructor(" + objectName + ")");
            }
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value",  new Prop("test") },
                    { "active", new Prop(
                        state["paused"].getBool()
                        || !state["started"].getBool()
                        || state["gameOver"].getBool()
                    )}
                };
            }
            component = new Components.Button(
                Model.application,
                GameObject.Find(objectName),
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(component);
        }
    }
}