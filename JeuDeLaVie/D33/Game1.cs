using System;
using System.Diagnostics;
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
        private Texture2D blackRectangle, tableTexture, menuTexture, arrowTexture, structureTexture;
        private SpriteFont font;
        private Thread thread1;
        private bool sideMenu = true, mouseFollowUp = true, structureFlipped = false, sideMenuEtMouseFollowUp;
        protected internal int staleWaitTime = 500, windowSizeX = 1800, windowSizeY = 960;
        private FPSCounter FpsCounter;
        private StructureTemplate selectedStructure;
        private int? indexSelectedStructure = null;
        private readonly int menuHeight = 176;
        private int arrowDirection = 0;
        private StructureManager structureMgr;

        public Game1()
        {
            selectedStructure = null;

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
            structureMgr = new StructureManager();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            tableTexture = new Texture2D(GraphicsDevice, windowSizeX, windowSizeY);

            plusButtonTexture = Content.Load<Texture2D>("Textures/plus");
            minusButtonTexture = Content.Load<Texture2D>("Textures/minus");

            blackRectangle = new Texture2D(GraphicsDevice, 1, 1);
            blackRectangle.SetData(new[] { Color.Black });

            font = Content.Load<SpriteFont>("daFont");

            JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
            generateMenuTexture();
            generateArrowTexture();
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //key manage vars
        private bool wasTABDown, wasMDown, wasCDown, wasQDown, wasEDown, wasRDown, wasWDown, wasSDown;
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
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = windowSizeX;
                    }
                    graphics.ApplyChanges();
                }
            }

            if (wasQDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.Q))
                    wasQDown = false;
            }
            else
            {
                if (sideMenuEtMouseFollowUp && Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                    wasQDown = true;
                    arrowDirection += 1;
                    if (arrowDirection >= 4)
                        arrowDirection = 0;
                    generateArrowTexture();
                    generateStructureTexture();
                }
            }

            if (wasEDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.E))
                    wasEDown = false;
            }
            else
            {
                if (sideMenuEtMouseFollowUp && Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    wasEDown = true;
                    arrowDirection -= 1;
                    if (arrowDirection < 0)
                        arrowDirection = 3;
                    generateArrowTexture();
                    generateStructureTexture();
                }
            }

            if (wasWDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.W))
                    wasWDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    wasWDown = true;
                    if (indexSelectedStructure == null)
                        indexSelectedStructure = 0;
                    else
                    {
                        indexSelectedStructure -= 1;
                        if (indexSelectedStructure < 0)
                            indexSelectedStructure = structureMgr.StructureTemplates.Count - 1;
                    }
                    selectedStructure = structureMgr.StructureTemplates[(int)indexSelectedStructure];
                    generateStructureTexture();
                    generateMenuTexture();
                }
            }

            if (wasSDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.S))
                    wasSDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    wasSDown = true;
                    if (indexSelectedStructure == null)
                        indexSelectedStructure = 0;
                    else {
                        indexSelectedStructure += 1;
                        if (indexSelectedStructure >= structureMgr.StructureTemplates.Count)
                            indexSelectedStructure = 0;
                    }
                    selectedStructure = structureMgr.StructureTemplates[(int)indexSelectedStructure];
                    generateStructureTexture();
                    generateMenuTexture();
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

            if (wasRDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.R))
                    wasRDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    wasRDown = true;
                    structureFlipped = !structureFlipped;
                    generateStructureTexture();
                }
            }

            if (wasCDown)
            {
                if (!Keyboard.GetState().IsKeyDown(Keys.C))
                    wasCDown = false;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    wasCDown = true;
                    JeuDeLaVieTable.AffichageChangement = !JeuDeLaVieTable.AffichageChangement;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        }

        protected override void Update(GameTime gameTime)
        {
            newState = Mouse.GetState();
            sideMenuEtMouseFollowUp = sideMenu && mouseFollowUp;

            if (JeuDeLaVieTable.Stale)
            {
                JeuDeLaVieTable.GenerateNew(tailleX: windowSizeX, tailleY: windowSizeY);
                JeuDeLaVieTable.GenerateInitialImg();
            }
            else { 
                thread1 = new Thread(JeuDeLaVieTable.CalculerCycle)
                {
                    Priority = ThreadPriority.Highest
                };
                thread1.Start();
            }

            DrawThread();
            oldState = newState;
            if (thread1 != null && thread1.IsAlive)
                thread1.Join();

            tableTexture.SetData(JeuDeLaVieTable.DonneeTables);

            KeyManage();
            base.Update(gameTime);
            FpsCounter.Add((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        protected void DrawThread()
        {
            //generate click boxes on top            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(tableTexture, new Vector2(0, 0));
            if (sideMenuEtMouseFollowUp)
            {
                bool statePressed = newState.LeftButton == ButtonState.Pressed && (oldState == null || oldState.LeftButton == ButtonState.Released) && this.IsActive;
                if (statePressed)
                {
                    bool foundSomething = false;
                    if (newState.X >= windowSizeX + 12 && newState.Y >= menuHeight - 30 && newState.X < windowSizeX + 37 && newState.Y < menuHeight - 5)
                    {
                        foundSomething = true;
                        arrowDirection += 1;
                        if (arrowDirection >= 4)
                            arrowDirection = 0;
                        generateStructureTexture();
                        generateArrowTexture();
                    }

                    for (int y = 0; !foundSomething && y < structureMgr.StructureTemplates.Count; y++)
                    {
                        if (newState.X >= windowSizeX + 6 && newState.Y >= menuHeight + 22 * y && newState.X < windowSizeX + 46 && newState.Y < menuHeight + 22 * (y + 1))
                        {
                            foundSomething = true;
                            selectedStructure = structureMgr.StructureTemplates[y];
                            indexSelectedStructure = y;
                            generateStructureTexture();
                            generateMenuTexture();
                        }
                    }
                }

                if (selectedStructure != null && newState.X >= 0 && newState.X < windowSizeX && newState.Y >= 0 && newState.Y < windowSizeY) { 
                    if (statePressed)
                    {
                        if (newState.X < windowSizeX && newState.Y < windowSizeY)
                        {
                            for (int y = 0; y < selectedStructure.getHeight(arrowDirection); y++)
                            {
                                for (int x = 0; x < selectedStructure.getWidth(arrowDirection); x++)
                                {
                                    if (selectedStructure.getValue(arrowDirection, x, y, structureFlipped) ?? false)
                                        JeuDeLaVieTable.setLife(x + newState.X - selectedStructure.getWidth(arrowDirection) / 2, y + newState.Y - selectedStructure.getHeight(arrowDirection) / 2);
                                }
                            }
                        }
                    }
                    spriteBatch.Draw(structureTexture, new Vector2(newState.X - selectedStructure.getWidth(arrowDirection) / 2, newState.Y - selectedStructure.getHeight(arrowDirection) / 2));
                }

                spriteBatch.Draw(menuTexture, new Vector2(windowSizeX, 0));
                spriteBatch.Draw(arrowTexture, new Vector2(windowSizeX + 12, menuHeight - 30));
                spriteBatch.Draw(plusButtonTexture, new Vector2(windowSizeX + 4, 112), scale: new Vector2(1f));
                spriteBatch.Draw(minusButtonTexture, new Vector2(windowSizeX + 27, 112), scale: new Vector2(1f));

                for (int i = 0; i < structureMgr.StructureTemplates.Count; i++)
                {
                    spriteBatch.DrawString(font, structureMgr.StructureTemplates[i].Id, new Vector2(windowSizeX + 6, menuHeight + 4 + i * 22), Color.Black);
                }
            }

            if (FpsCounter.FPSTotal >= FpsCounter.NbFrameCount)
            {
                spriteBatch.DrawString(font, "FPS:" + Environment.NewLine + FpsCounter.AvgFPS, new Vector2(windowSizeX + 6, 30), Color.Black);
                spriteBatch.DrawString(font, "1FPS" + Environment.NewLine + FpsCounter.CurrentFPS, new Vector2(windowSizeX + 6, 70), Color.Black);
            }
            spriteBatch.End();
        }


        private MouseState oldState, newState;
        private void generateMenuTexture()
        {
            Color[] menuMouse = new Color[50*windowSizeY];

            for (int y = 0; y < windowSizeY; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    if(x>=6&&y>=menuHeight&&x<40&&(y-menuHeight)%22!=0&&(y-menuHeight)/22 < structureMgr.StructureTemplates.Count)
                        if (indexSelectedStructure == null || ((y-menuHeight) / 22) != indexSelectedStructure)
                            menuMouse[y * 50 + x] = Color.Green;
                        else
                            menuMouse[y * 50 + x] = Color.Gray;
                    else
                        menuMouse[y * 50 + x] = Color.DarkSalmon;
                }
            }
            menuTexture = new Texture2D(GraphicsDevice, 50, windowSizeY);
            menuTexture.SetData(menuMouse);
        }

        private void generateArrowTexture()
        {
            Color[] arrowMenu = new Color[25 * 25];
            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    for(int subY =0; subY < 5; subY++)
                    {
                        for (int subX = 0; subX < 5; subX++)
                        {
                            arrowMenu[y * 5*5*5 + subY*25 + x * 5 + subX] = (structureMgr.StructureTexture.Find(l => l.Id == "arrow").getValue(arrowDirection, x, y) ?? false) ? Color.Black : Color.Transparent;
                        }
                    }
                }
            }
            arrowTexture = new Texture2D(GraphicsDevice, 25, 25);
            arrowTexture.SetData(arrowMenu);
        }

        private void generateStructureTexture()
        {
            if (selectedStructure != null) { 
                Color[] structureColorArray = new Color[selectedStructure.StructureMap.Length];
                for (int y = 0; y < selectedStructure.getHeight(arrowDirection); y++)
                {
                    for (int x = 0; x < selectedStructure.getWidth(arrowDirection); x++)
                    {
                        if (selectedStructure.getValue(arrowDirection, x, y, structureFlipped) ?? false)
                            structureColorArray[y * selectedStructure.getWidth(arrowDirection) + x] = Color.Black;
                    }
                }
                structureTexture = new Texture2D(GraphicsDevice, selectedStructure.getWidth(arrowDirection), selectedStructure.getHeight(arrowDirection));
                structureTexture.SetData(structureColorArray);
            }
        }

    }
}
