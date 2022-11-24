using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    public class SceneChange : Simulation.Event<SceneChange>
    {
        public string targetScene;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.SceneChange.Execute(" + targetScene + ")");
            }
            Model.gameModel.ChangeScene(targetScene);
        }

    }
}
