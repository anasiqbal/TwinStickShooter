using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TwinStickShooter
{
	class Enemy : Entity
	{
		static Random rand = new Random ();

		const float inactiveTime = 1;
		float timeUntilStart = inactiveTime;	// time in seconds before enemy can start moving
		public bool IsActive { get { return timeUntilStart <= 0; } }

		public int PointValue { get; private set; }		// points to be awarded when the enemy is destroyed

		private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>> ();

		public Enemy(Texture2D image, Vector2 position, int score)
		{
			this.image = image;
			this.position = position;
			PointValue = score;
			radius = image.Width / 2;

			// setting to transparent so that enemies will fade in on start
			color = Color.Transparent;
		}

		public override void Update(GameTime gameTime)
		{
			// If enemy wait time to start is over the start enemy behaviour
			// otherwise wait for time to complete and fade in the enemy
			if(timeUntilStart <= 0)
			{
				ApplyBehaviour ();
			}
			else
			{
				timeUntilStart -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				color = Color.White * (1 - timeUntilStart / inactiveTime);
			}

			// update enemy position and keep the enemy within the screen bounds
			position += velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
			position = Vector2.Clamp (position, Size / 2, GameRoot.ScreenSize - Size / 2);

			// this kind of applies friction
			velocity *= 0.8f;
		}

		/// <summary>
		/// Kill the enemy
		/// </summary>
		public void WasShot()
		{
			// applying variation to pitch so that the explosion sound is a bit different every time
			Sound.Explosion.Play (0.5f, rand.NextFloat (-0.2f, 0.2f), 0);
			isExpired = true;

			float hue1 = rand.NextFloat (0, 6);
			float hue2 = (hue1 + rand.NextFloat (0, 2)) % 6f;
			Color color1 = ColorUtil.HSVToColor (hue1, 0.5f, 1);
			Color color2 = ColorUtil.HSVToColor (hue2, 0.5f, 1);

			for (int i = 0; i < 120; i++)
			{
				// 10 step speed variation
				float speed = 400f * (1f - 1 / rand.NextFloat (1f, 10f));
				var state = new ParticleState ()
				{
					velocity = rand.NextVector2 (speed, speed),
					type = ParticleType.Enemy,
					lengthMultiplier = 1
				};

				Color color = Color.Lerp (color1, color2, rand.NextFloat (0, 1));
				GameRoot.ParticleManager.CreateParticle (Art.LineParticle, position, color, 2f, 1.5f, state);
			}
		}

		/// <summary>
		/// Handles collision with other enemies
		/// </summary>
		/// <param name="other">Reference to the second enemy this enemy is colliding with</param>
		public void HandleCollision(Enemy other)
		{
			// find the direction of collision
			// and push the enemies away from eachother 
			// with force in the direction 'd' (of course in opposite directions)
			var d = position - other.position;

			// the lesser the distance the more force is applied
			// force is reduced as the distance increases
			velocity += 20000 * d / (d.LengthSquared () + 1);
		}

		/// <summary>
		/// Method for adding behaviour to the enemy
		/// </summary>
		void AddBehaviour(IEnumerable<int> behaviour)
		{
			behaviours.Add (behaviour.GetEnumerator ());
		}

		void ApplyBehaviour()
		{
			// iterate over all the behaviours and run them this frame
			for(int i = 0; i < behaviours.Count; i++)
			{
				// MoveNext will return false if the behaviour has completed its opertion
				// therefore, remove this behaviour and save unnecessary calls
				if (!behaviours [i].MoveNext ())
					behaviours.RemoveAt (i);
			}
		}
		

		#region Create Enemies

		/// <summary>
		/// A factory method for creating seeker type enemies
		/// </summary>
		/// <param name="position">position at which the enemy is spawned</param>
		/// <param name="gameTime">frame delta time</param>
		/// <returns>the created seeker type enemy</returns>
		public static Enemy CreateSeeker(Vector2 position, GameTime gameTime)
		{
			var enemy = new Enemy (Art.Seeker, position, 2);
			enemy.AddBehaviour (enemy.FollowPlayer (100, gameTime));

			return enemy;
		}

		/// <summary>
		/// A factory method for creating wanderer type enemies
		/// </summary>
		/// <param name="position">position at which the enemy is spawned</param>
		/// <param name="gameTime">frame delta time</param>
		/// <returns>the created wanderer type enemy</returns>
		public static Enemy CreateWanderer(Vector2 position, GameTime gameTime)
		{
			var enemy = new Enemy (Art.Wanderer, position, 1);
			enemy.AddBehaviour (enemy.MoveRandomly (60, gameTime));
			return enemy;
		}

		#endregion

		#region Enemy Behaviours
		/// <summary>
		/// Follow the player where ever it goes
		/// </summary>
		/// <param name="acceleration">acceleration for movement</param>
		/// <param name="gameTime"></param>
		IEnumerable<int> FollowPlayer(float acceleration, GameTime gameTime)
		{
			while(true)
			{
				// get direction of player for current enemy position
				// and scale the vector to acceleration (give the vector a magnitude of acceleration)
				velocity += (PlayerShip.Instance.position - position).ScaleTo (acceleration);

				// set the enemy look direction same as anemy move direction
				if (velocity != Vector2.Zero)
					orientation = velocity.ToAngle ();

				yield return 0;
			}
		}

		/// <summary>
		/// Move the enemy in square
		/// </summary>
		/// <param name="gameTime"></param>
		IEnumerable<int> MoveInSquare(GameTime gameTime)
		{
			//const int framePerSide = 30;
			const float durationPerSide = 0.5f;
			float elapsedTime = 0;

			while(true)
			{
				// move right
				while(elapsedTime < durationPerSide)
				{
					elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
					velocity = Vector2.UnitX;
					yield return 0;
				}

				elapsedTime = 0;
				while (elapsedTime < durationPerSide)
				{
					elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
					velocity = Vector2.UnitX;
					yield return 0;
				}

				// move down
				elapsedTime = 0;
				while (elapsedTime < durationPerSide)
				{
					elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
					velocity = Vector2.UnitY;
					yield return 0;
				}

				// move left
				elapsedTime = 0;
				while (elapsedTime < durationPerSide)
				{
					elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
					velocity = -Vector2.UnitX;
					yield return 0;
				}

				// move up;
				elapsedTime = 0;
				while (elapsedTime < durationPerSide)
				{
					elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
					velocity = -Vector2.UnitY;
					yield return 0;
				}
			}
		}

		/// <summary>
		/// Move the enemy in random direction
		/// </summary>
		/// <param name="speed">Speed to move with</param>
		/// <param name="gameTime"></param>
		IEnumerable<int> MoveRandomly(float speed, GameTime gameTime)
		{
			float direction = rand.NextFloat (0, MathHelper.TwoPi);

			while (true)
			{
				// set the direction once after spawn and then update every 0.1 seconds
				direction += rand.NextFloat (-0.1f, 0.1f);
				direction = MathHelper.WrapAngle (direction);

				float loopTime = 0.1f;
				while(loopTime > 0)
				{
					velocity += MathUtil.FromPolar (direction, speed);

					// update orientation every frame, visually the enemy is rotating every frame
					orientation -= 10f * (float) gameTime.ElapsedGameTime.TotalSeconds;

					var bounds = GameRoot.Viewport.Bounds;
					bounds.Inflate (-image.Width, -image.Height);

					// if the enemy is outside the bounds, make it move away from the edge
					if (!bounds.Contains (position.ToPoint ()))
						direction = (GameRoot.ScreenSize / 2 - position).ToAngle () + rand.NextFloat (-MathHelper.PiOver2, MathHelper.PiOver2);

					loopTime -= (float) gameTime.ElapsedGameTime.TotalSeconds;
					yield return 0;
				}
			}
		}

		#endregion
	}
}
