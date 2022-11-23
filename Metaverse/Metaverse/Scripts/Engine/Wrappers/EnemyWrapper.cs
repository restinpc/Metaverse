using System.Collections.Generic;
using UnityEngine;
using Component = Engine.Components.Component;

namespace Engine.Wrappers
{
    public class EnemyWrapper
    {
        public EnemyWrapper(string objectName, List<Component> components, Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                { { "enabled", new Prop(!state["paused"].getBool()) } };
            }
            Component fout = new Components.Enemy(
                Model.application,
                GameObject.Find(objectName),
                parent,
                objectName,
                mapStateToProps
            );
            components.Add(fout);
            if (parent != null)
            {
                parent.addChild(fout);
            }
        }
    }
}