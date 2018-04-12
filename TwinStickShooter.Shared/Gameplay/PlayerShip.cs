
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

			MakeExhaustFire ();
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
				timeUntilRespawn = 3;
			else
			{
				PlayerStatus.EndSession ();
			}

			// Particles
			Color yellow = new Color (0.8f, 0.8f, 0.4f);
			for (int i = 0; i < 1200; i++)
			{
				float speed = 700 * (1f - 1 / rand.NextFloat (1f, 10f));
				Color color = Color.Lerp (Color.White, yellow, rand.NextFloat (0, 1));
				var state = new ParticleState ()
				{
					velocity = rand.NextVector2 (speed, speed),
					type = ParticleType.None,
					lengthMultiplier = 1
				};

				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, position, color, 3, 1.5f, state);
			}
		}

		private void MakeExhaustFire()
		{
			if (velocity.LengthSquared () > 0.1f)
			{
				// set up some variables
				//orientation = velocity.ToAngle ();
				Quaternion rot = Quaternion.CreateFromYawPitchRoll (0f, 0f, orientation);

				double t = GameRoot.GameTime.TotalGameTime.TotalSeconds;
				// The primary velocity of the particles is 3 pixels/frame in the direction opposite to which the ship is travelling.
				Vector2 baseVel = velocity.ScaleTo (-100);
				// Calculate the sideways velocity for the two side streams. The direction is perpendicular to the ship's velocity and the
				// magnitude varies sinusoidally.
				Vector2 perpVel = new Vector2 (baseVel.Y, -baseVel.X) * (0.6f * (float) Math.Sin (t * 10));
				Color sideColor = new Color (200, 38, 9);    // deep red
				Color midColor = new Color (255, 187, 30);   // orange-yellow
				Vector2 pos = position + Vector2.Transform (new Vector2 (-25, 0), rot);   // position of the ship's exhaust pipe.
				const float alpha = 0.7f;

				// middle particle stream
				Vector2 velMid = baseVel + rand.NextVector2 (0, 1);
				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, pos, Color.White * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (velMid, ParticleType.Enemy));
				GameRoot.ParticleManager.CreateParticle (Art.Glow, pos, midColor * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (velMid, ParticleType.Enemy));

				// side particle streams
				Vector2 vel1 = baseVel + perpVel + rand.NextVector2 (0, 0.3f);
				Vector2 vel2 = baseVel - perpVel + rand.NextVector2 (0, 0.3f);
				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, pos, Color.White * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (vel1, ParticleType.Enemy));
				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, pos, Color.White * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (vel2, ParticleType.Enemy));

				GameRoot.ParticleManager.CreateParticle (Art.Glow, pos, sideColor * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (vel1, ParticleType.Enemy));
				GameRoot.ParticleManager.CreateParticle (Art.Glow, pos, sideColor * alpha, 0.7f, new Vector2 (0.5f, 0.75f),
					new ParticleState (vel2, ParticleType.Enemy));
			}
		}
	}
}
