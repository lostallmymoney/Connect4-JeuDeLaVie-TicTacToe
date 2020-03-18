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
        Thread thread1;
        bool sideMenu = false;
        int staleWaitTime = 500, windowSizeX = 1800, windowSizeY = 960;
        Color[] donneeTables;
        FPSCounter FpsCounter;
        public Game1()
        {

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = false;

            //Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            //vsync
            graphics.SynchronizeWithVerticalRetrace = false;

            //fpsCount
            FpsCounter = new FPSCounter();
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

            JeuDeLaVie.JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);

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

            if (JeuDeLaVie.JeuDeLaVieTable.Stale)
            {
                Thread.Sleep(staleWaitTime);
                JeuDeLaVie.JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
            }

            thread1 = new Thread(JeuDeLaVie.JeuDeLaVieTable.CalculerCycle)
            {
                Priority = ThreadPriority.Highest
            };
            thread1.Start();

            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                sideMenu = sideMenu ? false : true;
                if (sideMenu)
                {
                    this.graphics.PreferredBackBufferWidth = 50 + windowSizeX;
                    rectTexture = new Texture2D(GraphicsDevice, windowSizeX + 50, windowSizeY);
                    this.graphics.ApplyChanges();
                }
                else
                {
                    this.graphics.PreferredBackBufferWidth = windowSizeX;
                    rectTexture = new Texture2D(GraphicsDevice, windowSizeX, windowSizeY);
                    this.graphics.ApplyChanges();
                }
            }

            DrawThread();
            FpsCounter.add((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        private void DrawThread()
        {
            if (sideMenu)
            {
                donneeTables = new Color[(50+windowSizeX) * windowSizeY];
            }
            else
            {
                donneeTables = new Color[windowSizeX * windowSizeY];
            }

            for (int i = 0, x = 0, y=0, tableX = 0, tableY = 0; i < donneeTables.Length; i++, x++)
            {
                if (sideMenu)
                {
                    if (x > windowSizeX)
                    {
                        
                    }
                    else
                    {
                        if (JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesNewB()])
                        {
                            if (!JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
                            {
                                donneeTables[i] = Color.DarkGreen;
                            }
                            else
                            {
                                donneeTables[i] = Color.Black;
                            }
                        }
                        else
                        {
                            if (JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
                            {
                                donneeTables[i] = Color.DarkRed;
                            }
                        }
                        tableX++;
                        if (tableX >= windowSizeX)
                        {
                            tableX = 0;
                        }
                    }
                    if (x >= windowSizeX + 50)
                    {
                        x = 0;
                        tableY++;
                        y++;
                    }
                }
                else
                {
                    if (JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesNewB()])
                    {
                        if (!JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
                        {
                            donneeTables[i] = Color.DarkGreen;
                        }
                        else
                        {
                            donneeTables[i] = Color.Black;
                        }
                    }
                    else
                    {
                        if (JeuDeLaVie.JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
                        {
                            donneeTables[i] = Color.DarkRed;
                        }
                    }
                    tableX++;
                    if (tableX == windowSizeX)
                    {
                        tableX = 0;
                        tableY++;
                    }
                }
            }
            rectTexture.SetData(donneeTables);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (sideMenu)
            {
                if (FpsCounter.FPSTotal >= FpsCounter.NbFrameCount)
                    spriteBatch.DrawString(font, "FPS" + Environment.NewLine + FpsCounter.avgFPS, new Vector2(windowSizeX + 6, 30), Color.Black);
                spriteBatch.DrawString(font, "1FPS" + Environment.NewLine + FpsCounter.CurrentFPS, new Vector2(windowSizeX + 6, 70), Color.Black);
            }
                spriteBatch.Draw(rectTexture, new Vector2(0, 0), color: Color.White, scale: new Vector2(1f));
            spriteBatch.End();
        }
    }
}
