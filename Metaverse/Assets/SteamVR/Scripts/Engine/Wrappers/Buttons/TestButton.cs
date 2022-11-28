using Platformer.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class TestButton
    {
        Components.Button component;
        public TestButton(
            string objectName,
            Dictionary<string, Components.Component> level,
            Scene scene,
            Components.Component parent
        )
        {
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value", new Prop("Test")},
                    { "visible", new Prop(
                        state["activeScene"].getString() == scene.ToString() && 
                        (
                            state["paused"].getBool()
                            || !state["started"].getBool()
                            || state["gameOver"].getBool()
                        ))
                    }
                };
            }

            int callback(Camera camera) {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.TestButton.callback()");
                }
                var ev = Simulation.Schedule<Gameplay.ZoomInCamera>();
                ev.objectName = "VRCamera";
                ev.fallbackObjectName = "FallbackObjects";
                return 0;
            }

            int onClick() {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.TestButton.onClick()");
                }
                var ev = Simulation.Schedule<Gameplay.ZoomOutCamera>();
                ev.objectName = "VRCamera";
                ev.fallbackObjectName = "FallbackObjects";
                ev.callback = callback;
                return 0;
            }
            component = new Components.Button(
                Model.application,
                scene,
                parent,
                objectName,
                mapStateToProps,
                onClick
            );
            parent.addChild(component);
            level[objectName] = component;
        }
    }
}