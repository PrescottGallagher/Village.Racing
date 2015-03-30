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
        public enum Movements { Running, Idle, Jumping };
        Movements lastAction = Movements.Idle;
        Movements currentAction = Movements.Idle;
        SpriteEffects direction = SpriteEffects.None;
        int playerScale = 72;
        Block currentBlock;
        Texture2D Head;
        Texture2D Body;
        Rectangle playerRect;
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
        int currentColor = 0;
        Vector2 FootOffset;
        bool left = false;
        TileMap tiles;
        MouseState lastState;
        float elapsedJump;
        bool canJump;
        Vector2 lastSolidBlock;

        float speed;
        float accel;
        float jump;

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
        }

        public void Update(GameTime gameTime)
        {

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

            Velocity.Y += 0.30f;
            playerRect = new Rectangle((int)Position.X + 8, (int)Position.Y, 50, 141);
            for (int y = (int)tiles.StartingPoint.Y; y != (int)tiles.EndingPoint.Y; y++)
            {
                for (int x = (int)tiles.StartingPoint.X; x != (int)tiles.EndingPoint.X; x++)
                {
                    if (tiles.levelOne[x, y] > 0)
                    {
                        currentBlock = tiles.blocks[x, y];
                        if (currentBlock.Sides[1].Intersects(playerRect))
                        {
                            if (currentBlock.SideEffects[1] == "Solid")
                            {
                                Position.Y = (tiles.blocks[x, y].rect.Y - 140);
                                Velocity.Y = 0;
                                lastSolidBlock = new Vector2(x, y);
                                canJump = true;
                            }
                            if (currentBlock.SideEffects[1] == "Brick")
                            {
                                Position.Y = (tiles.blocks[x, y].rect.Y - 140);
                                Velocity.Y = 0;
                                canJump = true;
                            }
                            if (currentBlock.SideEffects[1] == "Net")
                            {
                                Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                Velocity = Vector2.Zero;
                            }
                        }
                        if (currentBlock.Sides[3].Intersects(playerRect))
                        {
                            if (currentBlock.SideEffects[3] == "Solid")
                            {
                                Position.Y = (tiles.blocks[x, y].rect.Y + 64);
                                Velocity.Y = 0.5f;
                                tiles.blocks[x, y].positionOffsetY = 10;
                                canJump = false;
                            }
                            if (currentBlock.SideEffects[3] == "Brick")
                            {
                                Position.Y = (tiles.blocks[x, y].rect.Y + 64);
                                Velocity.Y = 0.5f;
                                canJump = false;
                                tiles.SetTile(x, y, 0);
                            }
                            if (currentBlock.SideEffects[3] == "Net")
                            {
                                Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                Velocity = Vector2.Zero;
                            }
                        }
                        if (currentBlock.Sides[0].Intersects(playerRect))
                        {
                            if (currentBlock.SideEffects[0] == "Solid")
                            {
                                Position.X = (tiles.blocks[x, y].rect.X - 57);
                                Velocity.X = 0;
                            }
                            if (currentBlock.SideEffects[0] == "Net")
                            {
                                Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                Velocity = Vector2.Zero;
                            }
                        }
                        if (currentBlock.Sides[2].Intersects(playerRect))
                        {
                            if (currentBlock.SideEffects[2] == "Solid")
                            {
                                Position.X = (tiles.blocks[x, y].rect.X + 56);
                                Velocity.X = 0;
                            }
                            if (currentBlock.SideEffects[2] == "Net")
                            {
                                Position.Y = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.Y - 140;
                                Position.X = tiles.blocks[(int)(lastSolidBlock.X), (int)lastSolidBlock.Y].Position.X + 10;
                                Velocity = Vector2.Zero;
                            }
                        }
                    }
                    if (tiles.levelOne[x, y] == 51)
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
                Position += Velocity;
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(currentAction)
            {
                case(Movements.Idle):
                    spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 57), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 32), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation * 1.4f, new Vector2(64, 32), direction, 1.0f);
                    break;
                case (Movements.Running):
                    if (direction == SpriteEffects.None)
                    {
                        spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 57), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 128), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation, new Vector2(64, 128), direction, 1.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 57), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation, new Vector2(64, 64), direction, 1.0f);
                        spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 64), direction, 1.0f);
                    }
                    break;
                case (Movements.Jumping):
                    spriteBatch.Draw(Head, new Rectangle((int)(Position.X + HeadOffset.X), (int)(Position.Y + HeadOffset.Y), playerScale, playerScale), null, charColor, headRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Body, new Rectangle((int)(Position.X + BodyOffset.X), (int)(Position.Y + BodyOffset.Y + 57), playerScale, playerScale), null, charColor, bodyRotation, new Vector2(64, 64), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X + 16), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, footRotation, new Vector2(64, 32), direction, 1.0f);
                    spriteBatch.Draw(Foot, new Rectangle((int)(Position.X + FootOffset.X), (int)(Position.Y + 102 + FootOffset.Y), playerScale, playerScale), null, charColor, -footRotation * 1.4f, new Vector2(64, 96), direction, 1.0f);
                    break;
            }
        }
    }
}
