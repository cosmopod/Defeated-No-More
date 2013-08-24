using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimplePlayer
{
   public class FireBall
    {
        Vector2 position;
        Texture2D picture;
        float speed;

        public FireBall(Texture2D firePicture, Vector2 startPosition, float updateSpeed)
        {
            picture = firePicture;
            position = startPosition;
            speed = updateSpeed;
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(picture, position, null, Color.White, 0.0f, new Vector2(10.0f, 10.0f), 1.0f, SpriteEffects.None, 1.0f);
        }

        public void Update(GameTime gametime)
        {
            position.Y -= speed * (float) gametime.ElapsedGameTime.TotalSeconds;
        }
    }
}
