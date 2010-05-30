package Diplomska.Protipi;

import javax.media.opengl.GL;
import javax.media.opengl.GLAutoDrawable;
import javax.media.opengl.GLEventListener;
import javax.media.opengl.glu.GLU;

import java.util.ArrayList;
import java.util.List;

import java.io.*;


import java.awt.event.MouseEvent;
import java.awt.event.MouseWheelEvent;
import java.awt.event.MouseListener;
import java.awt.event.MouseMotionListener;
import java.awt.event.MouseWheelListener;




/**
 * GLRenderer.java <BR>
 * author: Brian Paul (converted to Java by Ron Cemer and Sven Goethel) <P>
 *
 * This version is equal to Brian Paul's version 1.2 1999/10/21
 */
public class GLRenderer implements GLEventListener, MouseListener, MouseWheelListener, MouseMotionListener {

    List<Poly> polys;
    Poly backPoly;
    Color3 backColor;

    // Camera
    Vec3 eye = new Vec3 (0.0f, 0.0f, -27.0f);
    Vec3 center = new Vec3 (0.0f, 0.0f, 0.0f);
    Vec3 up = new Vec3 (0.0f, 1.0f, 0.0f);

    // GLU
    private final GLU glu = new GLU();

    


    public void parse(String filename)
    {
        

        polys = new ArrayList<Poly>();
        backPoly = new Poly();

        FileReader fr;
        BufferedReader br;
        String line;

        try {
                fr = new FileReader(filename);
                br = new BufferedReader(fr);

                Poly currentPoly = new Poly();
                Triangle currentTri = new Triangle();
                
                 // First parse backgorund poly!
                while (!(line = br.readLine()).equals("###")) // ### is endof poly
                {
                    Vec3 p = parseCoord(line);
                    backPoly.addVertex(p);
                }

                System.out.println("--- BACKGROUND COLOR ---");
                // Next line is background color
                backColor = parseColor(br.readLine());



                while ( (line = br.readLine()) != null)
                {
                   

                    // loop through all lines
                    if (line.equals("#####")/* == "#####"*/)
                    {
                        // end of poly!!!
                        // add it to list
                        polys.add(currentPoly);

                        // and start a new one!
                        currentPoly = new Poly();
                    }
                    else if (line.equals("#"))
                    {
                        // We have new triangle, add it to poly and clear it
                        currentTri.calcNormal();
                        currentPoly.addTri(currentTri);
                        currentTri = new Triangle();

                    }
                    else if (line.startsWith("C:")) // we have a color
                    {
                        Color3 clr = parseColor(line);
                        currentPoly.color = clr;

                    }
                    else
                    {
                        // new point!!!
                        Vec3 p = parseCoord(line);

                        // add to current tri
                        currentTri.addVertex(p);

                        // add to current poly
                        //currentPoly.addVertex(p);
                    }
                }

                

        }
        catch(IOException e) {
                System.out.println("Error reading vertices line." );
                e.printStackTrace();
        }

        System.out.println("Data parsed!!!!!!:)");
    
    }
    
    public Vec3 parseCoord(String coordStr) {
              String[] result = coordStr.split(" ");

              //System.out.println(result[0]);
              //System.out.println(result[1]);
              Vec3 pres = new Vec3(Float.parseFloat(result[0]), Float.parseFloat(result[1]), Float.parseFloat(result[2]));

              // translate
              pres.x -= 0.5f;
              pres.y -= 0.5f;
              

              // and scale
              pres.x *= -20;
              pres.y *= -20;

              //
              pres.z *= 0.6; // 0.1;
              //

              // TODO: Z-CORD?

              // coords should be in range [2, 1], [-2, -1]

              //return new Vec2(Float.parseFloat(result[0]), Float.parseFloat(result[1])); // TODO
              return pres;
    }


    public Color3 parseColor(String coordStr) {
              String[] result = coordStr.split(" ");

            //  result[0].replace("C", "");
             // result[0].replace(":", "");
             // result[0].trim();
              result[0] = result[0].substring(2);

              System.out.println(result[0]);

              //System.out.println(result[0]);
              //System.out.println(result[1]);
              Color3 pres = new Color3(Float.parseFloat(result[0]), Float.parseFloat(result[1]), Float.parseFloat(result[2]) );

              // translate
              pres.r /= 255.0f;
              pres.g /= 255.0f;
              pres.b /= 255.0f;

              // and scale
            //  pres.x *= 4;
           //   pres.y *= -2;*/

              // coords should be in range [2, 1], [-2, -1]

              //return new Vec2(Float.parseFloat(result[0]), Float.parseFloat(result[1])); // TODO
              return pres;
    }

    public void drawPoly(GLAutoDrawable drawable, Poly ply)
    {
        GL gl = drawable.getGL();

        gl.glPushMatrix();

        gl.glColor3f(ply.color.r, ply.color.g, ply.color.b);
    /*    gl.glBegin(GL.GL_TRIANGLE_FAN);
        for (Vec2 vec : ply.vertices)
        {
            gl.glVertex3f(vec.x, vec.y, 0.0f);
        }
        gl.glEnd();*/

        for (Triangle tri : ply.tris)
        {
            gl.glNormal3f(tri.n.x, tri.n.y, tri.n.z);
            gl.glBegin(GL.GL_TRIANGLES);
                gl.glVertex3f(tri.getVertex(0).x, tri.getVertex(0).y, tri.getVertex(0).z);
                gl.glVertex3f(tri.getVertex(1).x, tri.getVertex(1).y, tri.getVertex(1).z);
                gl.glVertex3f(tri.getVertex(2).x, tri.getVertex(2).y, tri.getVertex(2).z);

            gl.glEnd();
        }


        gl.glPopMatrix();

    }

    public void init(GLAutoDrawable drawable) {
        // Use debug pipeline
        // drawable.setGL(new DebugGL(drawable.getGL()));

        GL gl = drawable.getGL();
        System.err.println("INIT GL IS: " + gl.getClass().getName());

        // Enable VSync
        gl.setSwapInterval(1);

        // Setup the drawing area and shading mode
        gl.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        gl.glShadeModel(GL.GL_SMOOTH); // try setting this to GL_FLAT and see what happens.



        // material
        gl.glEnable(GL.GL_COLOR_MATERIAL);



	      // secular, shininess
	      float[] mat_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
	      float[] mat_ambient = { 0.7f, 0.7f, 0.7f, 1.0f };
	      float[] mat_diffuse = { 0.7f, 0.6f, 0.8f, 1.0f };
	      float[] mat_shininess = { 19.0f };

	      gl.glMaterialfv (GL.GL_FRONT, GL.GL_SPECULAR, mat_specular, 0);
	      gl.glMaterialfv (GL.GL_FRONT, GL.GL_AMBIENT, mat_ambient, 0);
	      gl.glMaterialfv (GL.GL_FRONT, GL.GL_DIFFUSE, mat_diffuse, 0);
	      gl.glMaterialfv (GL.GL_FRONT, GL.GL_SHININESS, mat_shininess, 0);

	// define the light source's position -- to the right, above, and in
	// front of the (0, 0, 0) origin.
        gl.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, new float[] { 2.0f, 30.0f, -12.0f, 1.0f }, 0);

	//  gl.glLightfv (GL.GL_LIGHT0, GL.GL_POSITION, light_position, 0);

	// diffuse color
	float [] light_diffuse = { 0.9f, 0.6f, 0.2f, 1.0f };
	gl.glLightfv (GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse, 0);

	// ambient color
	float [] light_ambient = { 0.6f, 0.6f, 0.6f, 1.0f };
	gl.glLightfv (GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient, 0);

        // enable lighting and ligh0
	gl.glEnable (GL.GL_LIGHTING);
	gl.glEnable (GL.GL_LIGHT0);


        





        this.parse("/home/aljosa/Pictures/xtreme-pic/Referencne/TRIANG_tab35c.wall");
    }

    public void reshape(GLAutoDrawable drawable, int x, int y, int width, int height) {
        GL gl = drawable.getGL();
        GLU glu = new GLU();

        if (height <= 0) { // avoid a divide by zero error!
        
            height = 1;
        }
        final float h = (float) width / (float) height;
        gl.glViewport(0, 0, width, height);
        gl.glMatrixMode(GL.GL_PROJECTION);
        gl.glLoadIdentity();
        glu.gluPerspective(45.0f, h, 1.0, 30.0);
        gl.glMatrixMode(GL.GL_MODELVIEW);
        gl.glLoadIdentity();
    }



    public void display(GLAutoDrawable drawable) {
        GL gl = drawable.getGL();

        // Clear the drawing area
        gl.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
        // Reset the current matrix to the "identity"
        gl.glLoadIdentity();

        // Move the "drawing cursor" around
      //  gl.glTranslatef(0.0f, 0.0f, -27.0f);

        glu.gluLookAt(eye.x, eye.y, eye.z, center.x, center.y, center.z, up.x, up.y, up.z);

        // Drawing Using Triangles
     /*   gl.glBegin(GL.GL_TRIANGLES);
            gl.glColor3f(1.0f, 0.0f, 0.0f);    // Set the current drawing color to red
            gl.glVertex3f(0.0f, 1.0f, 0.0f);   // Top
            gl.glColor3f(0.0f, 1.0f, 0.0f);    // Set the current drawing color to green
            gl.glVertex3f(-1.0f, -1.0f, 0.0f); // Bottom Left
            gl.glColor3f(0.0f, 0.0f, 1.0f);    // Set the current drawing color to blue
            gl.glVertex3f(1.0f, -1.0f, 0.0f);  // Bottom Right
        // Finished Drawing The Triangle
        gl.glEnd();

        // Move the "drawing cursor" to another position
        gl.glTranslatef(3.0f, 0.0f, 0.0f);
        // Draw A Quad
        gl.glBegin(GL.GL_QUADS);
            gl.glColor3f(0.5f, 0.5f, 1.0f);    // Set the current drawing color to light blue
            gl.glVertex3f(-1.0f, 1.0f, 0.0f);  // Top Left
            gl.glVertex3f(1.0f, 1.0f, 0.0f);   // Top Right
            gl.glVertex3f(1.0f, -1.0f, 0.0f);  // Bottom Right
            gl.glVertex3f(-1.0f, -1.0f, 0.0f); // Bottom Left
        // Done Drawing The Quad
        gl.glEnd();*/

        gl.glPushMatrix();

        //
       /* gl.glColor3f(1.0f, 0.0f, 0.0f);
        gl.glBegin(GL.GL_LINES);
        gl.glVertex3f(-2.0f, 0.0f, 0.0f);  // Top Left
        gl.glVertex3f(2.0f, 0.0f, 0.0f);  // Top Left

        gl.glVertex3f(0.0f, 1.0f, 0.0f);  // Top Left
        gl.glVertex3f(0.0f, -1.0f, 0.0f);  // Top Left
        gl.glEnd();*/
        //

        // draw background
        gl.glColor3f(backColor.r, backColor.g, backColor.b);
        gl.glNormal3f(0.0f, 0.0f,  1.0f);
        gl.glBegin(GL.GL_TRIANGLE_FAN);
        for (Vec3 v : backPoly.vertices)
            gl.glVertex3f(v.x, v.y, v.z);
        gl.glEnd();

        // draw plys
    //    gl.glColor3f(0.0f, 1.0f, 0.0f);
        for (Poly ply : polys)
        {
            drawPoly(drawable, ply);
        }
        gl.glPopMatrix();


        // Flush all drawing operations to the graphics card
        gl.glFlush();
    }

    public void displayChanged(GLAutoDrawable drawable, boolean modeChanged, boolean deviceChanged) {
    }

    public void mouseClicked(MouseEvent e) {
        System.out.print("Click!");

    }

    public void mouseDragged(MouseEvent e) {

    }

    public void mouseEntered(MouseEvent e) {

    }

    public void mouseExited(MouseEvent e) {

    }

    public void mouseMoved(MouseEvent e) {

    }

    public void mousePressed(MouseEvent e) {

    }

    public void mouseReleased(MouseEvent e) {

    }

    public void mouseWheelMoved(MouseWheelEvent e) {
        System.out.println( e.getWheelRotation());
       // this.

        eye.z += (float)e.getWheelRotation();



    }

}

