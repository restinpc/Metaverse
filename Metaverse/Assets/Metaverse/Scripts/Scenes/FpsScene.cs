using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class FpsScene
    {
        public static void Load()
        {
            if (Model.fpsScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initFpsScene()");
                }
                Model.fpsScene = new Dictionary<string, Components.Component> {
                        { "FPS.Game", null },
                        { "FPS.Navigation", null },
                        { "FPS.Navigation.Caption", null },
                        { "FPS.Navigation.Menu", null },
                        { "FPS.Navigation.Mansion", null },
                        { "FPS.Navigation.SteamVR", null },
                        { "FPS.Navigation.Quit", null }
                    };
                new Wrappers.GameWrapper(
                    "FPS.Game",
                    Model.fpsScene,
                    Scene.FPS
                );
                new Wrappers.ComponentWrapper(
                    "FPS.Navigation",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Game"]
                );
                new Wrappers.LabelWrapper(
                    "FPS.Navigation.Caption",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Navigation"],
                    "FPS Game"
                );
                new Wrappers.ButtonWrapper(
                    "FPS.Navigation.Menu",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Navigation"],
                    "Menu",
                    "Menu"
                );
                new Wrappers.ButtonWrapper(
                    "FPS.Navigation.Mansion",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Navigation"],
                    "Mansion",
                    "Mansion"
                );
                new Wrappers.ButtonWrapper(
                    "FPS.Navigation.SteamVR",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Navigation"],
                    "SteamVR",
                    "SteamVR"
                );
                new Wrappers.ExitGameButton(
                    "FPS.Navigation.Quit",
                    Model.fpsScene,
                    Scene.FPS,
                    Model.fpsScene["FPS.Navigation"]
                );
            }
            Model.application.stdin = Model.fpsScene["FPS.Game"];
        }
    }
}