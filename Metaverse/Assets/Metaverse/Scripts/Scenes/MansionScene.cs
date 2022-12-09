using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class MansionScene
    {
        public static void Load()
        {
            if (Model.mansionScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initMansionScene()");
                }
                Model.mansionScene = new Dictionary<string, Components.Component> {
                    { "Mansion.Game", null },
                };
                new Wrappers.GameWrapper(
                    "Mansion.Game",
                    Model.mansionScene,
                    Scene.Mansion
                );
            }
            Model.application.stdin = Model.mansionScene["Mansion.Game"];
        }
    }
}