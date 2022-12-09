using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class LoadingCaption
    {
        public LoadingCaption(
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
                string text = "Welcome to\n Metaverse";
                if (isGameOver)
                {
                    text = "Game Over";
                }
                else if (isPaused && !isDead)
                {
                    text = "Pause";
                }
                else if (isLoading)
                {
                    text = "Loading...";
                } else if (isStarted)
                {
                    text = "Done";
                }
                return new Dictionary<string, Prop>
                {
                    { "visible", new Prop(state["activeScene"].getString() == scene.ToString()) },
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