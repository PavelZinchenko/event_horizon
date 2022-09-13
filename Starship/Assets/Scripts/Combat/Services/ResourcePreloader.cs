using System.Linq;
using Combat.Domain;
using GameDatabase;
using Services.ObjectPool;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Combat.Services
{
    public class ResourcePreloader : MonoBehaviour
    {
        [Inject]
        private void Initialize(IObjectPool objectPool, IDatabase database, PrefabCache prefabCache, IResourceLocator resourceLocator, ICombatModel combatModel)
        {
            UnityEngine.Debug.Log("Preloading objects");

            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/DamageText"), 5);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/Flash"), 10);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/FlashAdditive"), 10);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/OrbAdditive"), 10);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/Smoke"), 10);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/SmokeAdditive"), 10);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/Wave"), 5);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/WaveThin"), 5);
            objectPool.PreloadObjects(prefabCache.LoadResourcePrefab("Combat/Effects/Wreck"), 5);

            foreach (var ship in combatModel.PlayerFleet.Ships.Take(12).Concat(combatModel.EnemyFleet.Ships.Take(12)))
            {
                foreach (var weapon in ship.ShipData.Components.Where(component => component.Info.Data.Weapon != null))
                {
                    var component = weapon.Info.CreateComponent(ship.ShipData.Model.Layout.CellCount);

                    foreach (var spec in component.Weapons)
                    {
                        // TODO: preload resources
                    }

                    foreach (var spec in component.WeaponsObsolete)
                    {
                        if (spec.Value.BulletPrefab)
                            objectPool.PreloadObjects(prefabCache.LoadPrefab(spec.Value.BulletPrefab), 3);
                        if (spec.Value.FireSound)
                            resourceLocator.GetAudioClip(spec.Value.FireSound)?.LoadAudioData();
                        if (spec.Value.HitSound)
                            resourceLocator.GetAudioClip(spec.Value.HitSound)?.LoadAudioData();
                        if (spec.Key.ChargeSound)
                            resourceLocator.GetAudioClip(spec.Key.ChargeSound)?.LoadAudioData();
                    }
                }
            }
            
            // Debug.Break();
        }
    }
}
