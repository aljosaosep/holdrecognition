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
public class Triangle {
   // public Vec2 V1, V2, V3;
    public List<Vec3> vertices;
    public Vec3 n;

    public Triangle()
    {
        this.vertices = new ArrayList<Vec3>();
        this.n = null; // new Vec3();
    }

    public void calcNormal()
    {
        Vec3 V0 = this.vertices.get(0);
        Vec3 V1 = this.vertices.get(1);
        Vec3 V2 = this.vertices.get(2);

        Vec3 A = new Vec3 ( V1.x - V0.x, V1.y - V0.y, V1.z - V0.z );
        Vec3 B = new Vec3 ( V2.x - V0.x, V2.y - V0.y, V2.z - V0.z );

        this.n = new Vec3(A.y * B.z - A.z*B.y, A.z*B.x - A.x * B.z, A.x*B.y - A.y * B.x);
    }

 /*   public Triangle(Vec2 v1, Vec2 v2, Vec2 v3)
    {

    }*/

    public void addVertex(Vec3 vertex)
    {
        if (this.vertices.size() <= 3)
            this.vertices.add(vertex);
    }

    public Vec3 getVertex(int i)
    {
        if (i < 3)
            return this.vertices.get(i);
        else return null;
    }

}
