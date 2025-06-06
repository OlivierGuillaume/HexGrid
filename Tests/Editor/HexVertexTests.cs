using System;
using NUnit.Framework;
using UnityEngine;
using HexGrid;
using System.Collections.Generic;
using System.Collections;

public class HexVertexTests
{
    HexPosition A = new HexPosition(-1,1);
    HexPosition B = new HexPosition(-1,2);
    HexPosition C = new HexPosition(-2,2);
    HexPosition D = new HexPosition(-2,1);

    [Test]
    public void Equalities()
    {
        HexVertex a = new HexVertex(A, 3);
        HexVertex b = new HexVertex(A, 3);
        HexVertex c = new HexVertex(A, 4);
        HexVertex d = new HexVertex(B, 3);

        Assert.AreEqual(a, b);
        Assert.IsTrue(a == b);
        Assert.IsFalse(a != b);

        Assert.AreNotEqual(a, c);
        Assert.IsFalse(a == c);
        Assert.IsTrue(a != c);

        Assert.AreNotEqual(a, d);
        Assert.IsFalse(a == d);
        Assert.IsTrue(a != d);
    }

    [Test]
    public void SameVertexFromDifferentHexes()
    {
        HexVertex a = new HexVertex(A, 3);
        HexVertex b = new HexVertex(B, 5);
        HexVertex c = new HexVertex(C, 1);

        Assert.AreEqual(a, b);
        Assert.AreEqual(a, c);
        Assert.AreEqual(b, c);

    }

    [Test]
    public void HexToWorld()
    {
        HexVertex a = new HexVertex(A, 3);

        Assert.AreEqual(-0.5f*MathF.Sqrt(3f), a.WorldPosition.x, 0.0001f);
        Assert.AreEqual(0f, a.WorldPosition.y, 0.0001f);
        Assert.AreEqual(-2.5f, a.WorldPosition.z, 0.0001f);

        a = new HexVertex(B, 2);

        Assert.AreEqual(0.5f * MathF.Sqrt(3f), a.WorldPosition.x, 0.0001f);
        Assert.AreEqual(0f, a.WorldPosition.y, 0.0001f);
        Assert.AreEqual(-3.5f, a.WorldPosition.z, 0.0001f);
    }

    [Test]
    public void WorldToHex()
    {
        Test(A);
        Test(B);
        Test(C);
        Test(D);

        void Test(HexPosition hex)
        {
            HexVertex v0 = new HexVertex(hex, 0);
            HexVertex v1 = new HexVertex(hex, 1);

            Vector3 h0wp = v0.WorldPosition;
            Vector3 h1wp = v1.WorldPosition;

            HexVertex v0bis = HexVertex.GetNearestTo(h0wp);
            HexVertex v1bis = HexVertex.GetNearestTo(h1wp);

            Assert.AreEqual(v0, v0bis);
            Assert.AreEqual(v1, v1bis);
        }
    }

    [Test]
    public void AdjacentHexes()
    {
        HexVertex a = new HexVertex(A, 3);
        var adj = a.GetAdjacentHexes();

        Assert.That(adj, Is.EquivalentTo(new HexPosition[] { A, B, C}));

        a = new HexVertex(A, 4);
        adj = a.GetAdjacentHexes();

        Assert.That(adj, Is.EquivalentTo(new HexPosition[] { A, C, D }));

    }

    [Test]
    public void AdjacentEdges()
    {
        HexVertex a = new HexVertex(A, 3);
        HexEdge[] adj = a.GetAdjacentEdges();

        var expected = new HexEdge[] { new HexEdge(A, B), new HexEdge(B, C), new HexEdge(C, A) };

        Assert.That(adj, Is.EquivalentTo(expected));

        a = new HexVertex(A, 4);
        adj = a.GetAdjacentEdges();

        Assert.That(adj, Is.EquivalentTo(new HexEdge[] { new HexEdge(A, C), new HexEdge(C, D), new HexEdge(D, A) }));

    }

    [Test]
    public void AdjacentVertices()
    {
        HexVertex a = new HexVertex(A, 3);

        HexVertex[] adj = a.GetAdjacentVertices();
        var expected = new HexVertex[] { new HexVertex(A, 2), new HexVertex(A, 4), new HexVertex(C, 2) };
        Assert.That(adj, Is.EquivalentTo(expected));


        a = new HexVertex(A, 2);

        adj = a.GetAdjacentVertices();
        expected = new HexVertex[] { new HexVertex(A, 1), new HexVertex(A, 3), new HexVertex(B, 1) };
        Assert.That(adj, Is.EquivalentTo(expected));

    }
}
