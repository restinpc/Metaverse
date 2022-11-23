using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// Fired when a player enters a trigger with a DeathZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredDeathZone"></typeparam>
    public class PlayerEnteredDeathZone : Simulation.Event<PlayerEnteredDeathZone>
    {
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerEnteredDeathZone.Execute()");
            }
            Model.player.death();
        }
    }
}