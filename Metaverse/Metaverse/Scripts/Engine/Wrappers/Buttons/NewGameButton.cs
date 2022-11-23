using Platformer.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Wrappers
{
    public class NewGameButton
    {
        public NewGameButton(string objectName, out Components.Button component, Components.Component parent)
        {
            if (parent.application.DEBUG)
            {
                Debug.Log("Engine.Wrappers.NewGameButton.constructor(" + objectName+")");
            }
            Dictionary<string, Prop> mapStateToProps(Dictionary<string, Prop> state)
            {
                return new Dictionary<string, Prop>
                {
                    { "value", new Prop("New Game (1)")},
                    { "active", new Prop(
                        state["paused"].getBool()
                        || !state["started"].getBool()
                        || state["gameOver"].getBool()
                    )}
                };
            }
            UnityEngine.UI.Button button = GameObject.Find(objectName).GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => this.onClick(button));
            component = new Components.Button(
                Model.application,
                GameObject.Find(objectName),
                parent,
                objectName,
                mapStateToProps
            );
            parent.addChild(component);
        }

        void onClick(UnityEngine.UI.Button sender)
        {
            Model.application.setState(Model.newGame());
            Simulation.Schedule<Engine.Gameplay.PlayerSpawn>();
        }
    }
}