using SimpleWargame.Map;
using SimpleWargame.Units;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.AbilitySystem
{
    public class AbilityGameObject : MonoBehaviour
    {
        private const string ANIMATOR_TRIGGER_LIGHTNING = "PlayAbilityAnimation";

        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool isAwaitingForAnimations;

        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (animator == null) { Debug.LogError("AbilityGameObject Error: There is no animator"); return; }
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public async Task PlayAnimationAndDestroySelf()
        {
            //play lightning animation
            animator.SetTrigger(ANIMATOR_TRIGGER_LIGHTNING);

            isAwaitingForAnimations = true;

            while (isAwaitingForAnimations) await Task.Yield();

            //destroy self
            Destroy(gameObject);
        }

        public void AbilityAnimationFinished()
        {
            isAwaitingForAnimations = false;
        }
    }
}
