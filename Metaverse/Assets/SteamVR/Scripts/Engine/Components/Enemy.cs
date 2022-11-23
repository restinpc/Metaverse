using System;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics;

namespace Engine.Components
{
    /**
     * @property application Application object.
     * @property name Component name.
     * @property parent Virtual DOM parent node.
     * @property nodes Array with a child nodes.
     * @property gameObject Unity game object.
     * @property renderId Render method executions counter (debug tool).
     * @property mapStateToProps Function to map object properties from application state container.
     * @property label TextMeshProUGUI element.
     */
    public class Enemy : Component
    {
        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal PatrolPath path;
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Enemy(
            App application,
            GameObject gameObject,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, gameObject, scene, parent, name, mapStateToProps)
        {
            this.control = gameObject.GetComponent<AnimationController>();
        }

        public void move(PatrolPath path)
        {
            try
            {
                if (!this.path)
                {
                    this.path = path;
                }
                if (path != null && this.props["enabled"].getBool())
                {
                    if (mover == null)
                    {
                        mover = path.CreateMover(control.maxSpeed * 0.5f);
                    }
                    Model.application.setState(
                        Model.setEnemyMove(
                            this.name,
                            new Vector2(
                                Mathf.Clamp(mover.Position.x - this.gameObject.transform.position.x, -1, 1),
                                mover.Position.y
                            )
                        ),
                        false
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Components.Enemy(" + this.name + ").move() -> " + e.Message);
            }
        }

        public void death()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Component.Enemy(" + this.name + ").death()");
            }
            try
            {
                EnemyController enemy = gameObject.GetComponent<EnemyController>();
                enemy._collider.enabled = false;
                enemy.control.enabled = false;
                if (enemy._audio && enemy.ouch)
                    enemy._audio.PlayOneShot(enemy.ouch);
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Components.Enemy(" + this.name + ").death() -> " + e.Message);
            }
        }
    }
}