using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class PlayerWrapper
    {
        public PlayerWrapper(string objectName, out Components.Player component, Scene scene, Components.Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                { 
                    { "dead", new Prop(state["player"].getDictionary()["dead"].getBool()) },
                    { "spawn", new Prop(state["player"].getDictionary()["spawn"].getBool()) },
                };
            }
            component = new Components.Player(
                Model.application,
                scene,
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