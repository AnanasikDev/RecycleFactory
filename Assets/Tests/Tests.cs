using NUnit.Framework;
using UnityEngine;
using RecycleFactory;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void UtilsTest()
        {
            Vector2Int dir = Vector2Int.up;
            Assert.That(Utils.RotateOnce(dir, 1) == Vector2Int.right);
            Assert.That(Utils.RotateOnce(dir, -1) == Vector2Int.left);
            Assert.That(Utils.Rotate(dir, 1) == Vector2Int.right);
            Assert.That(Utils.Rotate(dir, -1) == Vector2Int.left);
            Assert.That(Utils.Rotate(dir, 0) == Vector2Int.up);
            Assert.That(Utils.Rotate(dir, 4) == Vector2Int.up);
            Assert.That(Utils.Rotate(dir, 8) == Vector2Int.up);
            Assert.That(Utils.Rotate(dir, 3) == Utils.Rotate(dir, -1));
            Assert.That(Utils.Rotate(dir, 2) == Utils.Rotate(dir, -2));
        }

        [Test]
        public void ConveyorInAnchorTest()
        {
            RecycleFactory.Buildings.ConveyorAnchor anchor = new();
            anchor.direction = Vector2Int.up;
            anchor.localTilePosition = Vector2Int.right;

            Assert.That(anchor.GetRevolved(0).direction == Vector2Int.up);
            Assert.That(anchor.GetRevolved(0).localTilePosition == Vector2.right);
            Assert.That(anchor.GetRevolved(4).direction == Vector2Int.up);
            Assert.That(anchor.GetRevolved(4).localTilePosition == Vector2.right);

            Assert.That(anchor.GetRevolved(1).direction == Vector2Int.right);
            Assert.That(anchor.GetRevolved(1).localTilePosition == Vector2.down);

            Assert.That(anchor.GetRevolved(2).direction == Vector2Int.down);
            Assert.That(anchor.GetRevolved(2).localTilePosition == Vector2.left);
            Assert.That(anchor.GetRevolved(-2).direction == Vector2Int.down);
            Assert.That(anchor.GetRevolved(-2).localTilePosition == Vector2.left);

            Assert.That(anchor.GetRevolved(3).direction == Vector2Int.left);
            Assert.That(anchor.GetRevolved(3).localTilePosition == Vector2.up);
            Assert.That(anchor.GetRevolved(-1).direction == Vector2Int.left);
            Assert.That(anchor.GetRevolved(-1).localTilePosition == Vector2.up);
        }
    }
}
