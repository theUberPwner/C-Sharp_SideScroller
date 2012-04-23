﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Testing
{
    class Player : MovingObject
    {
        private bool canJump = true;
        public int score = 0;

        //Constructor
        public Player(Vector2 position, Vector2 velocity, Texture2D sprite)
            : base(position, velocity, sprite, ObjectType.Player)
        {
            
        }

        public void Jump()
        {
            if (alive && canJump)
            {
                velocity.Y += -(float)Math.Cos(Math.PI/4) * 8;
                canJump = false;
            }
        }

        public override void ReachedBottom()
        {
            base.ReachedBottom();
            canJump = true;
        }

        public override void DetectEnemyHit(MovingObject mob)
        {
            //if collision with enemy
            if (Position.Y + Height - 8 <= mob.Position.Y + 16)
            {
                //kill enemy if coming from above
                if (velocity.Y > 0)
                {
                    mob.Die();
                    score += 1;
                    velocity.Y = Math.Min(-velocity.Y, -4);
                }
            }
            else
            {
                //if not hit enemy from above then you die
                Die();
            }
        }

        public void Move(Direction dir)
        {
            base.Move(dir, 1.0f);
        }

        public bool LevelComplete(Level currentLevel)
        {
            if (SpriteBounds.Intersects(currentLevel.FinishZone))
                return true;
            return false;
        }

        public override void Update(Level currentLevel, GameTime gameTime)
        {
            if (!alive)
                return;

            Vector2 lastPosition = Position;

            //moving X position
            Position = new Vector2(Position.X + velocity.X, Position.Y);
            var gobj = IntersectsWithAny(currentLevel.GameObjects);
            if (gobj != null && gobj.solid)
                Position = lastPosition;

            velocity.X *= 0.75f;

            lastPosition = Position;

            //moving Y position
            Position = new Vector2(Position.X, Position.Y  + velocity.Y);
            gobj = IntersectsWithAny(currentLevel.GameObjects);
            if (gobj != null)
            {
                if (gobj.Type == ObjectType.ItemBox && velocity.Y < 0)
                {
                    ItemBox ibox = (ItemBox)gobj;
                    if (Position.Y >= (ibox.Position.Y + ibox.Height - 5))
                        if ((Position.X + Width/2 > ibox.Position.X) && (Position.X + Width/2 < ibox.Position.X + ibox.Width))
                            if (!ibox.hit)
                                ibox.Hit();
                }
                else if (gobj.Type == ObjectType.Item)
                {
                    Item I = (Item)gobj;
                    switch (I.ItemIndex)
                    {
                        case 0: //super size
                                break;
                        
                        case 1: //invincible
                                break;
                    }
                    I.alive = false;
                }

                if (gobj.solid)
                {
                    Position = lastPosition;
                }
                if (!canJump)
                {
                    velocity.Y = 0;
                    if (Position.Y < gobj.Position.Y)
                        canJump = true;
                }
            }
            else 
            {
                canJump = false;
            }

            base.Update(currentLevel, gameTime);
        }
    }
}
