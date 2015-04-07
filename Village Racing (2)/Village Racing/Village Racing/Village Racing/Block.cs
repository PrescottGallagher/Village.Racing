using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Village_Racing
{
    class Block
    {
        int x;
        int y;
        Texture2D texture;
        public Vector2 Position;
        public Rectangle rect;
        public Rectangle[] Sides = new Rectangle[4];
        public string[] SideEffects = new string[4];
        public float positionOffsetY = 0;

        public Block(Texture2D text, Vector2 pos, string Top, string Bottom, string Left, string Right)
        {
            SideEffects[0] = Top;
            SideEffects[1] = Bottom;
            SideEffects[2] = Left;
            SideEffects[3] = Right;
            Position = pos;
            texture = text;
            rect = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            x = (int)pos.X;
            y = (int)pos.Y;
            Sides[0] = new Rectangle((x), (y) + 10, 18, 44); //Left
            Sides[1] = new Rectangle((x) + 10, (y), 44, 10); //Top
            Sides[2] = new Rectangle((x) + 46, (y) + 10, 18, 44); //Right
            Sides[3] = new Rectangle((x) + 20, (y) + 54, 24, 10); //Bottom
        }

        public Block(Texture2D text, Vector2 pos, string allSides)
        {
            SideEffects[0] = allSides;
            SideEffects[1] = allSides;
            SideEffects[2] = allSides;
            SideEffects[3] = allSides;
            Position = pos;
            texture = text;
            rect = new Rectangle((int)pos.X, (int)pos.Y, 64, 64);
            x = (int)pos.X;
            y = (int)pos.Y;
            //Sides[0] = new Rectangle((x), (y) + 21, 16, 22); //Left
            //Sides[1] = new Rectangle((x) + 18, (y), 28, 10); //Top
            //Sides[2] = new Rectangle((x) + 54, (y) + 27, 10, 10); //Right
            //Sides[3] = new Rectangle((x) + 18, (y) + 54, 28, 10); //Bottom
            Sides[0] = new Rectangle((x), (y) + 10, 18, 44); //Left
            Sides[1] = new Rectangle((x) + 24, (y), 15, 10); //Top
            Sides[2] = new Rectangle((x) + 46, (y) + 10, 18, 44); //Right
            Sides[3] = new Rectangle((x) + 24, (y) + 54, 15, 10); //Bottom
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, new Rectangle(rect.X, rect.Y - (int)positionOffsetY, 64, 64), Color.White);
            //foreach (Rectangle item in Sides)
            //{
            //    spritebatch.Draw(texture, item, Color.Black);
            //}
            if (positionOffsetY > 0)
            {
                positionOffsetY -= 1f;
            }
        }
    }
}
