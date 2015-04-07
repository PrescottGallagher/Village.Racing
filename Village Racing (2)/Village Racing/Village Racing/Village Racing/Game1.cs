using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Village_Racing
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        enum GameStates { Playing, TitleScreen };
        GameStates currentState = GameStates.TitleScreen;
        SpriteBatch spriteBatch;
        Player player;
        TileMap tiles;
        TitleScreen mainScreen;
        Camera camera;
        KeyboardState keyState;
        Texture2D gameBG;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        protected override void Initialize()
        {
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            this.IsMouseVisible = true;
            gameBG = Content.Load<Texture2D>("background (1)");
            graphics.ApplyChanges();
            camera = new Camera(GraphicsDevice.Viewport);

            mainScreen = new TitleScreen(Content.Load<Texture2D>("SmallCloud"), Content.Load<Texture2D>("LargeCloud"), Content.Load<Texture2D>("Hill1"), Content.Load<Texture2D>("Hill2"), Content.Load<Texture2D>("Background"));
            tiles = new TileMap(Content.Load<Texture2D>("Brick"), Content.Load<Texture2D>("BB1"), Content.Load<Texture2D>("net-katase"), Content.Load<Texture2D>("push"), Content.Load<Texture2D>("water"), camera);
            player = new Player(Content.Load<Texture2D>("head"), Content.Load<Texture2D>("body"), Content.Load<Texture2D>("foot"), Color.DarkViolet, new Vector2(200, 200), tiles);
            tiles.toLevel();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (currentState == GameStates.Playing)
            {
                tiles.Update();
                player.Update(gameTime);
                camera.LookAt(player.Position);
                camera.Update();

                if (Keyboard.GetState().IsKeyUp(Keys.N) && keyState.IsKeyDown(Keys.N))
                {
                    camera.Rotate90(true);
                }
                keyState = Keyboard.GetState();
            }
            else
            {
                mainScreen.Update(gameTime);
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    currentState = GameStates.Playing;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(new Vector2(0.0f)));
            spriteBatch.Draw(gameBG, new Rectangle(0, 0, 1024, 768), Color.White);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, camera.GetViewMatrix(new Vector2(1.0f)));
            if (currentState == GameStates.Playing)
            {
                tiles.Draw(spriteBatch);
                player.Draw(spriteBatch);
            }
            else
            {
                mainScreen.Draw(spriteBatch);
            }
            spriteBatch.End();
            this.Window.Title = (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString() + " FPS || " + "X: " + ((int)player.Position.X / 64).ToString() + ", Y: " + ((int)player.Position.Y / 64).ToString();
            base.Draw(gameTime); 
        }
    }
}
