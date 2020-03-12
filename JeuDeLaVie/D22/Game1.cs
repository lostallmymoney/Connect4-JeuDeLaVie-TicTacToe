using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace D22
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D blackRectangle;
        Texture2D whiteRectangle;
        Texture2D rectTexture;
        SpriteFont font;
        Thread thread1;
        int staleWaitTime = 500, windowSizeX = 800, windowSizeY = 450;
        Color[] donneeTables;
        public Game1()
        {
            //Content.RootDirectory = "Content";
            this.Window.AllowUserResizing = false;
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            this.graphics.PreferredBackBufferWidth = 2 * windowSizeX;
            this.graphics.PreferredBackBufferHeight = 2 * windowSizeY;
            this.graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            rectTexture = new Texture2D(GraphicsDevice, windowSizeX, windowSizeY);

            blackRectangle = new Texture2D(GraphicsDevice, 1, 1);
            blackRectangle.SetData(new[] { Color.Black });

            font = Content.Load<SpriteFont>("daFont");
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

            JeuDeLaVie.JeuDeLaVieTable.Instance.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            //    Exit();

            if (JeuDeLaVie.JeuDeLaVieTable.Instance.Stale)
            {
                Thread.Sleep(staleWaitTime);
                JeuDeLaVie.JeuDeLaVieTable.Instance.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
            }

            while (thread1 != null && thread1.IsAlive)
            {
                Thread.Sleep(2);
            }
            thread1 = new Thread(TheThread);
            thread1.Priority = ThreadPriority.Highest;
            thread1.Start();

            JeuDeLaVie.JeuDeLaVieTable.Instance.setBooleanTables();
            TheThread2();
            base.Update(gameTime);
        }

        protected void TheThread()
        {
            JeuDeLaVie.JeuDeLaVieTable.Instance.CalculerCycle();
        }
        protected void TheThread2()
        {
            donneeTables = new Color[windowSizeX * windowSizeY];
            for (int i = 0, x = 0, y = 0; i < donneeTables.Length; i++, x++)
            {
                if (x == windowSizeX)
                {
                    x = 0;
                    y++;
                }

                if (JeuDeLaVie.JeuDeLaVieTable.Instance.TablesDisplayB[x, y, 1])
                {
                    donneeTables[i] = Color.Black;
                    if (!JeuDeLaVie.JeuDeLaVieTable.Instance.TablesDisplayB[x, y, 0])
                    {
                        donneeTables[i] = Color.DarkGreen;
                    }
                }
                else
                {
                    if (JeuDeLaVie.JeuDeLaVieTable.Instance.TablesDisplayB[x, y, 0])
                    {
                        donneeTables[i] = Color.DarkRed;
                    }
                }
            }
            rectTexture.SetData(donneeTables);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(rectTexture, new Vector2(0, 0), color: Color.White, scale: new Vector2(2f));
            spriteBatch.End();
        }
    }
}
