using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using JeuDeLaVie;
using System.Diagnostics;
using System.Windows.Forms;
using System;

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
        Texture2D rectTexture;
        SpriteFont font;
        Thread thread1, thread2;
        int staleWaitTime = 500, windowSizeX = 1800, windowSizeY = 960;
        Color[] donneeTables;
        public Game1()
        {
            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = false;

            //Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            //vsync
            graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            InactiveSleepTime = new System.TimeSpan(0);
            
            this.graphics.PreferredBackBufferWidth = windowSizeX;
            this.graphics.PreferredBackBufferHeight = windowSizeY;
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

            JeuDeLaVie.JeuDeLaVieTable.Instance.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);

        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (thread1 != null && thread1.IsAlive)
            {
                thread1.Join();
            }

            if (JeuDeLaVie.JeuDeLaVieTable.Instance.Stale)
            {
                Thread.Sleep(staleWaitTime);
                JeuDeLaVie.JeuDeLaVieTable.Instance.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
            }

            thread1 = new Thread(JeuDeLaVie.JeuDeLaVieTable.Instance.CalculerCycle)
            {
                Priority = ThreadPriority.Highest
            };
            thread1.Start();

            TheThread2();
            base.Update(gameTime);
        }

        private void TheThread2()
        {
            donneeTables = new Color[windowSizeX * windowSizeY];
            for (int i = 0, x = 0, y = 0; i < donneeTables.Length; i++, x++)
            {
                if (x == windowSizeX)
                {
                    x = 0;
                    y++;
                }

                if (JeuDeLaVie.JeuDeLaVieTable.Instance.TableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesNewB()])
                {
                    donneeTables[i] = Color.Black;
                    if (!JeuDeLaVie.JeuDeLaVieTable.Instance.TableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesOldB()])
                    {
                        donneeTables[i] = Color.DarkGreen;
                    }
                }
                else
                {
                    if (JeuDeLaVie.JeuDeLaVieTable.Instance.TableauDeLaVie[x, y, ArrayGPS.Instance.GetSwapTablesOldB()])
                    {
                        donneeTables[i] = Color.DarkRed;
                    }
                }
            }
            rectTexture.SetData(donneeTables);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(rectTexture, new Vector2(0, 0), color: Color.White, scale: new Vector2(1f));
            spriteBatch.End();
        }
    }
}
