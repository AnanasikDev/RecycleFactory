using DG.Tweening;
using RecycleFactory.Buildings;
using RecycleFactory.UI;
using System;
using System.Collections;
using UnityEngine;

namespace RecycleFactory.Player
{
    public class PlayerDemolisher : MonoBehaviour
    {
        internal Vector2Int selectedCell { get; private set; }

        private Func<bool> demolishTrigger = () => Input.GetMouseButtonDown(0) && !UIInputMask.isPointerOverUI;

        public event Action<Building> onDemolishEvent;
        public event Action onAnyDemolishEvent;

        public void Init()
        {
            
        }

        public void _Update()
        {
            selectedCell = Scripts.PlayerController.GetSelectedCell();
            if (demolishTrigger())
            {
                Building building = Map.getBuildingAt(selectedCell);
                if (building == null) return;
                Demolish(building);
            }
        }

        private void Demolish(Building building)
        {
            StartCoroutine(AnimateOnDemolish(building));
        }

        private IEnumerator AnimateOnDemolish(Building building)
        {
            YieldInstruction animate(Building _building)
            {
                return DOTween.Sequence().Append(_building.transform.DOScale(Vector3.zero, 0.25f)).Join(Scripts.PlayerCamera.cameraHandler.transform.DOShakePosition(0.4f, 0.25f)).Play().WaitForCompletion();
            }
            building.isAlive = false;

            yield return animate(building);
            onDemolishEvent?.Invoke(building);
            onAnyDemolishEvent?.Invoke();

            Scripts.Budget.Add(building.cost);
            building.Demolish();
        }
    }
}
