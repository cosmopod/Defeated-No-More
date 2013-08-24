using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimplePlayer
{
    class BasicParticleSystem
    {
        static Random random = new Random();
        List<Particle> particleList = new List<Particle>();
        Texture2D circle;
        int Count = 0;

        public BasicParticleSystem(Texture2D circle)
        {
            this.circle = circle;
        }
        public void AddExplosion(Vector2 position)
        {
            for (int i = 0; i < 250; i++)
            {
                Vector2 velocity2 = (float)random.Next(100) * Vector2.Normalize(new Vector2(((float)random.NextDouble()) - .5f, ((float)random.NextDouble()) - .5f));
                particleList.Add(new Particle(position, velocity2, (i > 70) ? new Vector4(1.0f, 0f, 0f, 1) : new Vector4(.941f, .845f, 0f, 1),
                new Vector4(.2f, .2f, .2f, 0f),
                new TimeSpan(0, 0, 0, 0, random.Next(1000) + 500)));
                Count++;
            }
        }

        public void Update(TimeSpan time, TimeSpan elapsed)
        {
            if (Count > 0)
            {
                for (int i = 0; i < particleList.Count; i++)
                {
                    particleList[i].Update(time, elapsed);
                    if (particleList[i].Delete) particleList.RemoveAt(i);
                }
                Count = particleList.Count;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (Count != 0)
            {
                int particleCount = 0;
                foreach (Particle particle in particleList)
                {
                    batch.Draw(circle,
                    new Vector2(particle.position.X, particle.position.Y),
                    null, new Color(((Particle)particle).Color), 0,
                    new Vector2(16, 16), .2f,
                    SpriteEffects.None, particle.position.Z);
                    particleCount++;
                }
            }
        }

    }
}
