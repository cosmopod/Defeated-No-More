using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimplePlayer
{
  public class Enemy
    {
        // Propiedades ///////////////////////////
        Vector2 position;
        Texture2D picture;
        float speed = 150.0f;
        // Las siguientes tres variables guardan y describen el movimiento horizontal en pantalla
        float deltaX = 0.0f; // cuánto se mueve horizantamente en cada refresco pixels/seg
        float xLength = 0.0f;// Esta variable y la siguientes acortan el movimiento horizontal para que no recorra toda la pantalla
        float xStart = 0.0f;
        // Describe y guarda el estado del fuego enemigo
        bool firingActive = false;
        bool firing = false;
        float fireSpeed = 1.0f; // Frecuencia de disparo (1.0f equivale a un disapro por segundo)
        float totalTime = 0.0f; //Contador del tiempo que llevamos disparando
        float radius = 40.0f; // radio de la esfera para las colisiones

        public Enemy(Texture2D picture, Vector2 startPosition, float speed)
        {
            this.picture = picture;
            position = startPosition;
            this.speed = speed;
        }

        //Métodos getter y setter
        public bool FiringActive
        {
            set { firingActive = value; }
        }

        public bool Firing
        {
            set { firing = value; }
            get { return firing; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public float Radius
        {
            get { return radius; }
        }

        public void SetAcrossMovement(float deltaX, float xLength) //Establece el movimiento horizontal del enemigo
        {
            this.deltaX = deltaX;
            this.xLength = xLength;
            xStart = position.X;
        }
        public int CollisionBall(List<FireBall> fireBallList)
        {
            for (int i = 0; i < fireBallList.Count; i++)
            {
                if ((fireBallList[i].Position - position).Length() < radius)   
                    return i;       
            }
            return -1;
        }
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(picture, position, null, Color.White, 0.0f, new Vector2(
            40.0f, 20.0f), 1.0f, SpriteEffects.None, 0.0f);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.X += deltaX * elapsed;
            if (Position.X < xStart - xLength || Position.X > xStart + xLength)  // se ha desplazdo lo suficiente por la pantalla? cambiamos la dirección
                deltaX *= -1.0f;
            position.Y += speed * elapsed;
            if (firingActive)
            {
                totalTime += elapsed;
                if (totalTime > fireSpeed)
                {
                    totalTime = 0.0f;
                    firing = true;
                }
            }
        }

    }
}
