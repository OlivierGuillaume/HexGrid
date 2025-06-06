using System;
using NUnit.Framework;
using UnityEngine;
using HexGrid;
using System.Collections.Generic;
using System.Collections;
using Unity.Automation.Players.WebGL;
using UnityEditor;

public class HexEdgeTests
{
    static HexPosition A = new HexPosition(-1, 1);
    static HexPosition B = new HexPosition(-1, 2);
    static HexPosition C = new HexPosition(-2, 2);
    static HexPosition D = new HexPosition(0, 1);

    static HexEdge AB = new(A, B);
    static HexEdge BC = new(B, C);
    static HexEdge CA = new(C, A);

    [Test]
    public void Equalities()
    {
        HexEdge AB2 = new(A, B);
        HexEdge BA = new(B, A);

        Assert.AreEqual(AB, AB2);
        Assert.AreEqual(AB.GetHashCode(), AB2.GetHashCode());
        Assert.IsTrue(AB == AB2);
        Assert.IsFalse(AB != AB2);

        Assert.AreEqual(AB, BA);
        Assert.AreEqual(AB.GetHashCode(), BA.GetHashCode());
        Assert.IsTrue(AB == BA);
        Assert.IsFalse(AB != BA);

        Assert.AreNotEqual(AB, BC);
        Assert.AreNotEqual(AB.GetHashCode(), BC.GetHashCode());
        Assert.IsFalse(AB == BC);
        Assert.IsTrue(AB != BC);
    }

    [Test]
    public void VertexAdjacentHexes()
    {
        HexPosition[] adj = AB.GetVertexAdjacentHexes();

        HexPosition[] exp = new HexPosition[] { C, new(0,1) };

        Assert.That(adj, Is.EquivalentTo(exp));
    }

    [Test]
    public void AdjacentEdges()
    {
        HexEdge[] adj = AB.GetAdjacentEdges();

        HexEdge[] exp = new HexEdge[] { new(A, C), new(A, D), new(B, C), new(B, D) };

        Assert.That(adj, Is.EquivalentTo(exp));
    }

    [Test]
    public void Vertices()
    {
        HexVertex[] vertices = AB.GetVertices();

        HexVertex[] exp = new HexVertex[] { new(A,2), new(A, 3) };

        Assert.That(vertices, Is.EquivalentTo(exp));
    }

}
