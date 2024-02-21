using SimpleWargame.BattleManagement;
using SimpleWargame.EffectSystem;
using SimpleWargame.Map;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimpleWargame.AbilitySystem
{
    public class ThunderstormGameobject : MonoBehaviour, IDragHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject lightningGameObject;
        [SerializeField] private CanvasGroup canvasGroup;

        private List<TileData> tiles = new List<TileData>();
        private bool isSpawnConfirmed;
        private List<Vector3> offsets = new List<Vector3>();

        private List<Effect> effects = new List<Effect>();

        private int lightningChanceToSpawn;
        private int lightningChanceToHit;
        private int lightningNumberOfTargets;
        private int lightningDamage;
        private int thunderstormDuration;

        private Sprite sprite;

        private void Start()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) { Debug.LogError("ThunderstormGameobject Error: there is no CanvasGroup"); return; }

            if (MapManager.Instance == null) { Debug.LogError("ThunderstormGameobject Error: there is no MapManager"); return; }
            if (BattleManager.Instance == null) { Debug.LogError("ThunderstormGameobject Error: there is no BattleManager"); return; }


            offsets.Add(new Vector3(-1, 1, 0));
            offsets.Add(new Vector3(0, 1, 0));
            offsets.Add(new Vector3(1, 1, 0));
            offsets.Add(new Vector3(-1, 0, 0));
            offsets.Add(new Vector3(0, 0, 0));
            offsets.Add(new Vector3(1, 0, 0));
            offsets.Add(new Vector3(-1, -1, 0));
            offsets.Add(new Vector3(0, -1, 0));
            offsets.Add(new Vector3(1, -1, 0));

            MapManager.Instance.OnSendClickedTile += MapManager_OnSendClickedTile;
            BattleManager.Instance.OnStartNewTurn += BattleManager_OnStartNewTurn;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.pointerCurrentRaycast.worldPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
            MapManager.Instance.OnSendClickedTile -= MapManager_OnSendClickedTile;
            isSpawnConfirmed = true;
        }

        public async Task SpawnConfirmed()
        {
            while (!isSpawnConfirmed)
            {
                await Task.Yield();
            }

            foreach (Vector3 position in offsets)
            {
                Vector3 positionWithOffset = position + transform.position;

                MapManager.Instance.MapTiles.TryGetValue(Vector3Int.CeilToInt(positionWithOffset), out TileData tile);

                if (tile == null) continue;

                if (effects == null) continue;

                foreach (Effect effect in effects)
                {
                    tile.AddEffect(effect);
                }

                tiles.Add(tile);
            }

            await CreateLightnings();
        }

        public void SetThunderstormProperties
            (int thunderstormDuration, Sprite sprite,
            int lightningChanceToSpawn, int lightningChanceToHit, int lightningNumberOfTargets, int lightningDamage)
        {
            this.sprite = sprite;
            this.thunderstormDuration = thunderstormDuration;
            this.lightningChanceToSpawn = lightningChanceToSpawn;
            this.lightningChanceToHit = lightningChanceToHit;
            this.lightningNumberOfTargets = lightningNumberOfTargets;
            this.lightningDamage = lightningDamage;
        }

        public void AddEffect(Effect effect)
        {
            effects.Add(effect);
        }

        private async void BattleManager_OnStartNewTurn(object sender, System.EventArgs e)
        {
            await CreateLightnings();

            thunderstormDuration--;

            if (thunderstormDuration <= 0)
            {
                foreach (TileData tile in tiles)
                {
                    if (effects == null) continue;

                    foreach (Effect effect in effects)
                    {
                        tile.RemoveEffect(effect);
                    }
                }

                BattleManager.Instance.OnStartNewTurn -= BattleManager_OnStartNewTurn;

                Destroy(gameObject);
            }
        }

        private void MapManager_OnSendClickedTile(object sender, MapManager.OnSendClickedTileEventArgs e)
        {
            transform.position = e.ClickedTile.CenterPosition;
        }

        private async Task CreateLightnings()
        {
            //at each tile position spawn lightning with some chance

            List<Task> tasks = new List<Task>();

            int chance = 0;
            AbilityGameObject lightning;
            foreach (TileData tile in tiles)
            {
                chance = Random.Range(1, 100);

                if (chance > lightningChanceToSpawn) continue;

                //spawn Lightning
                GameObject gameObject = Instantiate(lightningGameObject, transform);
                lightning = gameObject.GetComponent<AbilityGameObject>();
                lightning.transform.position = tile.CenterPosition;
                lightning.SetSprite(sprite);

                tasks.Add(PlayLigthningAnimationAndDealDamage(lightning, tile));
            }

            await Task.WhenAll(tasks);
        }

        public async Task PlayLigthningAnimationAndDealDamage(AbilityGameObject lightning, TileData tile)
        {
            await lightning.PlayAnimationAndDestroySelf();

            if (tile.StandingUnit != null)
            {
                await tile.StandingUnit.TakeDamage(lightningDamage, lightningNumberOfTargets, lightningChanceToHit);
            }
        }
    }
}
