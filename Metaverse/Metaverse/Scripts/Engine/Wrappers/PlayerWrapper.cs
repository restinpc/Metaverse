using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class PlayerWrapper
    {
        public PlayerWrapper(string objectName, out Components.Player component, Components.Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                { 
                    { "enabled", new Prop(
                        state["paused"].getBool() == false
                        && state["started"].getBool() == true
                        && state["dead"].getBool() == false
                    )},
                    { "dead", new Prop(state["dead"].getBool()) },
                    { "victory", new Prop(state["victory"].getBool()) },
                    { "inputEnabled", new Prop(state["inputEnabled"].getBool()) },
                    { "collider2d", new Prop(state["collider2d"].getBool()) },
                    { "spawn", new Prop(state["spawn"].getBool()) }
                };
            }
            component = new Components.Player(
                Model.application,
                GameObject.Find(objectName),
                parent,
                objectName,
                mapStateToProps
            );
            if (parent != null)
            {
                parent.addChild(component);
            }
        }
    }
}