using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimplePlayer
{
    struct AnimationCell
    {
        public Texture2D cell;
    }
    class Animation
    {
        int currentCell = 0;
        bool looping = false;
        bool stopped = false;
        bool playing = false;
        // Tiempo que necesitamos para ir al siguiente frame
        float timeShift = 0.0f;
        // Tiempo transcurrido desde el último cambio de frame
        float totalTime = 0.0f;
        int start = 0, end = 0;
        List<AnimationCell> cellList = new List<AnimationCell>();
        Vector2 position;
        float scale = 1.0f;
        SpriteEffects spriteEffect = SpriteEffects.None;
        Vector2 spriteOrigin = Vector2.Zero;

        public Animation(Vector2 position) // toma la posición donde será dibujada en pantalla la animación
        {
            this.position = position;
        }

        public float Scale
        {
            set
            {
                scale = value;
            }
        }

        public Vector2 SpriteOrigin
        {
            set
            {
                spriteOrigin = value;
            }
        }      

        public void AddCell(Texture2D cellPicture)
        {
            AnimationCell cell = new AnimationCell();
            cell.cell = cellPicture;
            cellList.Add(cell);
        }

        public void SetPosition(float x, float y)
        {
            position.X = x;
            position.Y = y;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;

        }

        public void SetMoveLeft()
        {
            spriteEffect = SpriteEffects.FlipHorizontally;
        }

        public void SetMoveRight()
        {
            spriteEffect = SpriteEffects.None;
        }

        public void LoopAll(float seconds)
        {
            if (playing) return;
            stopped = false;
            if (looping) return;
            looping = true;
            start = 0;
            end = cellList.Count - 1;
            currentCell = start;
            timeShift = seconds / (float)cellList.Count;
        }

        public void Loop(int start, int end, float seconds)
        {
            if (playing) return;
            stopped = false;
            if (looping) return;
            looping = true;
            this.start = start;
            this.end = end;
            currentCell = start;
            float difference = (float)end - (float)start;
            timeShift = seconds / difference;
        }

        public void Stop()
        {
            if (playing) return;
            stopped = true;
            looping = false;
            totalTime = 0.0f;
            timeShift = 0.0f;
        }

        public void GotoFrame(int number) //Nos permite ir a un frame concreto de la lista (el primero es 0)
        {
            if (playing) return;
            if (number < 0 || number >= cellList.Count) return;
            currentCell = number;
        }

        public void PlayAll(float seconds)
        {
            if (playing) return;
            GotoFrame(0);
            stopped = false;
            looping = false;
            playing = true;
            start = 0;
            end = cellList.Count - 1;
            timeShift = seconds / (float)cellList.Count;
        }

        public void Play(int start, int end, float seconds)
        {
            if (playing) return;
            GotoFrame(start);
            stopped = false;
            looping = false;
            playing = true;
            this.start = start;
            this.end = end;
            float difference = (float)end - (float)start;
            timeShift = seconds / difference;
        }

        public void Draw(SpriteBatch batch)
        {
            if (cellList.Count == 0 || currentCell < 0 ||
            cellList.Count <= currentCell) return;
            batch.Draw(cellList[currentCell].cell, position, null, Color.White, 0.0f,
            spriteOrigin,
            new Vector2(scale, scale), spriteEffect, 0.0f);
        }

        public void Update(GameTime gameTime)
        {
            if (stopped) return;
            totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (totalTime > timeShift)
            {
                totalTime = 0.0f;
                currentCell++;
                if (looping)
                {
                    if (currentCell > end) currentCell = start;
                }
                if (currentCell > end)
                {
                    currentCell = end;
                    playing = false;
                }
            }
        }
    }



}
