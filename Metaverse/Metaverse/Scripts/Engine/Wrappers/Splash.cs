using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class Splash
    {
        public Splash(string objectName, out Components.Component component, Components.Component parent)
        {
            Dictionary<string, Prop> splashMapStateToProps (Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "active", 
                        new Prop(
                            state["paused"].getBool() 
                            || !state["started"].getBool()
                            || state["gameOver"].getBool()
                            || state["dead"].getBool()
                        )
                    }
                };
            }
            component = new Components.Component(
                Model.application,
                GameObject.Find(objectName),
                parent,
                objectName,
                splashMapStateToProps
            );
            parent.addChild(component);
            RectTransform splashRectTransform = component.gameObject.GetComponent<RectTransform>();
            splashRectTransform.sizeDelta = new Vector2(0, 0);
        }
    }
}