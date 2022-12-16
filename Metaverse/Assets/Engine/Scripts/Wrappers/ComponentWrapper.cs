using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class ComponentWrapper
    {
        public ComponentWrapper(string objectName, Dictionary<string, Components.Component> level, Scene scene, Components.Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                { };
            }
            level[objectName] = new Components.Component(
                Model.application,
                scene,
                parent,
                objectName,
                mapStateToProps
            );
            if (parent != null)
            {
                parent.addChild(level[objectName]);
            }
        }
    }
}