using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowGrid : MonoBehaviour
{
    [SerializeField]
    Material gridMaterial;
    //[SerializeField]
    //public Color gridColor = Color.white;

    public int gridHalfWidth = 500;
    public int gridHalfHeight = 500;
    public int gridIntervalWidth = 1;
    public int gridIntervalheight = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {

        GL.PushMatrix();
        gridMaterial.SetPass(0);
        //GL.LoadOrtho();

        GL.Begin(GL.LINES);
        //GL.Color(gridColor);

        Vector3 startVertex = Vector3.zero;
        Vector3 endVertex = Vector3.zero;

        // z รเ
        startVertex.x = -gridHalfWidth;
        endVertex.x = gridHalfWidth;
        for (int z = -gridHalfHeight; z < gridHalfHeight; z += gridIntervalheight)
        {
            startVertex.z = z;
            endVertex.z = z;
            GL.Vertex(startVertex);
            GL.Vertex(endVertex);
        }
        // x รเ
        startVertex.z = -gridHalfHeight;
        endVertex.z = gridHalfHeight;
        for (int x = -gridHalfWidth; x < gridHalfWidth; x += gridIntervalWidth)
        {
            startVertex.x = x;
            endVertex.x = x;
            GL.Vertex(startVertex);
            GL.Vertex(endVertex);
        }

        GL.End();

        GL.PopMatrix();
    }
}
