using Platformer.Core;
using System;
using UnityEngine;

namespace Engine.Gameplay
{
    /// <summary>
    /// todo
    /// </summary>
    public class ZoomOutCamera : Simulation.Event<ZoomInCamera>
    {
        public string objectName;
        public int from = 60;
        public int to = 170;
        public Func<Camera, int> callback = null;
        public override void Execute()
        {
            if (Model.application.DEBUG)
            {
                Debug.Log("Engine.Gameplay.ZoomOutCamera.Execute(" + objectName + ")");
            }
            Camera camera = GameObject.Find(objectName).GetComponent<Camera>();
            Model.coroutines["zoomOutCamera"] = Model.gameModel.StartCoroutine(
                Model.gameModel.zoomOutCamera(camera, from, to, 0.001f, callback)
            );
        }
    }
}