
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TwinStickShooter
{
	class PlayerShip : Entity
	{
		private static PlayerShip instance;
		public static PlayerShip Instance
		{
			get
			{
				if (instance == null)
					instance = new PlayerShip ();

				return instance;
			}
		}

		const float speed = 400; // pixels per second
		const float bulletSpeed = 650;
		const float cooldownTime = 0.1f; // seconds
		float cooldownRemaining = 0;
		float timeUntilRespawn = 0;

		static Random rand = new Random ();

		public bool IsDead { get { return timeUntilRespawn > 0; } }

		private PlayerShip()
		{
			image = Art.Player;
			position = GameRoot.ScreenSize / 2;
			radius = 10;
		}

		public override void Update(GameTime gameTime)
		{
			// if gameover wait for key press and then restart the game
			if(PlayerStatus.IsGameOver)
			{
				timeUntilRespawn = 0.25f;

				if(Input.WasKeyPressed(Keys.Enter) || Input.WasButtonPressed(Buttons.Start))
				{
					PlayerStatus.Reset ();
				}
				return;
			}

			if(IsDead)
			{
				// wait for time to respawn
				timeUntilRespawn -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				if(timeUntilRespawn <= 0)
				{

				}
				return;
			}

			// update velocity and position
			velocity += speed * Input.GetMovementDirection ();
			position += velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

			// make sure the ship is always in the screen bounds
			position = Vector2.Clamp (position, Size / 2, GameRoot.ScreenSize - Size / 2);

			// shoot a bullet in the aim direction when the cooldown is also finnished
			var aim = Input.GetAimDirection ();
			if (aim.LengthSquared () > 0 && cooldownRemaining <= 0)
			{
				cooldownRemaining = cooldownTime;
				float aimAngle = aim.ToAngle ();
				Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll (0, 0, aimAngle);

				float randomSpread = rand.NextFloat (-0.04f, 0.04f) + rand.NextFloat (-0.04f, 0.04f);
				Vector2 vel = MathUtil.FromPolar (aimAngle + randomSpread, bulletSpeed);
				
				// spawn two bullets infront of the player. one on the left side and other on the right
				Vector2 offset = Vector2.Transform (new Vector2 (25, -8), aimQuat);
				EntityManager.Add (new Bullet (position + offset, vel));

				offset = Vector2.Transform (new Vector2 (25, 8), aimQuat);
				EntityManager.Add (new Bullet (position + offset, vel));

				Sound.Shot.Play (0.2f, rand.NextFloat (-0.2f, 0.2f), 0);
			}

			// make the ship look in the direction of player
			if (aim.LengthSquared () > 0)
				orientation = aim.ToAngle ();

			if (cooldownRemaining > 0)
				cooldownRemaining -= (float) gameTime.ElapsedGameTime.TotalSeconds;

			velocity = Vector2.Zero;
		}

		public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			// draw the player only if it is alive
			if(!IsDead)
				base.Draw (spriteBatch, gameTime);
		}

		public void Kill()
		{
			PlayerStatus.RemoveLife ();
			if (!PlayerStatus.IsGameOver)
				timeUntilRespawn = 2;
			else
			{
				PlayerStatus.EndSession ();
			}
		}


	}
}
