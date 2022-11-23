using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.PlayerSpawn.Execute()");
            }
            Model.player.spawn();
        }
    }
}
