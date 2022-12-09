using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Scenes
{
    public class LoadingScene
    {
        public static void Load()
        {
            if (Model.loadingScene == null)
            {
                if (Model.application.DEBUG)
                {
                    Debug.Log("Engine.Game.initLoadingScene()");
                }
                Model.loadingScene = new Dictionary<string, Components.Component> {
                        { "Loading.Game", null },
                        { "Loading.Player", null },
                        { "Loading.Navigation", null },
                        { "Loading.Navigation.Caption", null },
                        { "Loading.Navigation.Text", null },
                        { "Loading.Navigation.Menu", null },
                        { "Loading.Navigation.Test", null }
                    };
                new Wrappers.LoadingWrapper(
                    "Loading.Game",
                    Model.loadingScene,
                    Scene.Loading
                );
                new Wrappers.ComponentWrapper(
                    "Loading.Player",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Player"]
                );
                new Wrappers.ComponentWrapper(
                    "Loading.Navigation",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Game"]
                );
                new Wrappers.LoadingCaption(
                    "Loading.Navigation.Caption",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
                new Wrappers.LoadingText(
                    "Loading.Navigation.Text",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
                new Wrappers.NewGameButton(
                    "Loading.Navigation.Menu",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
                new Wrappers.TestButton(
                    "Loading.Navigation.Test",
                    Model.loadingScene,
                    Scene.Loading,
                    Model.loadingScene["Loading.Navigation"]
                );
            }
            Model.application.stdin = Model.loadingScene["Loading.Game"];
        }
    }
}