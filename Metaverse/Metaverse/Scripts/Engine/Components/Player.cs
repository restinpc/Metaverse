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
            GameObject gameObject,
            Component parent = null,
            string name = "",
            Func<
                Dictionary<string, Prop>,
                Dictionary<string, Prop>
            > mapStateToProps = null
        ) : base(application, gameObject, parent, name, mapStateToProps)
        {
            this.isDead = false;
            this.isVictory = false;
            this.isInputEnabled = true;
            this.player = gameObject.GetComponent<PlayerController>();
        }

        /**
         * @render
         */ 
        public override void render(GameObject stdout)
        {
            base.render(stdout);
            renderDeadProp();
            renderVictoryProp();
            renderInputEnableProp();
            renderCollider2dProp();
            renderSpawnProp();
        }

        private void renderDeadProp()
        {

            bool deadProp = this.props["dead"].getBool();
            if (deadProp && !this.isDead)
            {
                this.isDead = true;
                // Model.gameModel.virtualCamera.m_Follow = null;
                // Model.gameModel.virtualCamera.m_LookAt = null;
                player.animator.SetTrigger("hurt");
                player.animator.SetBool("dead", true);
                Dictionary<string, Prop> newState = Model.toggleInput(false);
                foreach (KeyValuePair<string, Prop> el in Model.toggleCollider2d(false))
                {
                    newState.Add(el.Key, el.Value);
                }
                Model.application.setState(newState);
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
                Model.application.setState(Model.toggleInput(false));
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

        private void renderCollider2dProp()
        {
            bool toggleCollider2dProp = this.props["collider2d"].getBool();
            if (!this.isToggleCollider2d && toggleCollider2dProp)
            {
                this.isToggleCollider2d = true;
                player.collider2d.enabled = true;
            }
            else if (this.isToggleCollider2d && !toggleCollider2dProp)
            {
                this.isToggleCollider2d = false;
                player.collider2d.enabled = false;
            }
        }

        private void renderSpawnProp()
        {
            bool spawnProp = this.props["spawn"].getBool();
            if (spawnProp)
            {
                Model.application.setState(Model.toggleSpawn(false));
                if (player.audioSource && player.respawnAudio)
                    player.audioSource.PlayOneShot(player.respawnAudio);
                player.Teleport(Model.gameModel.spawnPoint.transform.position);
                player.jumpState = PlayerController.JumpState.Grounded;
                player.animator.SetBool("dead", false);
                // Model.gameModel.virtualCamera.m_Follow = player.transform;
                // Model.gameModel.virtualCamera.m_LookAt = player.transform;
                Simulation.Schedule<Gameplay.EnablePlayerInput>(2f);
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
                Schedule<Gameplay.EnemyDeath>().enemy = enemy;
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
                Dictionary<string, Prop> newState = Model.killPlayer();
                foreach (KeyValuePair<string, Prop> el in Model.pauseGame())
                {
                    newState.Add(el.Key, el.Value);
                }
                if (newState["lives"].getInt() == 0)
                {
                    foreach (KeyValuePair<string, Prop> el in Model.gameOver())
                    {
                        newState.Add(el.Key, el.Value);
                    }
                }
                Model.application.setState(newState);
            }
        }

        public void spawn()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").spawn()");
            }
            Model.application.setState(Model.toggleInput(false));
            player.collider2d.enabled = true;
            if (player.audioSource && player.respawnAudio)
                player.audioSource.PlayOneShot(player.respawnAudio);
            player.Teleport(Model.gameModel.spawnPoint.transform.position);
            player.jumpState = PlayerController.JumpState.Grounded;
            player.animator.SetBool("dead", false);
            // Model.gameModel.virtualCamera.m_Follow = player.transform;
            // Model.gameModel.virtualCamera.m_LookAt = player.transform;
            Simulation.Schedule<Gameplay.EnablePlayerInput>(2f);
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

        public void victory()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").victory()");
            }
            Model.application.setState(Model.victory());
        }

        public void revive()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").revive()");
            }
            Model.application.setState(Model.revivePlayer());
        }

        public void enableInput()
        {
            if (DEBUG && this.name.Length > 0)
            {
                Debug.Log("Engine.Components.Player(" + this.name + ").enableInput()");
            }
            Model.application.setState(Model.toggleInput(true));
        }
    }
}