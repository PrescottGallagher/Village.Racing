using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Village_Racing
{
    public class Camera
    {
        public bool rotating;
        public bool left;
        public int rotationCount;

        public Camera(Viewport viewport)
        {
            Origin = new Vector2(viewport.Width / 2.0f, viewport.Height / 2.0f);
            Zoom = 1.0f;
            _viewport = viewport;
        }

        

        public Rectangle? Limits
        {
            get { return _limits; }
            set
            {
                if (value != null)
                {
                    _limits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = System.Math.Max(_viewport.Width, value.Value.Width),
                        Height = System.Math.Max(_viewport.Height, value.Value.Height)
                    };
                    Position = Position;
                }
                else
                {
                    _limits = null;
                }
            }
        }

        private Rectangle? _limits;
        private readonly Viewport _viewport;
        private static Vector2 _position;
        public Vector2 lastPosition;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                
                _position = value;
                if (Limits != null && Zoom == 1.0f && Rotation == 0.0f)
                {
                    _position.X = MathHelper.Clamp(value.X, Limits.Value.X, Limits.Value.X + Limits.Value.Width - _viewport.Width);
                    _position.Y = MathHelper.Clamp(value.Y, Limits.Value.Y, Limits.Value.Y + Limits.Value.Height - _viewport.Height);
                    
                }
            }
        }

        public void Rotate90(bool left)
        {
            rotating = true;
            this.left = left;
            rotationCount = 0;
        }
        public Vector2 Origin { get; set; }
        public float Zoom { get; set; }
        public float Rotation { get; set; }

        public void LookAt(Vector2 position)
        {
            Position = position - Origin;
        }

        public void Update()
        {
            if (rotating)
            {
                if (left)
                {
                    Rotation -= (float)((Math.PI / 2.0f) / 60);
                }
                else
                {
                    Rotation += (float)((Math.PI / 2.0f) / 60);
                }
                rotationCount++;
                if (rotationCount == 60)
                {
                    rotating = false;
                }
            }
        }

        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
    }
}
