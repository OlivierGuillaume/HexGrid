using System;
using NUnit.Framework;
using UnityEngine;
using HexGrid;
using System.Collections.Generic;

public class HexPositionTests
{
    HexPosition O = new HexPosition(0, 0);
    HexPosition A = new HexPosition(2, -2);
    HexPosition Abis = new HexPosition(2, -2);
    HexPosition B = new HexPosition(0, 2);
    HexPosition C = new HexPosition(-3, 2);
    HexPosition D = new HexPosition(0, 1);

    Vector2Int Oo = new Vector2Int(0, 0);
    Vector2Int Ao = new Vector2Int(1, -2);
    Vector2Int Bo = new Vector2Int(1, 2);
    Vector2Int Co = new Vector2Int(-2, 2);

    Vector3 Ow = new Vector3(0f, 0f, 0f);
    Vector3 Aw = new Vector3(MathF.Sqrt(3), 0f, 3f);
    Vector3 Bw = new Vector3(MathF.Sqrt(3), 0f, -3f);
    Vector3 Cw = new Vector3(-2f * MathF.Sqrt(3), 0f, -3f);

    HexPosition CRotAroundD60 = new HexPosition(-1, -1);
    HexPosition CRotAroundD120 = new HexPosition(2, -2);
    HexPosition CRotAroundD180 = new HexPosition(3, 0);
    HexPosition CRotAroundD240 = new HexPosition(1, 3);
    HexPosition CRotAroundD300 = new HexPosition(-2, 4);

    [Test]
    public void CubeToOffset()
    {
        Assert.AreEqual(Oo, O.OffsetCoordinates); 
        Assert.AreEqual(Ao, A.OffsetCoordinates);
        Assert.AreEqual(Bo, B.OffsetCoordinates);
        Assert.AreEqual(Co, C.OffsetCoordinates);
    }

    [Test]
    public void OffsetToCube()
    {
        Assert.AreEqual(O, new HexPosition(Oo, true));
        Assert.AreEqual(A, new HexPosition(Ao, true));
        Assert.AreEqual(B, new HexPosition(Bo, true));
        Assert.AreEqual(C, new HexPosition(Co, true));
    }

    [Test]
    public void WorldPosition()
    {
        Assert.AreEqual(Ow, O.WorldPosition);
        Assert.AreEqual(Aw, A.WorldPosition);
        Assert.AreEqual(Bw, B.WorldPosition);
        Assert.AreEqual(Cw, C.WorldPosition);
    }

    [Test]
    public void HexFromWorldPosition()
    {
        Assert.AreEqual(O, HexPosition.GetNearestTo(O.WorldPosition));
        Assert.AreEqual(A, HexPosition.GetNearestTo(A.WorldPosition));
        Assert.AreEqual(B, HexPosition.GetNearestTo(B.WorldPosition));
        Assert.AreEqual(C, HexPosition.GetNearestTo(C.WorldPosition));

        Assert.AreEqual(O, HexPosition.GetNearestTo(O.WorldPosition + new Vector3(-0.4f,0f,0.3f)));
        Assert.AreEqual(A, HexPosition.GetNearestTo(A.WorldPosition + new Vector3(0.25f, 0f, -0.3f)));
        Assert.AreEqual(B, HexPosition.GetNearestTo(B.WorldPosition + new Vector3(0.4f, 0f, 0.4f)));
        Assert.AreEqual(C, HexPosition.GetNearestTo(C.WorldPosition + new Vector3(-0.4f, 0f, -0.3f)));
    }

    [Test]
    public void Neighbors()
    {
        HexPosition[] An = A.GetAdjacentHexes();
        HexPosition[] AnExpected = { new HexPosition(3,-3), new HexPosition(3, -2), new HexPosition(2, -1), new HexPosition(1, -1), new HexPosition(1, -2), new HexPosition(2, -3) };
        Array.Sort(An);
        Array.Sort(AnExpected);

        CollectionAssert.AreEqual(AnExpected, An);
    }

    [Test]
    public void Distance()
    {
        Assert.AreEqual(4, A.DistanceTo(B));
        Assert.AreEqual(4, B.DistanceTo(A));
        Assert.AreEqual(3, B.DistanceTo(C));
        Assert.AreEqual(3, C.DistanceTo(B));
        Assert.AreEqual(5, A.DistanceTo(C));
        Assert.AreEqual(5, C.DistanceTo(A));
    }

    [Test]
    public void Rotation()
    {
        Assert.AreEqual(CRotAroundD60, C.Rotate60Around(D));
        Assert.AreEqual(CRotAroundD120, C.Rotate60Around(D,2));
        Assert.AreEqual(CRotAroundD180, C.Rotate60Around(D,3));
        Assert.AreEqual(CRotAroundD240, C.Rotate60Around(D,4));
        Assert.AreEqual(CRotAroundD300, C.Rotate60Around(D,5));
        Assert.AreEqual(C, C.Rotate60Around(D, 6));

        Assert.AreEqual(CRotAroundD60, C.Rotate60Around(D,7));
        Assert.AreEqual(CRotAroundD60, C.Rotate60Around(D,-5));
        Assert.AreEqual(C, C.Rotate60Around(D,0));
        Assert.AreEqual(C, C.Rotate60Around(D,-6));
        Assert.AreEqual(CRotAroundD180, C.Rotate60Around(D,3));
        Assert.AreEqual(CRotAroundD180, C.Rotate60Around(D, 9));
        Assert.AreEqual(CRotAroundD180, C.Rotate60Around(D, -3));
        Assert.AreEqual(CRotAroundD180, C.Rotate60Around(D, -9));

    }

    [Test]
    public void Comparison()
    {
        Assert.IsTrue(A == Abis);
        Assert.IsFalse(A != Abis);
        Assert.IsFalse(A == B);
        Assert.IsTrue(A != B);
    }

    [Test]
    public void Hash()
    {
        HashSet<HexPosition> set = new();

        set.Add(A);
        set.Add(Abis);

        Assert.AreEqual(1, set.Count);

        set.Clear();
        set.Add(A);
        set.Add(B);

        Assert.AreEqual(2, set.Count);

    }
}
