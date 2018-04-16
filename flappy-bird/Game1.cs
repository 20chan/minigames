using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Grid.Framework;
using Grid.Framework.Components;

namespace flappy_bird
{
    public class Game1 : Scene
    {
        Bird bird;
        Queue<Hurdle> hurdles;

        protected override void InitSize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            base.InitSize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            var birdObj = new GameObject("bird");
            birdObj.AddComponent<Bird>();
            birdObj.AddComponent<Renderable2D>().Texture = LoadContent<Texture2D>("bird");
            birdObj.Scale = new Vector2(0.3f, 0.3f);

            bird = Instantiate(birdObj).GetComponent<Bird>();
            hurdles = new Queue<Hurdle>();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

    class Bird : Component
    {
        private readonly Vector2 gravity;
        private readonly float gravityPower = 0.3f;
        private readonly float jumpPower = 15f;

        private Vector2 _velocity;

        public Bird()
        {
            gravity = new Vector2(0, gravityPower);
        }

        bool isSpaceNotPressing = true;
        public override void Update()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && isSpaceNotPressing)
            {
                isSpaceNotPressing = false;
                this._velocity = new Vector2(0, -jumpPower);
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Space) && !isSpaceNotPressing)
                isSpaceNotPressing = true;

            _velocity += gravity;
            gameObject.Position += _velocity;
            base.Update();
        }

        public override object Clone()
        {
            return this;
        }
    }

    class Hurdle : Component
    {
        private int _height;
        private GameObject _up, _down;

        public override void Start()
        {
            
            base.Start();
        }

        public override object Clone()
        {
            return new Hurdle()
            {

            };
        }
    }
}
