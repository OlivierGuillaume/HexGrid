using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HexGrid;

public class HexPositionTests
{
    HexPosition O = new HexPosition(0, 0);
    HexPosition A = new HexPosition(2, -2);
    HexPosition B = new HexPosition(0, 2);
    HexPosition C = new HexPosition(-3, 2);

    Vector2Int Oo = new Vector2Int(0, 0);
    Vector2Int Ao = new Vector2Int(1, -2);
    Vector2Int Bo = new Vector2Int(1, 2);
    Vector2Int Co = new Vector2Int(-2, 2);

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
        Assert.AreEqual(O, new HexPosition(O.WorldPosition));
        Assert.AreEqual(A, new HexPosition(A.WorldPosition));
        Assert.AreEqual(B, new HexPosition(B.WorldPosition));
        Assert.AreEqual(C, new HexPosition(C.WorldPosition));

        Assert.AreEqual(O, new HexPosition(O.WorldPosition + new Vector3(-0.4f,0f,0.3f)));
        Assert.AreEqual(A, new HexPosition(A.WorldPosition + new Vector3(0.25f, 0f, -0.3f)));
        Assert.AreEqual(B, new HexPosition(B.WorldPosition + new Vector3(0.4f, 0f, 0.4f)));
        Assert.AreEqual(C, new HexPosition(C.WorldPosition + new Vector3(-0.4f, 0f, -0.3f)));
    }

    [Test]
    public void Neighbors()
    {
        HexPosition[] An = A.GetNeighbors();
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
}
