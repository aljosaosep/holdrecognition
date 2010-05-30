/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */



package Diplomska.Protipi;

import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author aljosa
 */
public class Poly {
    public List<Vec3> vertices;
    public List<Triangle> tris;
    
    public Color3 color;

    public Poly()
    {
        this.vertices = new ArrayList<Vec3>();
        this.tris = new ArrayList<Triangle>();
        this.color = new Color3();
        
    }

    public Poly(List<Triangle> tris)
    {
        //this.vertices = vert;
        this.tris = tris;
        this.color = new Color3();
    }

    public void addVertex(Vec3 vertex)
    {
        this.vertices.add(vertex);
    }

    public Vec3 getVertex(int i)
    {
        return this.vertices.get(i);
    }

    public void addTri(Triangle tri)
    {
        this.tris.add(tri);
    }

    public Triangle getTri(int i)
    {
        return this.tris.get(i);
    }

    public void setColor(float r, float g, float b)
    {
        this.color.r = r;
        this.color.g = g;
        this.color.b = b;
    }

}
