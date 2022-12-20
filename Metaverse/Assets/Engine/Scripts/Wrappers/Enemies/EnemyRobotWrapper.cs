using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class EnemyRobotWrapper
    {
        public EnemyRobotWrapper(string objectName, Dictionary<string, Components.Component> level, Scene scene, Components.Component parent = null)
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "visible", new Prop(scene.ToString() == state["activeScene"].getString()) }
                };
            }
            level[objectName] = new Components.EnemyRobot(
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