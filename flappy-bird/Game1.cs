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
            bird = birdObj.AddComponent<Bird>();
            birdObj.AddComponent<Renderable2D>().Texture = LoadContent<Texture2D>("bird");
            birdObj.Scale = new Vector2(0.3f, 0.3f);

            Hurdle.bird = bird;

            Instantiate(birdObj);
            hurdles = new Queue<Hurdle>();
            Hurdle.upPrefab = LoadContent<Texture2D>("up");
            Hurdle.downPrefab = LoadContent<Texture2D>("down");

            //SpawnHurdle();
        }

        private Hurdle SpawnHurdle()
        {
            var obj = new GameObject("hurdle");
            obj.AddComponent<Hurdle>();
            Instantiate(obj);
            return obj.GetComponent<Hurdle>();
        }

        int i = 0;
        protected override void Update(GameTime gameTime)
        {
            if (i > 150)
            {
                hurdles.Enqueue(SpawnHurdle());
                i = 0;
            }

            i++;
            base.Update(gameTime);
        }
    }

    class Bird : Component
    {
        private readonly Vector2 gravity;
        private readonly float gravityPower = 0.5f;
        private readonly float jumpPower = 20f;

        private int width, height;
        public Rectangle Bounds =>
            new Rectangle(
                (int)gameObject.AbsolutePosition.X,
                (int)gameObject.AbsolutePosition.Y,
                width,
                height);

        private Vector2 _velocity;

        public Bird()
        {
            gravity = new Vector2(0, gravityPower);
        }

        public override void Start()
        {
            var b = gameObject.GetComponent<Renderable2D>().Texture.Bounds;
            width = b.Width;
            height = b.Height;
            base.Start();
        }

        bool isSpaceNotPressing = true;
        public override void Update()
        {
            Debug.DisplayLine(Bounds.ToString());
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
    }

    class Hurdle : Component
    {
        private static System.Random random = new System.Random();
        private int _height;
        private GameObject _up, _down;

        public static Texture2D upPrefab, downPrefab;
        public static Bird bird;

        public Hurdle() { }
        
        public override void Start()
        {
            _height = random.Next(-750, -300);

            _up = new GameObject("hurdle_up", gameObject);
            _down = new GameObject("hurdle_down", gameObject);

            _up.AddComponent<Renderable2D>().Texture = upPrefab;
            _down.AddComponent<Renderable2D>().Texture = downPrefab;

            _up.Position = new Vector2(0, -0);
            _down.Position = new Vector2(0, 1000);

            Instantiate(_up);
            Instantiate(_down);

            gameObject.Position = new Vector2(1700, _height);
            base.Start();
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                gameObject.Position = new Vector2(gameObject.Position.X - 10, gameObject.Position.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                gameObject.Position = new Vector2(gameObject.Position.X + 10, gameObject.Position.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                gameObject.Position = new Vector2(gameObject.Position.X, gameObject.Position.Y - 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                gameObject.Position = new Vector2(gameObject.Position.X, gameObject.Position.Y + 10);
            }
            // gameObject.Position = new Vector2(gameObject.Position.X - 10, gameObject.Position.Y);
            if (ISCollided(bird.Bounds))
                Damn();
            else
                NotDamn();

            if (gameObject.Position.X < -1700)
                Destroy(gameObject);
            base.Update();
        }
        
        public Rectangle UpBounds
            => new Rectangle(
                (int)_up.AbsolutePosition.X,
                (int)_up.AbsolutePosition.Y,
                (int)upPrefab.Width,
                (int)upPrefab.Height);

        public Rectangle DownBounds
            => new Rectangle(
                (int)_down.AbsolutePosition.X,
                (int)_down.AbsolutePosition.Y,
                (int)downPrefab.Width,
                (int)downPrefab.Height);

        public bool ISCollided(Rectangle bound)
            => UpBounds.Intersects(bound)
                || DownBounds.Intersects(bound);

        private void Damn()
        {
            _up.GetComponent<Renderable2D>().Color = Color.Red;
            _down.GetComponent<Renderable2D>().Color = Color.Red;
        }

        private void NotDamn()
        {
            _up.GetComponent<Renderable2D>().Color = Color.White;
            _down.GetComponent<Renderable2D>().Color = Color.White;
        }
    }
}
