using Engine.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class ExitGameButton
    {
        Components.Button component;
        public ExitGameButton(
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
                    { "value", new Prop("Quit") }
                };
            }
            int callback(Camera camera)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.ExitGameButton.callback()");
                }
                Application.Quit();
                return 0;
            }
            int onClick()
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.ExitGameButton.onClick()");
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