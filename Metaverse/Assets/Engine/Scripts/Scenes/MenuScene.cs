using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class MenuScene
    {
        public static void Load()
        {
            if (Model.menuScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.MenuScene()");
                }
                Model.menuScene = new Dictionary<string, Components.Component> {
                    { "Menu.Game", null },
                    { "Menu.Navigation", null },
                    { "Menu.Navigation.Caption", null },
                    { "Menu.Navigation.Mansion", null },
                    { "Menu.Navigation.SteamVR", null },
                    { "Menu.Navigation.FPS", null },
                    { "Menu.Navigation.Quit", null }
                };
                new Wrappers.GameWrapper(
                    "Menu.Game",
                    Model.menuScene,
                    Scene.Menu
                );
                new Wrappers.ComponentWrapper(
                    "Menu.Navigation",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Game"]
                );
                new Wrappers.LabelWrapper(
                    "Menu.Navigation.Caption",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"],
                    "Main Menu"
                );
                new Wrappers.ButtonWrapper(
                    "Menu.Navigation.Mansion",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"],
                    "Mansion",
                    "Mansion"
                );
                new Wrappers.ButtonWrapper(
                    "Menu.Navigation.SteamVR",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"],
                    "SteamVR",
                    "SteamVR"
                );
                new Wrappers.ButtonWrapper(
                    "Menu.Navigation.FPS",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"],
                    "FPS",
                    "FPS"
                );
                new Wrappers.ExitGameButton(
                    "Menu.Navigation.Quit",
                    Model.menuScene,
                    Scene.Menu,
                    Model.menuScene["Menu.Navigation"]
                );
            }
            Model.application.stdin = Model.menuScene["Menu.Game"];
        }
    }
}