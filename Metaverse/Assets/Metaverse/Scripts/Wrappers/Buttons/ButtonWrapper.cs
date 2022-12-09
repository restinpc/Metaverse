using Engine.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class ButtonWrapper
    {
        public ButtonWrapper(string objectName, Dictionary<string, Components.Component> level, Scene scene, Components.Component parent, string value, string targetScene)
        {
            if (parent.application.DEBUG)
            {
                Debug.Log("Engine.Wrappers.ButtonWrapper.constructor(" + objectName + ")");
            }
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value",  new Prop(value) },
                };
            }
            int callback(Camera camera)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.ButtonWrapper.callback()");
                }
                Simulation.Schedule<Events.SceneChange>(0.7f).targetScene = targetScene;
                return 0;
            }
            int onClick()
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Wrappers.ButtonWrapper.onClick(" + targetScene + ")");
                }
                Simulation.Schedule<Events.ZoomOutCamera>(0.01f).callback = callback;
                return 0;
            }
            level[objectName] = new Components.Button(
                Model.application,
                scene,
                parent,
                objectName,
                mapStateToProps,
                onClick
            );
            parent.addChild(level[objectName]);
        }
    }
}