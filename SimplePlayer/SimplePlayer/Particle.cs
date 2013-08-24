using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SimplePlayer
{
    class Particle
    {
        Vector4 color; //Usamos vectores 4D para los colores
        Vector4 startColor;
        Vector4 endColor;
        TimeSpan endTime = TimeSpan.Zero;
        TimeSpan lifetime;
        public Vector3 position;
        Vector3 velocity;
        protected Vector3 acceleration = new Vector3(1.0f, 1.0f, 1.0f);
        public bool Delete;

        public Particle(Vector2 position2, Vector2 velocity2, Vector4 startColor, Vector4 endColor, TimeSpan lifetime)
        {
            velocity = new Vector3(velocity2, 0.0f);
            position = new Vector3(position2, 0.0f);
            this.startColor = startColor;
            this.endColor = endColor;
            this.lifetime = lifetime;
        }

        public Vector4 Color
        {
            get
            {
                return color;
            }
        }

        public void Update(TimeSpan time, TimeSpan elapsedTime)
        {

            if (endTime == TimeSpan.Zero) //Comienza a animación por primera vez
            {
                endTime = time + lifetime;
            }
            if (time > endTime)
            {
                Delete = true;
            }
            float percentLife = (float)((endTime.TotalSeconds - time.TotalSeconds)
            / lifetime.TotalSeconds);
            color = Vector4.Lerp(endColor, startColor, percentLife);
            velocity += Vector3.Multiply(acceleration, (float)elapsedTime.TotalSeconds);
            position += Vector3.Multiply(velocity, (float)elapsedTime.TotalSeconds);
        }

    }
}
