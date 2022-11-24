using System.Collections;
using System.Collections.Generic;
using Platformer.Core;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// AnimationController integrates physics and animation. It is generally used for simple enemy animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class AnimationController : Kinematic
    {
        /// <summary>
        /// Max horizontal speed.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Max jump velocity
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Set to true to initiate a jump.
        /// </summary>
        public bool jump;

        /// <summary>
        /// Set to true to set the current jump velocity to zero.
        /// </summary>
        public bool stopJump;

        public new Engine.App application;
        public Engine.Components.Component component;
        Engine.Game gameModel = Simulation.GetModel<Engine.Game>();

        SpriteRenderer spriteRenderer;
        Animator animator;

        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            application = Engine.Model.application;
            component = Engine.Model.application.getComponentByName(this.name);
        }

        protected override void ComputeVelocity()
        {
            Dictionary<string, Engine.Prop> enemies = this.application.state["enemies"].getDictionary();
            Vector3 move = enemies[this.name].getDictionary()["move"].getVector3();
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * gameModel.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * gameModel.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            targetVelocity = move * maxSpeed;
        }

        public override void Update()
        {
            if (component != null)
            {
                if (!component.props["enabled"].getBool())
                {
                    this.animator.speed = 0;
                } else
                {
                    this.animator.speed = 1;
                }
            }
            base.Update();
        }
    }
}