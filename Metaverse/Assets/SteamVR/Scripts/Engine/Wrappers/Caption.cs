using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class Caption
    {
        public Caption(
            string objectName,
            Dictionary<string, Components.Component> level,
            Scene scene,
            Components.Component parent
        ){
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                string text = "Welcome to\n Metaverse";
                bool isGameOver = state["gameOver"].getBool();
                bool isLoading = state["loading"].getBool();
                bool isPaused = state["paused"].getBool();
                bool isDead = state["player"].getDictionary()["dead"].getBool();
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
                    text = "Loading..\nPlease wait";
                }
                return new Dictionary<string, Prop>
                {
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