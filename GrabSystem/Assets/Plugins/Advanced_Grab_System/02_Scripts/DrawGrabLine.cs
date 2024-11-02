using UnityEngine;

public class DrawGrabLine : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.01f;
    [SerializeField] private int lineVertices = 10;
    [SerializeField] private Material lineMaterial;
    [SerializeField] private bool usingColor = true;
    [SerializeField] private Color lineColor = Color.white;
    private LineRenderer grabLine;

    public void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        if (lineMaterial == null) grabLine.material = new Material(Shader.Find("Sprites/Default"));
        else grabLine.material = lineMaterial;

        grabLine.numCornerVertices = lineVertices;
        grabLine.numCapVertices = lineVertices;

        if (usingColor)
        {
            grabLine.startColor = lineColor;
            grabLine.endColor = lineColor;
        }
        
        grabLine.startWidth = lineWidth;
        grabLine.endWidth = lineWidth;

        grabLine.SetPosition(0, startPos);
        grabLine.SetPosition(1, endPos);
    }

    public void AddLine(GameObject obj)
    {
        grabLine = obj.AddComponent<LineRenderer>();
    }
    public void RemoveLine(GameObject obj)
    {
        Destroy(obj.GetComponent<LineRenderer>());
    }

    public void SetWidth(float width) { lineWidth = width; }
    public void SetLineVertices(int vertices) { lineVertices = vertices; }
    public void SetLineColor(Color color) { lineColor = color; }
}
