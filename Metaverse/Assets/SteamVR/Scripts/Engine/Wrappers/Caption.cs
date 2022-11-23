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
                if (state["gameOver"].getBool())
                {
                    text = "Game Over";
                }
                else if (state["paused"].getBool() && !state["dead"].getBool())
                {
                    text = "Pause";
                } else if (state["victory"].getBool())
                {
                    text = "Victory";
                } else if (state["lives"].getInt() > 0 && state["started"].getBool())
                {
                    text = state["lives"].getInt() + " / 3";
                }
                return new Dictionary<string, Prop>
                {
                    { "enabled", new Prop(state["activeScene"].getString() == Scene.Loading.ToString()) },
                    { "value", new Prop(text)}
                };
            }
            level[objectName] = new Components.Label(
                Model.application,
                GameObject.Find(objectName),
                scene,
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(level[objectName]);
        }
    }
}