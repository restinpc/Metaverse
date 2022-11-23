using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// Fired when the player performs a Jump.
    /// </summary>
    /// <typeparam name="PlayerJumped"></typeparam>
    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
        public PlayerController player;

        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerJumped.Execute()");
            }
            Model.player.jump();
        }
    }
}