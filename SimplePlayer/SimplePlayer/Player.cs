using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Design;

namespace SimplePlayer
{
    class Player
    {
        Vector2 posicion;
        int windowWidth, windowHeight;
        float velocidad = 0.0f;
        Animation playerAnimation;

        int lives = 5; // número de vidas

        //Colisiones /////////////////
        float radius = 30.0f;
        // recuperar el estado después de la colisión
        float blinkTime = 0.0f;
        float blinkTimeTotal = 0.0f;
        bool blinkOn = false;
        bool recoveringActive = false;
        const float recoverLength = 3.0f;


        public Player(GraphicsDevice device, Vector2 posicion, Vector2 origin)
        {

            //Toma la posicón que le pasa el método
            this.posicion = posicion;

            //Crea la animacion
            playerAnimation = new Animation(posicion);

            //Origen del sprite, su centro desde el que será dibujado es el centro del sprite de la nave
            playerAnimation.SpriteOrigin = origin;
            playerAnimation.Stop();


            //Dimensiones de la pantalla (tomamos la info del área de dibujo)
            windowWidth = device.Viewport.Width;
            windowHeight = device.Viewport.Height;


        }
        public bool BlinkOn
        {
            get
            {
                return blinkOn;
            }
            set
            {
                blinkOn = value;
            }
        }
        public bool RecoveringActive
        {
            get
            {
                return recoveringActive;
            }
        }
        public int Lives
        {
            get
            {
                return lives;

            }
            set { lives = value; }
        }

        public void AddCell(Texture2D cellPicture)
        {
            playerAnimation.AddCell(cellPicture);
        }

        public void Draw(SpriteBatch batch)
        {
            playerAnimation.SetPosition(posicion);
            playerAnimation.Draw(batch);
        }

        public void Update(GameTime gameTime)
        {
            playerAnimation.Update(gameTime);
            //Actualizamos la posición en el eje Y valiendonos del valor de velocidad
            posicion.Y += velocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Sensación de aceleración (mientras más cerca wste de 1 la sensación de rozamiento será menor
            velocidad *= 0.95f;
            // Nos aseguramos que la nave en ningún momento desaparezca de la pantalla (márgenes de Viewport)
            if (posicion.X > windowWidth) posicion.X = windowWidth;
            if (posicion.Y > windowHeight) posicion.Y = windowHeight;
            if (posicion.X < 0) posicion.X = 0.0f;
            if (posicion.Y < 0) posicion.Y = 0.0f;

            // cambios de estado en caso de colisión
            if (recoveringActive)
            {
                const float blinkTimeSlice = 1.0f / 15.0f;
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                blinkTime += elapsed;
                blinkTimeTotal += elapsed;
                if (blinkTimeTotal > recoverLength)
                    recoveringActive = false;
                else
                {
                    if (blinkTime > blinkTimeSlice)
                    {
                        blinkOn = !blinkOn;
                        blinkTime = 0.0f;
                    }
                    
                }
                
            }

        }
        public bool CollisionTest(Vector2 point, float radius) // Método detector de colisiones
        {
            if ((point - posicion).Length() < this.radius + radius) //Si la suma de los dos radios es menor que la distancia entre sus centros, hay colision
            {
                if (!recoveringActive)
                {
                    lives--;
                    recoveringActive = true;
                    blinkTimeTotal = 0.0f;
                }
                return true;
            }
            return false;
        }

        public void Acelerar()
        {
            playerAnimation.GotoFrame(0);
            velocidad -= 20.0f;
        }

        public void Retroceder()
        {
            velocidad += 20.0f;
        }

        public void GirarIzquierda()
        {
            playerAnimation.GotoFrame(1);
            posicion.X -= 3.0f;
        }

        public void GirarDerecha()
        {
            playerAnimation.GotoFrame(2);
            posicion.X += 3.0f;
        }
        public void Enderezar()
        {
            playerAnimation.GotoFrame(0);
        }

        //Posicion de la nave
        public Vector2 Posicion
        {
            get
            {
                return posicion;
            }
            set { posicion = value; }
        }


    }
}
