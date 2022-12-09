using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class LoadingText
    {
        public LoadingText(
            string objectName,
            Dictionary<string, Components.Component> level,
            Scene scene,
            Components.Component parent
        ){
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                bool isGameOver = state["gameOver"].getBool();
                bool isLoading = state["loading"].getBool();
                bool isPaused = state["paused"].getBool();
                bool isStarted = state["started"].getBool();
                bool isDead = state["player"].getDictionary()["dead"].getBool();
                string text = "Are you ready to begin?";
                if (isGameOver)
                {
                    text = "";
                }
                else if (isPaused && !isDead)
                {
                    text = "Press any key to contunue";
                }
                else if (isLoading)
                {
                    text = "Please wait";
                } else if (isStarted)
                {
                    text = "";
                }
                return new Dictionary<string, Prop>
                {
                    { "visible", new Prop(state["activeScene"].getString() == Scene.Loading.ToString()) },
                    { "value", new Prop(text)}
                };
            }
            level[objectName] = new Components.Label(
                Model.application,
                scene,
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(level[objectName]);
        }
    }
}