using UnityEngine;
using System.Collections;

namespace GameModel
{
	public class Config : MonoBehaviour
	{
		public AudioClip ShipExplosionSound;
		public AudioClip SmallShipExplosionSound;
		public AudioClip ShipWarpSound;
		public AudioClip ShipCollisionSound;
		public AudioClip BuyItemSound;
		public Material DefaultMaterial;
		public int CollisionMapSize = 8;
		public GameObject AsteroidPrefab;
		public GameObject PlanetPrefab;
		public GameObject AtmospherePrefab;
		public AudioClip AsteroidCollisionSound;
		public AudioClip ShipRetreatSound;
		public AudioClip RepairSound;
	}
}
