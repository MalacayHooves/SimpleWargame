using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleWargame
{
    public class UnitVisuals : MonoBehaviour
    {
        private const string ANIMATOR_TRIGGER_ATTACK = "Attack";
        private const string ANIMATOR_TRIGGER_TAKE_DAMAGE = "TakeDamage";
        private const string ANIMATOR_TRIGGER_DEFENSE = "Defense";

        public static bool IsAwaitingForAnimations { get; private set; }

        [Header("Links to Objects")]
        [SerializeField] private SpriteRenderer unitSpriteRenderer;
        [SerializeField] private SpriteRenderer playerColorSprite;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform attackVisual;
        [SerializeField] private GameObject activitySprite;
        [SerializeField] private GameObject selectedSprite;

        private Unit unit;

        private void Start()
        {
            if (unitSpriteRenderer == null) { Debug.LogError("UnitVisuals: There is no UnitSpriteRenderer"); return; }
            if (animator == null) { Debug.LogError("UnitVisuals: There is no animator"); return; }
            if (attackVisual == null) { Debug.LogError("UnitVisuals: There is no attackVisual gameobject"); return; }
            if (activitySprite == null) { Debug.LogError("UnitVisuals: There is no activitySprite gameobject"); return; }
            if (activitySprite == null) { Debug.LogError("UnitVisuals: There is no selectedSprite gameobject"); return; }

            selectedSprite.SetActive(false);
        }

        public void SetUnit(Unit newUnit)
        {
            unit = newUnit;
            unitSpriteRenderer.sprite = unit.UnitSprite;
        }

        public void SetPlayerColor(Color color)
        {
            playerColorSprite.color = color;
        }

        public async Task PlayAttackAnimation(Vector3 enemyDirection)
        {
            enemyDirection.z = transform.position.z;

            Quaternion rotation = Quaternion.LookRotation(enemyDirection - transform.position, Vector3.right);
            attackVisual.transform.rotation = rotation;

            animator.SetTrigger(ANIMATOR_TRIGGER_ATTACK);

            IsAwaitingForAnimations = true;

            while (IsAwaitingForAnimations) { await Task.Yield(); }
        }

        public async Task PlayTakeDamageAnimation()
        {
            animator.SetTrigger(ANIMATOR_TRIGGER_TAKE_DAMAGE);

            IsAwaitingForAnimations = true;

            while (IsAwaitingForAnimations) { await Task.Yield(); }
        }

        public async Task PlayDefenseAnimation()
        {
            animator.SetTrigger(ANIMATOR_TRIGGER_DEFENSE);

            IsAwaitingForAnimations = true;

            while (IsAwaitingForAnimations) { await Task.Yield(); }
        }

        public async Task PlayDeathAnimation()
        {
            print("Death animation isn't implemented yet");

            while (IsAwaitingForAnimations) { await Task.Yield(); }
        }

        public void SetUnitActivitySpriteState(bool isActive) { activitySprite.SetActive(!isActive); }   //"!" because we need to hide sprite for active unit

        public void SetUnitSelectedSpriteState(bool isSelected) { selectedSprite.SetActive(isSelected); }

        public void SetUnitHalfTransparent(bool isHalfTransparent)
        {
            if (isHalfTransparent)
            {
                Color color = unitSpriteRenderer.color;
                color.a = .5f;
                unitSpriteRenderer.color = color;
                color = playerColorSprite.color;
                color.a = .5f;
                playerColorSprite.color = color;
            }
            else
            {
                Color color = unitSpriteRenderer.color;
                color.a = 1f;
                unitSpriteRenderer.color = color;
                color = playerColorSprite.color;
                color.a = 1f;
                playerColorSprite.color = color;
            }
        }

        private void AttackFinished()
        {
            IsAwaitingForAnimations = false;
        }

        private void TakeDamageFinished()
        {
            IsAwaitingForAnimations = false;
        }

        private void DefenseFinished()
        {
            IsAwaitingForAnimations = false;
        }
    }
}
