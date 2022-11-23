using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.EnablePlayerInput.Execute()");
            }
            Model.player.enableInput();
            Model.player.revive();
        }
    }
}