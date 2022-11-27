using Platformer.Core;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// todo
    /// </summary>
    public class ZoomInCamera : Simulation.Event<ZoomInCamera>
    {
        public string objectName;
        public int from = 160;
        public int to = 60;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.ZoomInCamera.Execute(" + objectName + ")");
            }
            Camera camera = GameObject.Find(objectName).GetComponent<Camera>();
            Model.coroutines["zoomInCamera"] = Model.gameModel.StartCoroutine(Model.gameModel.zoomInCamera(camera, from, to, 0.001f));
        }
    }
}