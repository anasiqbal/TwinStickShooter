using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwinStickShooter
{
	/// <summary>
	/// Class for managing, updating and drawing all the entities
	/// A static class so that only one instance exists
	/// </summary>
	static class EntityManager
	{
		// a list to store all entities
		static List<Entity> entities = new List<Entity> ();

		// lists for saving certain entities
		static List<Bullet> bullets = new List<Bullet> ();
		static List<Enemy> enemies = new List<Enemy> ();
		static List<BlackHole> blackHoles = new List<BlackHole> ();
		
		static bool isUpdating;
		static List<Entity> addedEntities = new List<Entity> ();

		public static int Count { get { return entities.Count; } }
		public static int BlackHoleCount { get { return blackHoles.Count; } }

		public static void Add(Entity entity)
		{
			// if the class is updating all entities and new entities are added
			// then add them to a separate list
			if (!isUpdating)
			{
				// add all entities to one place to manage update and draw calls at one place
				entities.Add (entity);

				if (entity is Bullet)
					bullets.Add (entity as Bullet);
				else if (entity is Enemy)
					enemies.Add (entity as Enemy);
				else if (entity is BlackHole)
					blackHoles.Add (entity as BlackHole);
			}
			else
				addedEntities.Add (entity);
		}

		public static void Update(GameTime gameTime)
		{
			// kind of a lock to stop all other operations while update operations are performed
			isUpdating = true;
			HandleCollisions ();

			foreach (var entity in entities)
				entity.Update (gameTime);

			isUpdating = false;

			// once the update operation is complete
			// add the entities (ones added while the update operations were running)
			// to the main list so that there can be managed
			foreach (var entity in addedEntities)
			{
				entities.Add (entity);
				if (entity is Bullet)
					bullets.Add (entity as Bullet);
				else if (entity is Enemy)
					enemies.Add (entity as Enemy);
				else if (entity is BlackHole)
					blackHoles.Add (entity as BlackHole);
			}

			addedEntities.Clear ();

			// linq operation to remove expired entities
			entities = entities.Where (x => !x.isExpired).ToList ();
			bullets = bullets.Where (x => !x.isExpired).ToList ();
			enemies = enemies.Where (x => !x.isExpired).ToList ();
			blackHoles = blackHoles.Where (x => !x.isExpired).ToList ();
		}

		public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
		{
			// loop through all the entities and draw them
			foreach (var entity in entities)
				entity.Draw (spriteBatch, gameTime);
		}
		
		/// <summary>
		/// Check if two entities are colliding. Performs circular collision detection
		/// </summary>
		/// <param name="a">First Entity</param>
		/// <param name="b">Second Entity</param>
		/// <returns></returns>
		static bool IsColliding(Entity a, Entity b)
		{
			// sum of radius. This is the maximum distance at which to entities will be considered to be colliding
			float radius = a.radius + b.radius;

			// return true if both entities are alive (not expired)
			// and their distance is less than the sum of their radiuses
			return !a.isExpired && !b.isExpired && Vector2.DistanceSquared (a.position, b.position) < radius * radius;
		}

		static void HandleCollisions()
		{
			// check collision for every enemy with every other enemy in the list
			for(int i = 0; i < enemies.Count; i++)
			{
				// starting from i as previous enemies have already checked for collision with the enemy at i
				// therefore no need to check again. +1 because we donot want to check collisions with self
				for(int j = i + 1; j < enemies.Count; j++)
				{
					if(IsColliding(enemies[i], enemies[j]))
					{
						enemies [i].HandleCollision (enemies [j]);
						enemies [j].HandleCollision (enemies [i]);
					}
				}
			}

			// check collision for every bullet with every enemy
			for (int i = 0; i < enemies.Count; i++)
			{
				for (int j = 0; j < bullets.Count; j++)
				{
					if (IsColliding (enemies [i], bullets [j]))
					{
						enemies [i].WasShot ();
						PlayerStatus.AddPoints (enemies [i].PointValue);
						PlayerStatus.IncreaseMultiplier ();
						bullets [j].isExpired = true;
					}
				}
			}

			// check collision of every enemy with the player
			for (int i = 0; i < enemies.Count; i++)
			{
				if (enemies[i].IsActive && IsColliding (enemies [i], PlayerShip.Instance))
				{
					KillPlayer ();
					break;
				}
			}

			// check collision of all the blackholes with enemies, bullets and player
			// bullets cause damage to the blackhole while player and enemies are killed
			for (int i = 0; i < blackHoles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					if (enemies [j].IsActive && IsColliding (blackHoles [i], enemies[j]))
					{
						enemies [j].WasShot ();
					}
				}

				for(int j = 0; j < bullets.Count; j++)
				{
					if(IsColliding(blackHoles[i], bullets[j]))
					{
						bullets [j].isExpired = true;
						blackHoles [i].WasShot ();
					}
				}

				if(IsColliding(PlayerShip.Instance, blackHoles[i]))
				{
					KillPlayer ();
					break;
				}

			}
		}

		/// <summary>
		/// A helper method to kill player, destroy all enemies and reset the game state
		/// </summary>
		static void KillPlayer()
		{
			PlayerShip.Instance.Kill ();

			enemies.ForEach (x => x.WasShot ());
			blackHoles.ForEach (x => x.Kill ());

			EnemySpawner.Reset ();
		}

		/// <summary>
		/// Get list of all entities around a position in a certain radius
		/// </summary>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static IEnumerable GetNearbyEntities(Vector2 position, float radius)
		{
			return entities.Where (x => Vector2.DistanceSquared (position, x.position) < radius * radius);
		}
	}
}
