using System;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Mechanics;
using Platformer.Core;
using static Platformer.Core.Simulation;

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
        public bool isDead;
        public bool isVictory;
        public bool isInputEnabled;
        public bool isToggleCollider2d;
        public PlayerController player;
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
            this.isInputEnabled = true;
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
                        this.player = gameObject.GetComponent<PlayerController>();
                    }
                    renderDeadProp();
                    renderVictoryProp();
                    renderInputEnableProp();
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
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
                Model.application.setState(Model.toggleInputAction(false));
                Simulation.Schedule<Gameplay.PlayerSpawn>(2f);
            }
            else if (this.isDead && !deadProp)
            {
                this.isDead = false;
            }
        }

        private void renderVictoryProp()
        {
            bool victoryProp = this.props["victory"].getBool();
            if (!this.isVictory && victoryProp)
            {
                this.isVictory = true;
                player.animator.SetTrigger("victory");
                Model.application.setState(Model.toggleInputAction(false));
            }
        }

        private void renderInputEnableProp()
        {
            bool inputEnabledProp = this.props["inputEnabled"].getBool();
            if (!this.isInputEnabled && inputEnabledProp)
            {
                this.isInputEnabled = true;
                player.controlEnabled = true;
            }
            else if (this.isInputEnabled && !inputEnabledProp)
            {
                this.isInputEnabled = false;
                player.controlEnabled = false; if (!this.isInputEnabled && inputEnabledProp)
                {
                    this.isInputEnabled = true;
                    player.controlEnabled = true;
                }
                else if (this.isInputEnabled && !inputEnabledProp)
                {
                    this.isInputEnabled = false;
                    player.controlEnabled = false;
                }
            }
        }

        private void renderSpawnProp()
        {
            bool spawnProp = this.props["spawn"].getBool();
            if (spawnProp)
            {
                Model.application.setState(Model.toggleSpawnAction(false));
                if (player.audioSource && player.respawnAudio)
                    player.audioSource.PlayOneShot(player.respawnAudio);
                player.jumpState = PlayerController.JumpState.Grounded;
                player.animator.SetBool("dead", false);
            }
        }

        /**
         * @methods 
         */ 
        public void enemyCollision(EnemyController enemy)
        {
            var willHurtEnemy = player.Bounds.center.y >= enemy.Bounds.max.y;
            if (willHurtEnemy)
            {
                // Schedule<Gameplay.EnemyDeath>().enemy = enemy;
                player.Bounce(2);
            }
            else
            {
                this.death();
            }
        }

        public void death()
        {
            if (DEBUG && this.name.Length > 0)
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

        public void spawn()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").spawn()");
            }
            Model.application.setState(Model.toggleInputAction(false));
            player.collider2d.enabled = true;
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);
        }

        public void jump()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").jump()");
            }
            if (player.audioSource && player.jumpAudio)
            {
                player.audioSource.PlayOneShot(player.jumpAudio);
            }
        }

        public void revive()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").revive()");
            }
            Model.application.setState(Model.revivePlayerAction());
        }

        public void enableInput()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").enableInput()");
            }
            Model.application.setState(Model.toggleInputAction(true));
        }
    }
}