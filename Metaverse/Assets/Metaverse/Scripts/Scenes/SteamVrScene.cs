using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class SteamVrScene
    {
        public static void Load()
        {
            if (Model.steamVrScrene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initSteamVrScene()");
                }
                Model.steamVrScrene = new Dictionary<string, Components.Component> {
                    { "SteamVR.Game", null },
                };
                new Wrappers.GameWrapper(
                    "SteamVR.Game",
                    Model.steamVrScrene,
                    Scene.SteamVR
                );
            }
            Model.application.stdin = Model.steamVrScrene["SteamVR.Game"];
        }
    }
}