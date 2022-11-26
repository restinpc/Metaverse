using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class Element
    {
        public Element(string objectName, Dictionary<string, Components.Component> level, Components.Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                { };
            }
            level[objectName] = new Components.Component(
                Model.application,
                GameObject.Find(objectName),
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