/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package Diplomska.Protipi;

/**
 *
 * @author aljosa
 */
public class Vec3 extends Vec2 {
    public float z;

    public Vec3()
    {
        super();
        z = 0.0f;
    }

    public Vec3(float x, float y, float z)
    {
        super(x, y);
        this.z = z;
    }


}
