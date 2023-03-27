using UnityEditor;
using UnityEngine;

namespace StageLightManeuver
{
    public static class PanTiltEditorDrawer
    {

        public static void DrawCanvas()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            //get current rect position
                    
            var position = EditorGUILayout.GetControlRect();
            Rect layoutRectangle = GUILayoutUtility.GetRect(position.width,position.width,position.width,position.width);

            if(Event.current.type == EventType.Repaint)
            {
                // If we are currently in the Repaint event, begin to draw a clip of the size of 
                // previously reserved rectangle, and push the current matrix for drawing.
                GUI.BeginClip(layoutRectangle);
                GL.PushMatrix();

                // Clear the current render buffer, setting a new background colour, and set our
                // material for rendering.
                GL.Clear(true, false, Color.black);
                // material.SetPass(0);

                // Start drawing in OpenGL Quads, to draw the background canvas. Set the
                // colour black as the current OpenGL drawing colour, and draw a quad covering
                // the dimensions of the layoutRectangle.
                GL.Begin(GL.QUADS);
                GL.Color(Color.black);
                GL.Vertex3(0, 0, 0);
                GL.Vertex3(layoutRectangle.width, 0, 0);
                GL.Vertex3(layoutRectangle.width, layoutRectangle.height, 0);
                GL.Vertex3(0, layoutRectangle.height, 0);
                GL.End();

                // Start drawing in OpenGL Lines, to draw the lines of the grid.
                GL.Begin(GL.LINES);

                // Store measurement values to determine the offset, for scrolling animation,
                // and the line count, for drawing the grid.
                int offset = (Time.frameCount * 2) % 50;
                int count = (int)(layoutRectangle.width / 10) + 20;

                for(int i = 0; i < count; i++)
                {
                    // For every line being drawn in the grid, create a colour placeholder; if the
                    // current index is divisible by 5, we are at a major segment line; set this
                    // colour to a dark grey. If the current index is not divisible by 5, we are
                    // at a minor segment line; set this colour to a lighter grey. Set the derived
                    // colour as the current OpenGL drawing colour.
                    Color lineColour = i % 5 == 0 ? new Color(0.5f, 0.5f, 0.5f) : new Color(0.2f, 0.2f, 0.2f);
                    GL.Color(lineColour);

                    // Derive a new x co-ordinate from the initial index, converting it straight
                    // into line positions, and move it back to adjust for the animation offset.
                    float x = i * 10 - offset;

                    if (x >= 0 && x < layoutRectangle.width)
                    {
                        // If the current derived x position is within the bounds of the
                        // rectangle, draw another vertical line.
                        GL.Vertex3(x, 0, 0);
                        GL.Vertex3(x, layoutRectangle.height, 0);
                    }

                    if (i < layoutRectangle.height / 10)
                    {
                        // Convert the current index value into a y position, and if it is within
                        // the bounds of the rectangle, draw another horizontal line.
                        GL.Vertex3(0, i * 10, 0);
                        GL.Vertex3(layoutRectangle.width, i * 10, 0);
                    }
                }

                // End lines drawing.
                GL.End();

                // Pop the current matrix for rendering, and end the drawing clip.
                GL.PopMatrix();
                GUI.EndClip();
            }

            // End our horizontal 
            GUILayout.EndHorizontal();
        }
    }
}