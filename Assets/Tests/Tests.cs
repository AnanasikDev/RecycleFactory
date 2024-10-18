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
    }
}
