using UnityEngine;
using RecycleFactory.Buildings;
using RecycleFactory.Buildings.Logistics;
using RecycleFactory.UI;

namespace RecycleFactory
{
    public class BuildingArrowPreviewController : MonoBehaviour
    {
        [SerializeField] private BuildingArrowPreview prefab;
        [SerializeField] private float scale = 0.2f;

        private Pool<BuildingArrowPreview> pool = new Pool<BuildingArrowPreview>((BuildingArrowPreview o) => o.gameObject.activeSelf);

        public void Init()
        {
            pool.createFunc = () =>
            {
                var i = Instantiate(prefab);
                i.transform.localScale = Vector3.one * scale;
                return i;
            };
        }

        public void Display(Transform building, Building buildingPrefab, int rotation)
        {
            pool.objects.ForEach(o => o.gameObject.SetActive(false));
            if (buildingPrefab.TryGetComponent<BuildingExtension_Releaser>(out BuildingExtension_Releaser releaser))
            {
                foreach (ConveyorBelt_Anchor anchor in releaser.outAnchors)
                {
                    var newAnchor = anchor.GetRevolved(rotation);
                    var arrow = pool.TakeInactiveOrCreate();
                    arrow.gameObject.SetActive(true);
                    arrow.transform.position = building.transform.position + newAnchor.localTilePosition.ConvertTo2D().ProjectTo3D() + newAnchor.direction.ConvertTo2D().ProjectTo3D();
                    arrow.transform.eulerAngles = arrow.transform.eulerAngles.WithY(Utils.dir2rot[newAnchor.direction] * 90);
                }

            }

            if (buildingPrefab.TryGetComponent<BuildingExtension_Receiver>(out BuildingExtension_Receiver receiver))
            {
                foreach (ConveyorBelt_Anchor anchor in receiver.inAnchors)
                {
                    var newAnchor = anchor.GetRevolved(rotation);
                    var arrow = pool.TakeInactiveOrCreate();
                    arrow.gameObject.SetActive(true);
                    arrow.transform.position = building.transform.position + newAnchor.localTilePosition.ConvertTo2D().ProjectTo3D() - newAnchor.direction.ConvertTo2D().ProjectTo3D();
                    arrow.transform.eulerAngles = arrow.transform.eulerAngles.WithY(Utils.dir2rot[newAnchor.direction] * 90);
                }

            }
        }

        public void HideAll()
        {
            pool.objects.ForEach(o => o.gameObject.SetActive(false));
        }
    }
}
