using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace UI
{
    public class MenuEventSystemHandler : MonoBehaviour
    {
        [Header("References")]
        public List<Selectable> Selectables = new List<Selectable>();
        [SerializeField] protected Selectable _firstSelected;

        [Header("Animations")]
        [SerializeField] protected float _selectedAnimationScale = 1.1f;
        [SerializeField] protected float _scaleDuration = 0.25f;
        [SerializeField] protected List<GameObject> _animationExclusions = new List<GameObject>();

        protected Selectable _lastSelected;
        protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

        private void Awake()
        {
            foreach (Selectable selectable in Selectables)
            {
                if (selectable == null) continue;
                AddSelectionListeners(selectable);
                if (!_scales.ContainsKey(selectable))
                    _scales.Add(selectable, selectable.transform.localScale);
            }
        }

        private void OnEnable()
        {
            foreach (Selectable sel in Selectables)
            {
                if (sel != null && _scales.ContainsKey(sel))
                    sel.transform.localScale = _scales[sel];
            }

            StartCoroutine(SelectAfterDelay());
        }

        protected IEnumerator SelectAfterDelay()
        {
            yield return null;

            if (_firstSelected != null && _firstSelected.gameObject != null && _firstSelected.gameObject.activeInHierarchy)
            {
                EventSystem.current?.SetSelectedGameObject(_firstSelected.gameObject);
            }
            else
            {
                Debug.LogWarning("MenuEventSystemHandler: _firstSelected is not assigned or inactive!");
            }
        }

        protected virtual void AddSelectionListeners(Selectable selected)
        {
            if (selected == null) return;

            EventTrigger trigger = selected.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = selected.gameObject.AddComponent<EventTrigger>();

            AddEventTrigger(trigger, EventTriggerType.Select, OnSelect);
            AddEventTrigger(trigger, EventTriggerType.Deselect, OnDeselect);
            AddEventTrigger(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
            AddEventTrigger(trigger, EventTriggerType.PointerExit, OnPointerExit);
        }

        private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(action);
            trigger.triggers.Add(entry);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject == null) return;

            _lastSelected = eventData.selectedObject.GetComponent<Selectable>();

            if (_animationExclusions.Contains(eventData.selectedObject)) return;

            Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
            StartCoroutine(eventData.selectedObject.transform.ScaleTo(newScale, _scaleDuration));
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject == null) return;

            var sel = eventData.selectedObject.GetComponent<Selectable>();
            if (sel != null && _scales.ContainsKey(sel))
                StartCoroutine(eventData.selectedObject.transform.ScaleTo(_scales[sel], _scaleDuration));
        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerEventData)
                pointerEventData.selectedObject = pointerEventData.pointerEnter;
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            if (eventData is PointerEventData pointerEventData)
                pointerEventData.selectedObject = null;
        }

        public virtual void OnNavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current == null) return;

            if (EventSystem.current.currentSelectedGameObject == null && _lastSelected != null)
                EventSystem.current.SetSelectedGameObject(_lastSelected.gameObject);
        }
    }

    public static class TransformTween
    {
        public static IEnumerator ScaleTo(this Transform transform, Vector3 targetScale, float duration, AnimationCurve curve = null)
        {
            if (transform == null) yield break;

            Vector3 startScale = transform.localScale;
            float time = 0f;
            curve ??= AnimationCurve.EaseInOut(0, 0, 1, 1);

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / duration);
                transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, t);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        public static IEnumerator PunchScale(this Transform transform, float punchAmount = 0.2f, float duration = 0.3f)
        {
            if (transform == null) yield break;

            Vector3 startScale = transform.localScale;
            Vector3 maxScale = startScale * (1 + punchAmount);
            float halfDuration = duration / 2f;

            yield return transform.ScaleTo(maxScale, halfDuration);
            yield return transform.ScaleTo(startScale, halfDuration);
        }
    }
}
