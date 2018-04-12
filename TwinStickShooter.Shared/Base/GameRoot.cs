using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TwinStickShooter
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class GameRoot : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public static GameRoot Instance { get; private set; }
		public static GameTime GameTime { get; private set; }
		public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
		public static Vector2 ScreenSize { get { return new Vector2 (Viewport.Width, Viewport.Height); } }

		public static ParticleManager<ParticleState> ParticleManager { get; private set; }

		public GameRoot()
		{
			graphics = new GraphicsDeviceManager (this)
			{
				PreferredBackBufferHeight = 1080,//720;
				PreferredBackBufferWidth = 1920,//1280;
				IsFullScreen = true
			};

			Content.RootDirectory = "Content";
			Instance = this;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize ();

			// instantiate player
			EntityManager.Add (PlayerShip.Instance);
			ParticleManager = new ParticleManager<ParticleState> (1024 * 20, ParticleState.UpdateParticle);

			// start background music and set it to looping
			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play (Sound.Music);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			Art.Load (Content);
			Sound.Load (Content);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
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
			GameTime = gameTime;
			// game exit scenarios
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			
			// call update methods for required classes
			Input.Update (gameTime);
			EntityManager.Update (gameTime);
			EnemySpawner.Update (gameTime);
			ParticleManager.Update (gameTime);

			// only call if the game is active
			if(IsActive)
			{
				PlayerStatus.Update (gameTime);
			}

			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.Black);

			// draw every entity and mouse coursor
			spriteBatch.Begin (SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw (spriteBatch, gameTime);

			// draw the custom mouse cursor
			spriteBatch.Draw (Art.Pointer, Input.MousePosition, Color.White);
			spriteBatch.End ();

			spriteBatch.Begin (SpriteSortMode.Deferred, BlendState.Additive);
			ParticleManager.Draw (spriteBatch, gameTime);
			spriteBatch.End ();

			// draw text in a separate batch (did this for experimental reasons)
			spriteBatch.Begin (0, BlendState.Additive);
			spriteBatch.DrawString (Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2 (5), Color.White);
			DrawRightAlignedString ("Score: " + PlayerStatus.Score, 5);
			DrawRightAlignedString ("Multiplier: " + PlayerStatus.Multiplier, 35);

			if (PlayerStatus.IsGameOver)
			{
				string text = "Game Over\n" +
					"Your Score: " + PlayerStatus.Score + "\n" +
					"High Score: " + PlayerStatus.HighScore;

				Vector2 textSize = Art.Font.MeasureString (text);
				spriteBatch.DrawString (Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
			}

			spriteBatch.End ();

			base.Draw (gameTime);
		}

		private void DrawRightAlignedString(string text, float y)
		{
			var textWidth = Art.Font.MeasureString (text).X;
			spriteBatch.DrawString (Art.Font, text, new Vector2 (ScreenSize.X - textWidth - 5, y), Color.White);
		}
	}
}
