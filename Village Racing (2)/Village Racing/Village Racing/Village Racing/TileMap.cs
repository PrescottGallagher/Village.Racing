using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;


namespace Village_Racing
{
    class TileMap
    {
        public Texture2D Brick;
        Texture2D Metal;
        Texture2D net;
        enum Levels {Ship, Outside};
        public int[,] levelOne = new int[512, 512];
        public Block[,] blocks = new Block[512, 512];
        public Vector2 StartingPoint;
        public Vector2 EndingPoint;
        MouseState lastState;
        Camera camera;


        public TileMap(Texture2D Brick, Texture2D Metal, Texture2D Net, Camera camera)
        {
            this.Brick = Brick;
            this.Metal = Metal;
            this.camera = camera;
            net = Net;
        }

        public void SetTile(int x, int y, int type)
        {
            levelOne[x, y] = type;
            if(type == 1)
                blocks[x, y] = new Block(Metal, new Vector2(x * 64, y * 64), "Solid", "Solid", "Solid", "Solid");
            if(type == 2)
                blocks[x, y] = new Block(Brick, new Vector2(x * 64, y * 64), "Solid", "Brick", "Solid", "Brick");
            if (type == 3)
                blocks[x, y] = new Block(net, new Vector2(x * 64, y * 64), "Net");
            //blocks[x, y] = new Rectangle(x * 64, y * 64, 64, 64);
        }

        public void toLevel()
        {
            using (StreamReader streamReader = new StreamReader("level.txt"))
            {
                for(int Y = 0; Y != 512; Y++)
                {
                    string line = streamReader.ReadLine();
                    string[] numbers = line.Split(',');

                    for (int X = 0; X != 512; X++)
                    {
                        int tile = int.Parse(numbers[X]);
                        SetTile(X, Y, tile);
                    }
                }
            }
        }

        public void Update()
        {
            StartingPoint = (camera.Position);
            EndingPoint = new Vector2(StartingPoint.X + 1124, StartingPoint.Y + 868);
            EndingPoint /= 64;
            StartingPoint /= 64;

            //if (Mouse.GetState().LeftButton == ButtonState.Pressed /* && lastState.LeftButton == ButtonState.Pressed*/)
            //{
            //    SetTile((int)((Mouse.GetState().X + camera.Position.X) / 64), (int)((Mouse.GetState().Y + camera.Position.Y) / 64), 2);
            //}
            //if (Mouse.GetState().RightButton == ButtonState.Pressed /*&& lastState.RightButton == ButtonState.Pressed*/)
            //{
            //    SetTile((int)((Mouse.GetState().X + camera.Position.X) / 64), (int)((Mouse.GetState().Y + camera.Position.Y) / 64), 0);
            //}
            lastState = Mouse.GetState();

            if (StartingPoint.X < 0)
            {
                StartingPoint.X = 0;
            }
            if (StartingPoint.Y < 0)
            {
                StartingPoint.Y = 0;
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = (int)StartingPoint.Y; y != (int)EndingPoint.Y; y++)
            {
                for (int x = (int)StartingPoint.X; x != (int)EndingPoint.X; x++)
                {
                    if (levelOne[x, y] > 0)
                    {
                        //spriteBatch.Draw(Metal, new Rectangle((x * 64) + 0, y * 64, 64, 64), Color.White);
                        blocks[x, y].Draw(spriteBatch);
                    }
                }
            }
        }

            
            
  
    }
}
