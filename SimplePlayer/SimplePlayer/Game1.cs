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

namespace SimplePlayer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //ASPECTOS GRÁFICOS DEL ENTORNO  ///////////
        GraphicsDeviceManager graphics;
        MultiBackground spaceBackground;
        MultiBackground spaceBackground2;
        MultiBackground spaceBackground3;
        SpriteBatch spriteBatch;
        SpriteFont CourierNew;
        SpriteFont instructionsBeginText;
        SpriteFont instructionsEndText;
        
        //Control del canal alpha en las fuentes
        int mAlphaValue = 1;
        int mFadeIncrement = 10;
        double mFadeDelay = .035;
        float timeIntructionText = 10.0f;
        //posiciones de las fuentes
        Vector2 textLeft;
        Vector2 textMiddle;
        Vector2 posBeginText;
        Vector2 posPlayText;
        Vector2 posEndText;
        BasicParticleSystem particleSystem;
        BasicParticleSystem playerShipExplotion;
        TimeSpan totalTimeSpan = new TimeSpan();
        // ASPECTOS SONOROS DEL JUEGO /////////////
        Song introTheme;
        Song music;
        SoundEffect explosion;
        SoundEffect laser;
        bool gameSongStartedPlaying = false;
        bool gameIntroMusic = false;
        // ENEMIGOS ///////////////////////////////
        public enum EnemyType
        {
            grey,
            black
        }
        Player playerShip;
        List<FireBall> enemyFireballList = new List<FireBall>();
        List<Enemy> enemyShipList = new List<Enemy>();
        Texture2D enemyTextureBlack;
        Texture2D enemyTextureGrey;
        Texture2D enemyFireballTexture;
        Random rnd = new Random();
        public const int numEnemiesOnScreen = 15;

        // JUGADOR ////////////////////////////////
        List<FireBall> playerFireBallList = new List<FireBall>();
        Texture2D playerFireBallTexture;
        float buttonFireDelay = 0.0f; //retraso de disparador para evitar que el polling llene de disparos la pantalla
        float points = 0;


        // ESTADO ///////////////////////////////
        public enum GameState
        {
            StartScreen,
            Running,
            EndScreen
        }

        GameState gameState = GameState.StartScreen;
        Texture2D startScreen; //Splash para inicio y fin de juego
        Texture2D endScreen;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            setWindowSize(1280, 720);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            Window.Title = "Defeated No More";
        }


        public void setWindowSize(int x, int y)
        {
            //configurar la altura y la anchura del back buffer
            this.graphics.PreferredBackBufferWidth = x;
            this.graphics.PreferredBackBufferHeight = y;

            // this.graphics.IsFullScreen = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Instanciamos el objeto que moverá el fondo (scroller) y cargamos las texturas
            spaceBackground = new MultiBackground(graphics);
            spaceBackground2 = new MultiBackground(graphics);
            spaceBackground3 = new MultiBackground(graphics);
            Texture2D spaceTexture = Content.Load<Texture2D>("spaceBackground");
            Texture2D spaceTexture2 = Content.Load<Texture2D>("spaceBackground2");
            Texture2D spaceTexture3 = Content.Load<Texture2D>("spaceBackground3");

            spaceBackground.AddLayer(spaceTexture, 0.1f, 100.0f); //Añadimos el fondo al scroller
            spaceBackground.SetMoveUpDown(); // Configuramos un movimiento vertical del fondo
            spaceBackground.StartMoving(); // bandera del movimiento levantada

            spaceBackground2.AddLayer(spaceTexture2, 0.0f, 130.0f); //Añadimos el fondo al scroller
            spaceBackground2.SetMoveUpDown(); // Configuramos un movimiento vertical del fondo
            spaceBackground2.StartMoving(); // bandera del movimiento levantada

            spaceBackground3.AddLayer(spaceTexture3, 0.0f, 200.0f); //Añadimos el fondo al scroller
            spaceBackground3.SetMoveUpDown(); // Configuramos un movimiento vertical del fondo
            spaceBackground3.StartMoving(); // bandera del movimiento levantada

            //Iniciamos las fuentes
            CourierNew = Content.Load<SpriteFont>("ShipFont");
            instructionsBeginText = Content.Load<SpriteFont>("ShipFont");
            instructionsEndText = Content.Load<SpriteFont>("ShipFont");
       


            //Vectores de posición de las fuentes
            textLeft = new Vector2(graphics.GraphicsDevice.Viewport.Width / 30, graphics.GraphicsDevice.Viewport.Height / 30);
            textMiddle = new Vector2(graphics.GraphicsDevice.Viewport.Width / 1.4f, graphics.GraphicsDevice.Viewport.Height / 30);
            posBeginText = new Vector2(graphics.GraphicsDevice.Viewport.Width / 10.8f, graphics.GraphicsDevice.Viewport.Height / 1.6f);
            posPlayText = new Vector2(graphics.GraphicsDevice.Viewport.Width / 6.0f, graphics.GraphicsDevice.Viewport.Height / 1.6f);
            posEndText = new Vector2(graphics.GraphicsDevice.Viewport.Width / 13.5f, graphics.GraphicsDevice.Viewport.Height / 1.6f);

            //Iniciamos el objeto de la nave del jugador
            Texture2D shipMain = Content.Load<Texture2D>("spaceShip");
            playerShip = new Player(graphics.GraphicsDevice, new Vector2(400.0f, 350.0f), new Vector2(shipMain.Width / 2, shipMain.Height / 2));

            //Cargamos el resto de sprites del jugador  y los cargamos a la lista de cells
            playerShip.AddCell(shipMain); //Frame 0
            Texture2D shipLeft = Content.Load<Texture2D>("shipLeft");
            playerShip.AddCell(shipLeft); // Frame 1
            Texture2D shipRight = Content.Load<Texture2D>("shipRight");
            playerShip.AddCell(shipRight); // Frame 2

            //Cargamos la textura del disparo del jugador
            playerFireBallTexture = Content.Load<Texture2D>("fireBall");

            // Cargamos las texturas de las naves enemigas y su disparo
            enemyTextureGrey = Content.Load<Texture2D>("enemyShip2");
            enemyTextureBlack = Content.Load<Texture2D>("enemyShip3");
            enemyFireballTexture = Content.Load<Texture2D>("enemyFireBall");

            //Cargamos la textura para el efecto de la explosion de las naves
            particleSystem = new BasicParticleSystem(Content.Load<Texture2D>("heartPinkPixel"));
            playerShipExplotion = new BasicParticleSystem(Content.Load<Texture2D>("starExplosion"));


            //Cargamos los splash del juego
            startScreen = Content.Load<Texture2D>("beginSplash");
            endScreen = Content.Load<Texture2D>("endSplash");

            //Cargamos música y sonidos
            introTheme = Content.Load<Song>("introTheme");
            music = Content.Load<Song>("music");
            SoundEffect.MasterVolume = 0.2f;
            explosion = Content.Load<SoundEffect>("explosion");
            laser = Content.Load<SoundEffect>("laser");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Control del delay para el canal alpha de las fuentes
            mFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            if (mFadeDelay <= 0)
            {
                //Reseteamos el delay del efecto
                mFadeDelay = .035;

                //Aumentamos o disminuimos el valor del tiempo del efecto
                mAlphaValue += mFadeIncrement;

                //Controlamos el máximo y el mínimo del valor para el canal alpha
                if (mAlphaValue >= 255 || mAlphaValue <= 0)
                {
                    mFadeIncrement *= -1;
                }
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (gameState == GameState.StartScreen || gameState == GameState.EndScreen)
            {
                if (!gameIntroMusic)
                {
                    MediaPlayer.Play(introTheme);
                    MediaPlayer.IsRepeating = true;
                    gameIntroMusic = true;
                }

                UpdateSplashScreen();
            }
            else // El juego está activo
            {
                timeIntructionText -= (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (!gameSongStartedPlaying)
                {
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;
                    MediaPlayer.Play(music);
                    MediaPlayer.IsRepeating = true;
                    gameSongStartedPlaying = true;
                }
                //Intervalo de tiempos para las explosiones
                totalTimeSpan += gameTime.ElapsedGameTime;
                particleSystem.Update(totalTimeSpan, gameTime.ElapsedGameTime);
                playerShipExplotion.Update(totalTimeSpan, gameTime.ElapsedGameTime);

                spaceBackground.Update(gameTime);
                spaceBackground2.Update(gameTime);
                spaceBackground3.Update(gameTime);
                // TODO: Add your update logic here //////////
                UpdateInput(); //Actualización de los input de los controles
                playerShip.Update(gameTime);

                //aplicamos el retraso en el polling de los disparos
                //float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                buttonFireDelay -= elapsed;


                //actualizamos la posicion de los disparos de la nave
                for (int i = 0; i < playerFireBallList.Count; i++)
                {
                    playerFireBallList[i].Update(gameTime);
                    if (playerFireBallList[i].Position.Y < -100.0f) //Eliminamos el objeto de la lista cuando sale de pantalla
                        playerFireBallList.RemoveAt(i);
                }

                //Actualizamos la posicíon de los enemigos y de los disparos

                for (int i = 0; i < enemyShipList.Count; i++)
                {
                    enemyShipList[i].Update(gameTime);
                    if (enemyShipList[i].Firing)
                    {
                        FireBall fireball = new FireBall(enemyFireballTexture, new Vector2(enemyShipList[i].Position.X + 10.0f, enemyShipList[i].Position.Y + 30.0f), -300.0f);
                        enemyFireballList.Add(fireball);
                        enemyShipList[i].Firing = false;
                    }

                    // Colisión de los disparos del jugador con los enemigos y de las naves con el jugador
                    if (enemyShipList.Count > 0) //De momento nos aseguramos de que haya al menos un enemigo en lista
                    {
                        int collide = enemyShipList[i].CollisionBall(playerFireBallList);

                        if (collide != -1)
                        {
                            particleSystem.AddExplosion(enemyShipList[i].Position * new Vector2(1.0f, 1.2f));
                            enemyShipList.RemoveAt(i);
                            playerFireBallList.RemoveAt(collide);
                            points += 200;
                            explosion.Play();
                        }
                        else
                        {
                            if (!playerShip.RecoveringActive) //Controlamos si la nave acaba de ser destruida
                            {
                                if (playerShip.CollisionTest(enemyShipList[i].Position, enemyShipList[i].Radius))
                                {
                                    particleSystem.AddExplosion(enemyShipList[i].Position * new Vector2(1.0f, 1.1f));
                                    playerShipExplotion.AddExplosion(playerShip.Posicion);
                                    enemyShipList.RemoveAt(i);
                                    explosion.Play();
                                }
                                else if (enemyShipList[i].Position.Y > 900.0f) //eliminamos los enemigos que salen por debajo sin colisionar
                                    enemyShipList.RemoveAt(i);
                            }
                        }

                    }


                }

                //Colisión de los disparos enemigos con el jugador
                for (int i = 0; i < enemyFireballList.Count; i++)
                {
                    enemyFireballList[i].Update(gameTime);

                    if (!playerShip.RecoveringActive) //Controlamos si la nave en ese momento acaba de ser destruida
                    {
                        if (playerShip.CollisionTest(enemyFireballList[i].Position, 20.0f))
                        {

                            enemyFireballList.RemoveAt(i); //eliminamos el disparo
                            playerShipExplotion.AddExplosion(playerShip.Posicion);

                            explosion.Play();
                        }
                        else if (enemyFireballList[i].Position.Y > 1000.0f) //eliminamos los disparos enemigos que salen por debajo sin colisionar
                            enemyFireballList.RemoveAt(i);
                    }

                }

                CreateEnemyTime();
                if (playerShip.Lives < 0)
                {
                    gameState = GameState.EndScreen;
                    playerShip.Posicion = new Vector2(400.0f, 500.0f);
                    playerShip.Lives = 5;
                    this.points = 0;
                    enemyFireballList.Clear();
                    enemyShipList.Clear();
                    playerFireBallList.Clear();
                    UpdateSplashScreen();
                    MediaPlayer.Stop();
                    gameSongStartedPlaying = false;
                    timeIntructionText = 5.0f;
                }
            }


            base.Update(gameTime);
        }
        private void UpdateSplashScreen() //Método para actualizar el estado del juego
        {
            KeyboardState keyState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || keyState.IsKeyDown(Keys.Space))
            {

                gameState = GameState.Running;

            }
        }
        private void UpdateInput()
        {
            // cremos los objetos que recogerán el estado del teclado y del mando XBox (método polling)
            KeyboardState keystate = Keyboard.GetState();
            GamePadState gamePadSate = GamePad.GetState(PlayerIndex.One);

            //Recogemos los inputs de la posición de la nave //////////////////

            if (keystate.IsKeyDown(Keys.W) || gamePadSate.DPad.Up == ButtonState.Pressed)
            {
                playerShip.Acelerar();
            }

            if (keystate.IsKeyDown(Keys.S) || gamePadSate.DPad.Down == ButtonState.Pressed)
            {
                playerShip.Retroceder();
            }

            if (keystate.IsKeyDown(Keys.A) || gamePadSate.DPad.Left == ButtonState.Pressed)
            {
                playerShip.GirarIzquierda();
            }

            else if (keystate.IsKeyDown(Keys.D) || gamePadSate.DPad.Right == ButtonState.Pressed)
            {
                playerShip.GirarDerecha();
            }
            else
            {
                playerShip.Enderezar();
            }

            //Inputs para los disparos ///////////////////////////
            if (gamePadSate.Buttons.A == ButtonState.Pressed || gamePadSate.Triggers.Right >= 0.5f || keystate.IsKeyDown(Keys.P))
            {
                if (buttonFireDelay <= 0.0f)
                {
                    FireBall shot = new FireBall(playerFireBallTexture, new Vector2(playerShip.Posicion.X, playerShip.Posicion.Y - 10.0f), 300.0f);
                    playerFireBallList.Add(shot);
                    laser.Play();

                    buttonFireDelay = 0.20f;
                }
            }

        }

        public void CreateEnemy(EnemyType enemyType) // Función para crear enemigos
        {

            //Random r = new Random();
            int startX = RndNumber(1, 1200);
            if (enemyType == EnemyType.black)
            {
                Enemy enemy = new Enemy(enemyTextureBlack, new Vector2(startX, -100), 150.0f);
                enemy.SetAcrossMovement((float)startX / 1200.0f * 250.0f, 100.0f);
                enemyShipList.Add(enemy);

            }
            if (enemyType == EnemyType.grey)
            {
                Enemy enemy = new Enemy(enemyTextureGrey, new Vector2(startX, -100), 150.0f);
                enemy.Firing = true;
                enemyShipList.Add(enemy);

            }

        }

        public void CreateEnemyTime() //Creacion de un num minimo de enemigos
        {

            int numEnemies = enemyShipList.Count;
            if (numEnemies < numEnemiesOnScreen)
            {

                CreateEnemy(EnemyType.black);
                CreateEnemy(EnemyType.grey);
            }

        }

        public void DrawText(SpriteBatch textBatch) //Método para escribir marcadores en pantalla
        {
            string output = "1UP: " + playerShip.Lives.ToString();
            textBatch.DrawString(CourierNew, output, textLeft, Color.FromNonPremultiplied(222,73,209, 256));
            string pointString = points.ToString();
            for (int i = pointString.Length; i < 8; i++)
                pointString = "0" + pointString;
            pointString = "Pts: " + pointString;
            textBatch.DrawString(CourierNew, pointString, textMiddle, Color.FromNonPremultiplied(222,73,209,256));
        }
        public void DrawBeginText(SpriteBatch textBatch) //Textos del inicio
        {
            String output = "Press Start Button or Spacebar to Begin";
            textBatch.DrawString(instructionsBeginText, output, posBeginText, Color.FromNonPremultiplied(222, 73, 209, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));
        }
        public void DrawEndText(SpriteBatch textBatch) //Textos del inicio
        {
            String output = "Press Start Button or Spacebar to Restart";
            textBatch.DrawString(instructionsBeginText, output, posEndText, Color.FromNonPremultiplied(222, 73, 209, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));
        }


        public void DrawIntructionText(SpriteBatch textBatch) //Textos de instruciones
        {
            String output = "Use the WASD keys and P to Shot";
            textBatch.DrawString(instructionsBeginText, output, posPlayText, Color.FromNonPremultiplied(222, 73, 209, (byte)MathHelper.Clamp(mAlphaValue, 0, 255)));
        }
        private void DrawStartScreen() //Método para escribir la pantalla de inicio
        {
            spriteBatch.Begin();
            spriteBatch.Draw(startScreen, Vector2.Zero, null, Color.White, 0.0f, new Vector2(0, 0), 0.67f, SpriteEffects.None, 0.0f);
            DrawBeginText(spriteBatch);
            spriteBatch.End();
        }

        private void DrawEndScreen() //Método para escribir la pantalla de fin
        {
            spriteBatch.Begin();
            spriteBatch.Draw(endScreen, Vector2.Zero, null, Color.White, 0.0f, new Vector2(0, 0), 0.67f, SpriteEffects.None, 0.0f);
            DrawEndText(spriteBatch);
            spriteBatch.End();
        }

        public static int RndNumber(int low, int high) // Método para obtener random con una semilla distinta al tiempo del sistema
        {
            Random rnd = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));

            int rndNumber = rnd.Next(low, high);
            return rndNumber;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (gameState == GameState.StartScreen)
            {
                DrawStartScreen();
            }
            else if (gameState == GameState.Running)
            {
                spriteBatch.Begin();
                spaceBackground.Draw(); //Dibujamos el fondo
                spaceBackground2.Draw();
                spaceBackground3.Draw();
                if (timeIntructionText > 0)
                    DrawIntructionText(spriteBatch);
                //La nave usa su método Draw()
                if (!playerShip.BlinkOn)
                {
                    playerShip.Draw(spriteBatch);

                }
                playerShip.BlinkOn = false; //Devolvemos el parpadeo a su estado inicial
                //Dibujamos cada Fireball
                foreach (FireBall f in playerFireBallList)
                {
                    f.Draw(spriteBatch);
                }

                //Dibujamos los enemigos y sus disparos
                foreach (Enemy enemy in enemyShipList)
                {
                    enemy.Draw(spriteBatch);
                }
                foreach (FireBall fireball in enemyFireballList)
                {
                    fireball.Draw(spriteBatch);
                }
                particleSystem.Draw(spriteBatch);// Dibujamos las explosiones
                playerShipExplotion.Draw(spriteBatch);

                DrawText(spriteBatch); // Dibujamos los textos
                spriteBatch.End(); // Fin del Batch
            }
            else if (gameState == GameState.EndScreen)
            {
                DrawEndScreen();
                
                
            }

            base.Draw(gameTime);
        }
    }
}
