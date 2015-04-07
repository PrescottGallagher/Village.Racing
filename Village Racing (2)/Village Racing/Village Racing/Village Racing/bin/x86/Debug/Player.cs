using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Village_Racing
{
    class Player
    {
        bool changedAction = true;
        public bool colliding = false;
        float hd;
        int topOffset = 118;
        int bottomOffset;
        int leftOffset;
        int rightOffset;
        float vd;
        Vector2 lastPosition;
        public enum Movements { Running, Idle, Jumping };
        Movements lastAction = Movements.Idle;
        Movements currentAction = Movements.Idle;
        SpriteEffects direction = SpriteEffects.None;
        int playerScale = 64;
        Block currentBlock;
        Texture2D Head;
        Texture2D Body;
        Rectangle playerRect;
        Rectangle[] playerRects = new Rectangle[3];
        Texture2D Foot;
        public Vector2 Position;
        Vector2 Velocity;
        Vector2 BodyOffset;
        Vector2 HeadOffset;
        float headRotation;
        float bodyRotation;
        bool moving;
        float footRotation;
        Color charColor;
        Color[] colors = { Color.Blue, Color.White, Color.Pink, Color.Yellow, Color.Orange };
        Vector2 FootOffset;
        bool left = false;
        TileMap tiles;
        bool canJump;
        Vector2 lastSolidBlock;
        float gravity = 0.3f;
        float speed;
        float accel;
        float jump;
        float elapsedJump;

        public Player(Texture2D head, Texture2D body, Texture2D foot, Color mycolor, Vector2 position, TileMap tiles)
        {
            Head = head;
            Body = body;
            Foot = foot;
            charColor = mycolor;
            Position = position;
            BodyOffset = Vector2.Zero;
            HeadOffset = Vector2.Zero;
            FootOffset = Vector2.Zero;
            headRotation = -0.2f;
            bodyRotation = 0;
            footRotation = 0;
            this.tiles = tiles;
            speed = 60;
            accel = 50;
            jump = 50;
            //playerRects[0] = new Rectangle((int)position.X, (int)position.Y, playerScale, playerScale);
            //playerRects[1] = new Rectangle((int)position.X, (int)position.Y + 51, playerScale, playerScale);
            //playerRects[2] = new Rectangle((int)position.X, (int)position.Y + 90, playerScale, playerScale);
        }

        public void Update(GameTime gameTime)
        {
            playerRects[0] = new Rectangle((int)Position.X + 8, (int)Position.Y, playerScale - 14, playerScale);
            playerRects[1] = new Rectangle((int)Position.X + 8, (int)Position.Y + 51, playerScale - 14, playerScale);
            playerRects[2] = new Rectangle((int)Position.X + 8, (int)Position.Y + 90, playerScale - 14, playerScale);
            #region Animation
            if (lastAction != currentAction)
            {
                changedAction = true;
            }

            switch (currentAction)
            {
                case (Movements.Idle):
                    if (changedAction)
                    {
                        BodyOffset = Vector2.Zero;
                        HeadOffset = Vector2.Zero;
                        FootOffset = Vector2.Zero;
                        headRotation = -0.2f;
                        bodyRotation = 0;
                        footRotation = 0;
                    }
                    if (HeadOffset.X > 3)
                    {
                        left = true;
                    }
                    if (HeadOffset.X < 1)
                    {
                        left = false;
                    }
                    if (left)
                    {
                        HeadOffset.X -= 0.1f;
                        BodyOffset.X -= 0.04f;
                        bodyRotation += 0.002f;
                        footRotation -= 0.005f;
                        FootOffset.Y += 0.2f;
                        FootOffset.X -= 0.2f;
                    }
                    else
                    {
                        HeadOffset.X += 0.05f;
                        BodyOffset.X += 0.02f;
                        bodyRotation -= 0.001f;
                        footRotation += 0.0025f;
                        FootOffset.Y -= 0.1f;
                        FootOffset.X += 0.1f;
                    }
                    changedAction = false;
                    break;

                case (Movements.Running):
                    if (changedAction)
                    {
                        footRotation = 0;
                        FootOffset.X = 30;
                    }
                    if (footRotation > 1f)
                    {
                        left = true;
                    }
                    if (footRotation < -1f)
                    {
                        left = false;
                    }
                    if (left)
                    {
                        footRotation -= 0.2f;
                        HeadOffset.Y += 0.5f;
                    }
                    else
                    {
                        footRotation += 0.2f;
                        HeadOffset.Y -= 0.5f;
                    }
                    changedAction = false;
                    break;
                case (Movements.Jumping):
                    if (changedAction)
                    {
                        BodyOffset = Vector2.Zero;
                        HeadOffset = Vector2.Zero;
                        FootOffset = Vector2.Zero;
                        headRotation = -0.2f;
                        bodyRotation = 0;
                        footRotation = -0.001f;
                        left = false;
                    }
                    if (footRotation < -1)
                    {
                        left = true;
                    }
                    if (!left)
                    {
                        footRotation -= -footRotation;
                    }
                    changedAction = false;
                    break;
            }
            #endregion

            #region Base Gravity and Collision

            Velocity.Y += gravity;
            playerRect = new Rectangle((int)Position.X + 8, (int)Position.Y, 40, 120);
            for (int y = (int)tiles.StartingPoint.Y; y != (int)tiles.EndingPoint.Y; y++)
            {
                for (int x = (int)tiles.StartingPoint.X; x != (int)tiles.EndingPoint.X; x++)
                {
                    try
                    {
                        if (tiles.levelOne[x, y] > 0)
                        {
                            currentBlock = tiles.blocks[x, y];
                            if (currentBlock != null)
                            {
                                if (currentBlock.Sides[1].Intersects(playerRect))
                                {
                                    if (currentBlock.SideEffects[1] == "Solid")
                                    {
                                        Position.Y = (tiles.blocks[x, y].rect.Y - topOffset);
                                        Velocity.Y = 0;
                                        gravity = 0.3f;
                                        lastSolidBlock = new Vector2(x, y);
                                        canJump = true;
                                    }
                                    else if (currentBlock.SideEffects[1] == "Brick")
                                    {
                                        Position.Y = (tiles.blocks[x, y].rect.Y - topOffset);
                                        Velocity.Y = 0;
                                        canJump = true;
                                    }
                                    else if (currentBlock.SideEffects[1] == "Net")
                                    {
                                        Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                        Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                        Velocity = Vector2.Zero;
                                    }
                                    else if (currentBlock.SideEffects[1] == "Push")
                                    {
                                        if (tiles.levelOne[x, y + 1] == 0)
                                        {
                                            Position.Y = (tiles.blocks[x, y].rect.Y - topOffset);
                                            Velocity.Y = 0;
                                            canJump = true;

                                            tiles.SetTile(x, y + 1, 4);
                                            tiles.SetTile(x, y, 0);
                                        }
                                        else
                                        {
                                            Position.Y = (tiles.blocks[x, y].rect.Y - topOffset);
                                            Velocity.Y = 0;
                                            canJump = true;
                                        }
                                    }
                                    else if (currentBlock.SideEffects[1] == "Water")
                                    {
                                        gravity = 0.1f;
                                    }
                                }
                                if (currentBlock.Sides[3].Intersects(playerRect))
                                {
                                    if (currentBlock.SideEffects[3] == "Solid")
                                    {
                                        gravity = 0.3f;
                                        //Position.Y = (tiles.blocks[x, y].rect.Y + 64);
                                        Velocity.Y = 1f;
                                        tiles.blocks[x, y].positionOffsetY = 10;
                                        canJump = false;
                                    }
                                    else if (currentBlock.SideEffects[3] == "Brick")
                                    {
                                        //Position.Y = (tiles.blocks[x, y].rect.Y + 50);
                                        Velocity.Y = 1f;
                                        gravity = 0.3f;
                                        canJump = false;
                                        tiles.SetTile(x, y, 0);
                                    }
                                    else if (currentBlock.SideEffects[3] == "Net")
                                    {
                                        Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                        Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                        Velocity = Vector2.Zero;
                                    }
                                    else if (currentBlock.SideEffects[3] == "Push")
                                    {
                                        if (tiles.levelOne[x, y - 1] == 0)
                                        {
                                            //Position.Y = (tiles.blocks[x, y].rect.Y + 50);
                                            Velocity.Y = 0.5f;
                                            canJump = false;
                                            tiles.SetTile(x, y - 1, 4);
                                            tiles.SetTile(x, y, 0);
                                        }
                                        else
                                        {
                                            //Position.Y = (tiles.blocks[x, y].rect.Y + 50);
                                            Velocity.Y = 0.5f;
                                            canJump = false;
                                            tiles.blocks[x, y].positionOffsetY = 10;
                                        }
                                    }
                                    else if (currentBlock.SideEffects[3] == "Water")
                                    {
                                        gravity = 0.1f;
                                    }
                                }
                                if (currentBlock.Sides[0].Intersects(playerRect))
                                {
                                    if (currentBlock.SideEffects[0] == "Solid")
                                    {
                                        Position.X = (tiles.blocks[x, y].rect.X - 58);
                                        //Position.X = lastPosition.X;
                                        Velocity.X = 0;
                                    }
                                    else if (currentBlock.SideEffects[0] == "Net")
                                    {
                                        Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                        Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                        Velocity = Vector2.Zero;
                                    }
                                    else if (currentBlock.SideEffects[0] == "Push")
                                    {
                                        if (tiles.levelOne[x + 1, y] == 0)
                                        {
                                            Position.X = (tiles.blocks[x, y].rect.X - 58);
                                            Velocity.X = 0;
                                            tiles.SetTile(x + 1, y, 4);
                                            tiles.SetTile(x, y, 0);
                                        }
                                        else
                                        {
                                            Position.X = (tiles.blocks[x, y].rect.X - 58);
                                            Velocity.X = 0;
                                        }
                                    }
                                    else if (currentBlock.SideEffects[0] == "Water")
                                    {
                                        gravity = 0.1f;
                                    }
                                }
                                if (currentBlock.Sides[2].Intersects(playerRect))
                                {
                                    if (currentBlock.SideEffects[2] == "Solid")
                                    {
                                        Position.X = (tiles.blocks[x, y].rect.X + 64);
                                        //Position.X = lastPosition.X;
                                        Velocity.X = 0;
                                    }
                                    else if (currentBlock.SideEffects[2] == "Net")
                                    {
                                        Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                        Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                        Velocity = Vector2.Zero;
                                    }
                                    else if (currentBlock.SideEffects[2] == "Push")
                                    {
                                        if (tiles.levelOne[x - 1, y] == 0)
                                        {
                                            Position.X = (tiles.blocks[x, y].rect.X + 64);
                                            Velocity.X = 0;
                                            tiles.SetTile(x - 1, y, 4);
                                            tiles.SetTile(x, y, 0);
                                        }
                                        else
                                        {
                                            Position.X = (tiles.blocks[x, y].rect.X + 64);
                                            Velocity.X = 0;
                                        }
                                    }
                                    else if (currentBlock.SideEffects[2] == "Water")
                                    {
                                        gravity = 0.1f;
                                    }
                                }
                            }
                            //colliding = false;
                            //currentBlock = tiles.blocks[x, y];
                            //hd = Math.Abs((playerRects[1].Center.X * playerRects[1].Center.X) + (currentBlock.rect.Center.X * currentBlock.rect.Center.X));
                            //vd = Math.Abs((playerRects[1].Center.Y * playerRects[1].Center.Y) + (currentBlock.rect.Center.Y * currentBlock.rect.Center.Y));
                            //if(currentBlock != null)
                            //{
                            //    if (currentBlock.rect.Intersects(playerRect))
                            //    {
                            //        colliding = true;

                            //        //if (hd < vd)
                            //        //{
                            //        //    if (playerRect.Center.X < currentBlock.rect.Center.X)
                            //        //    {
                            //        //        if (currentBlock.SideEffects[0] == "Solid")
                            //        //        {
                            //        //            Position.X = (currentBlock.rect.X - 58);
                            //        //            //Position.X = lastPosition.X;
                            //        //            Velocity.X = 0;
                            //        //        }
                            //        //    }
                            //        //    else
                            //        //    {
                            //        //        if (currentBlock.SideEffects[2] == "Solid")
                            //        //        {
                            //        //            Position.X = (tiles.blocks[x, y].rect.X + 64);
                            //        //            //Position.X = lastPosition.X;
                            //        //            Velocity.X = 0;
                            //        //        }
                            //        //    }
                            //        //}
                                    
                            //        //if (vd < hd)
                            //        //{
                            //        //    if (playerRect.Center.Y < currentBlock.rect.Center.Y)
                            //        //    {
                            //        //        if (currentBlock.SideEffects[1] == "Solid")
                            //        //        {
                            //        //            Position.Y = currentBlock.rect.Y - (playerRect.Height) + 1;
                            //        //            Velocity.Y = 0;
                            //        //            gravity = 0.3f;
                            //        //            lastSolidBlock = new Vector2(x, y);
                            //        //            canJump = true;
                            //        //        }
                            //        //    }
                            //        //    else
                            //        //    {
                            //        //        if (currentBlock.SideEffects[3] == "Solid")
                            //        //        {
                            //        //            Position.Y = currentBlock.rect.Bottom;
                            //        //            Velocity.Y = 1;
                            //        //            canJump = false;
                            //        //        }
                            //        //    }
                            //        //}

                            //    }
                            //}
                        }
                    }
                    catch
                    {
                        Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                        Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                    }
                }
            }



            #endregion

            #region Input
            moving = false;
            lastAction = currentAction;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                direction = SpriteEffects.FlipHorizontally;
                currentAction = Movements.Running;
                moving = true;
                Velocity.X -= 0.5f;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                direction = SpriteEffects.None;
                currentAction = Movements.Running;
                moving = true;
                Velocity.X += 0.5f;
            }
            else
            {
                currentAction = Movements.Idle;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (canJump && elapsedJump < 1f)
                {
                    currentAction = Movements.Jumping;
                    Velocity.Y -= jump / 4;
                    canJump = false;
                }

            }

            if (!moving)
            {
                if (Velocity.X < 0)
                {
                    if (Velocity.X > -2)
                    {
                        Velocity.X += 0.25f;
                    }
                    else
                    {
                        Velocity.X += 0.5f;
                    }
                }
                if (Velocity.X > 0)
                {
                    if (Velocity.X < 2)
                    {
                        Velocity.X -= 0.25f;
                    }
                    else
                    {
                        Velocity.X -= 0.5f;
                    }
                }
                if (Velocity.X < 0.25f && Velocity.X > 0f || Velocity.X > -0.25f && Velocity.X < 0f)
                {
                    Velocity.X = 0;
                }
            }
            else
            {
                if (Velocity.X > (speed / 3))
                {
                    Velocity.X = speed / 3;
                }
                else if (Velocity.X < -(speed / 3))
                {
                    Velocity.X = -(speed / 3);
                }
            }
            if (Velocity.Y != 0)
            {
                currentAction = Movements.Jumping;
            }

                if (Velocity.Y > 0)
                {
                    canJump = false;
                }
                lastPosition = Position;    
            Position += Velocity;
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(currentAction)
            {
                case(Movements.Idle):
                    spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 51), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 32), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation * 1.4f, new Vector2(64, 32), direction, 1.0f);
                    break;
                case (Movements.Running):
                    if (direction == SpriteEffects.None)
                    {
                        spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 51), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 128), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation, new Vector2(64, 128), direction, 1.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 51), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 64), direction, 1.0f);
                    }
                    break;
                case (Movements.Jumping):
                    spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 51), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 32), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 90 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation * 1.4f, new Vector2(64, 96), direction, 1.0f);
                    break;
            }
        }
    }
}
