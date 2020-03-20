using System;
using System.Threading;
using JeuDeLaVie;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace D22
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private static Texture2D plusButtonTexture, minusButtonTexture;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D blackRectangle, rectTexture;
        private SpriteFont font;
        private Thread thread1;
        private bool sideMenu = true, mouseFollowUp = true;
        protected internal int staleWaitTime = 500, windowSizeX = 1800, windowSizeY = 960;
        private Color[] donneeTables;
        private FPSCounter FpsCounter;
        public Game1()
        {

            IsMouseVisible = true;
            Window.AllowUserResizing = false;

            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);
            IsFixedTimeStep = false;
            //vsync
            graphics.SynchronizeWithVerticalRetrace = false;

            //fpsCount
            FpsCounter = new FPSCounter();
        }

        protected override void Initialize()
        {
            InactiveSleepTime = new TimeSpan(0);

            if (sideMenu)
                graphics.PreferredBackBufferWidth = 50 + windowSizeX;
            else
                graphics.PreferredBackBufferWidth = windowSizeX;

            graphics.PreferredBackBufferHeight = windowSizeY;
            graphics.ApplyChanges();
            base.Initialize();
        }
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            if (sideMenu)
                rectTexture = new Texture2D(GraphicsDevice, windowSizeX + 50, windowSizeY);
            else
                rectTexture = new Texture2D(GraphicsDevice, windowSizeX, windowSizeY);

            plusButtonTexture = Content.Load<Texture2D>("Textures/plus");
            minusButtonTexture = Content.Load<Texture2D>("Textures/minus");

            blackRectangle = new Texture2D(GraphicsDevice, 1, 1);
            blackRectangle.SetData(new[] { Color.Black });

            font = Content.Load<SpriteFont>("daFont");

            JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);

        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        //key manage vars
        private bool wasTABDown;
        private bool wasMDown;
        protected void KeyManage()
        {
            if (wasTABDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.Tab))
                    wasTABDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Tab))
                {
                    wasTABDown = true;
                    sideMenu = !sideMenu;
                    if (sideMenu)
                    {
                        graphics.PreferredBackBufferWidth = 50 + windowSizeX;
                        rectTexture = new Texture2D(GraphicsDevice, windowSizeX + 50, windowSizeY);
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = windowSizeX;
                        rectTexture = new Texture2D(GraphicsDevice, windowSizeX, windowSizeY);
                    }
                    graphics.ApplyChanges();
                }
            }

            if (wasMDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.M))
                    wasMDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.M))
                {
                    wasMDown = true;
                    mouseFollowUp = !mouseFollowUp;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        protected override void Update(GameTime gameTime)
        {
            if (thread1 != null && thread1.IsAlive)
            {
                thread1.Join();
            }

            if (JeuDeLaVieTable.Stale)
            {
                Thread.Sleep(staleWaitTime);
                JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
            }

            thread1 = new Thread(JeuDeLaVieTable.CalculerCycle)
            {
                Priority = ThreadPriority.Highest
            };
            thread1.Start();

            KeyManage();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FpsCounter.Add((float)gameTime.ElapsedGameTime.TotalSeconds);
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
                        donneeTables[i] = Color.DarkSalmon;
                    }
                    else
                    {
                        if (JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesNewB()])
                        {
                            if (!JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
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
                            if (JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
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
                    if (JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesNewB()])
                    {
                        if (!JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
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
                        if (JeuDeLaVieTable.TableauDeLaVie[tableX, tableY, ArrayGPS.GetSwapTablesOldB()])
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

            spriteBatch.Draw(rectTexture, new Vector2(0, 0));

            if (sideMenu)
            {
                if (FpsCounter.FPSTotal >= FpsCounter.NbFrameCount) 
                { 
                    spriteBatch.DrawString(font, "FPS:" + Environment.NewLine + FpsCounter.AvgFPS, new Vector2(windowSizeX + 6, 30), Color.Black);
                    
                }
                spriteBatch.DrawString(font, "1FPS" + Environment.NewLine + FpsCounter.CurrentFPS, new Vector2(windowSizeX + 6, 70), Color.Black);
                if (mouseFollowUp) { 
                    spriteBatch.Draw(plusButtonTexture, new Vector2(windowSizeX + 4, 112), scale: new Vector2(1f));
                    spriteBatch.Draw(minusButtonTexture, new Vector2(windowSizeX + 27, 112), scale: new Vector2(1f));
                }
            }
            spriteBatch.End();
        }
    }
}
