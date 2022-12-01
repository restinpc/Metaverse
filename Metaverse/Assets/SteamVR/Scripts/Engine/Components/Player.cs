using System;
using System.Collections.Generic;
using UnityEngine;

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
     */
    public class Player : Component
    {
        public new Dictionary<string, Prop> props = new Dictionary<string, Prop>() {
            { "visible", null },
            { "dead", null },
            { "spawn", null }
        };
        public bool isDead;
        public bool isVictory;
        public Valve.VR.InteractionSystem.Player player;
        /**
        * @constructor
        * @param application Application object.
        * @param gameObject GameObject.
        * @param parent Virtual DOM parent node.
        * @param name Label name.
        * @param mapStateToProps Function to map object properties from application state container.
        */
        public Player(
            App application,
            Scene scene,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, scene, parent, name, mapStateToProps)
        {
            this.isDead = false;
            this.isVictory = false;
        }

        /**
         * @render
         */ 
        public override void render(GameObject stdout)
        {
            base.render(stdout);
            try
            {
                if (gameObject.activeSelf)
                {
                    if (this.player == null)
                    {
                        this.player = gameObject.GetComponent<Valve.VR.InteractionSystem.Player>();
                    }
                    renderDeadProp();
                    renderSpawnProp();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Engine.Player(" + this.name + ").render(" + this.renderId + ") -> " + e.Message);
            }
        }

        private void renderDeadProp()
        {
            bool deadProp = this.props["dead"].getBool();
            if (deadProp && !this.isDead)
            {
                this.isDead = true;
                // player.animator.SetTrigger("hurt");
                // player.animator.SetBool("dead", true);
                Model.application.setState(Model.toggleInputAction(false));
            }
            else if (this.isDead && !deadProp)
            {
                this.isDead = false;
            }
        }

        private void renderSpawnProp()
        {
            bool spawnProp = this.props["spawn"].getBool();
            if (spawnProp)
            {
                Model.application.setState(Model.toggleSpawnAction(false));
            }
        }

        /**
         * @methods 
         */

        public void death()
        {
            if (this.application.DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").death()");
            }
            if (!this.isDead)
            {
                Dictionary<string, Prop> baseState = Model.killPlayerAction();
                List<Dictionary<string, Prop>> actions = new List<Dictionary<string, Prop>>()
                {
                    baseState,
                    Model.pauseGameAction()
                };
                if (baseState["lives"].getInt() == 0)
                {
                    actions.Add(Model.gameOverAction());
                }
                Model.application.setState(Model.mergeActions(actions));
            }
        }

        public void revive()
        {
            if (this.application.DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").revive()");
            }
            Model.application.setState(Model.revivePlayerAction());
        }
    }
}