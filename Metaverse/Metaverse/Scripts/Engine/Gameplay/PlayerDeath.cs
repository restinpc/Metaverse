using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerDeath.Execute()");
            }
            Model.player.death();
        }

    }
}
