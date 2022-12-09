using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class LabelWrapper
    {
        public LabelWrapper(
            string objectName,
            Dictionary<string, Components.Component> level,
            Scene scene,
            Components.Component parent,
            string value
        ){
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value", new Prop(value)}
                };
            }
            level[objectName] = new Components.Label(
                Model.application,
                scene,
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(level[objectName]);
        }
    }
}