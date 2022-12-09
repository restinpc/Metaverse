using Engine.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class NewGameButton
    {
        Components.Button component;
        public NewGameButton(
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
                    { "value", new Prop("Start")},
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
            int callback(Camera camera)
            {
                Simulation.Schedule<Events.SceneChange>().targetScene = "Menu";
                return 0;
            }
            int onClick() {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.NewGameButton.onClick()");
                }
                var ev = Simulation.Schedule<Events.ZoomOutCamera>(0.01f);
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