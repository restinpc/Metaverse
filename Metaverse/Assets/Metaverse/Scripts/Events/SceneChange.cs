using Engine.Core;
using UnityEngine;

namespace Engine.Events
{
    public class SceneChange : Simulation.Event<SceneChange>
    {
        public string targetScene;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Events.SceneChange.Execute(" + targetScene + ")");
            }
            Model.gameModel.ChangeScene(targetScene);
        }

    }
}
