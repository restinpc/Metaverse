using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class FpsScene
    {
        public static void Load()
        {
            if (Model.fpsScrene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initFpsScene()");
                }
                Model.fpsScrene = new Dictionary<string, Components.Component> {
                        { "FPS.Game", null },
                    };
                new Wrappers.GameWrapper(
                    "FPS.Game",
                    Model.fpsScrene,
                    Scene.FPS
                );
            }
            Model.application.stdin = Model.fpsScrene["FPS.Game"];
        }
    }
}