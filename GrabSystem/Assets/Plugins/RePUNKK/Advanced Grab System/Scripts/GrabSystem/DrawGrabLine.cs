using UnityEngine;

public class DrawGrabLine : MonoBehaviour
{
    private float _lineWidth;
    private int _lineVertices;
    private Material _lineMaterial;
    private bool _usingColor;
    private Color _lineColor;
    private LineRenderer _grabLine;

    public void Initialize(float lineWidth, int lineVertices, Material lineMaterial, bool usingColor, Color lineColor)
    {
        _lineWidth = lineWidth;
        _lineVertices = lineVertices;
        _lineMaterial = lineMaterial;
        _usingColor = usingColor;
        _lineColor = lineColor;
    }

    public void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        if (_grabLine == null) return;

        if (_lineMaterial == null)
        {
            _grabLine.material = new Material(Shader.Find("Sprites/Default"));
        }
        else
        {
            _grabLine.material = _lineMaterial;
        }

        _grabLine.numCornerVertices = _lineVertices;
        _grabLine.numCapVertices = _lineVertices;

        if (_usingColor)
        {
            _grabLine.startColor = _lineColor;
            _grabLine.endColor = _lineColor;
        }
        
        _grabLine.startWidth = _lineWidth;
        _grabLine.endWidth = _lineWidth;

        _grabLine.SetPosition(0, startPos);
        _grabLine.SetPosition(1, endPos);
    }

    public void AddLine(GameObject obj)
    {
        _grabLine = obj.AddComponent<LineRenderer>();
    }

    public void RemoveLine(GameObject obj)
    {
        if (obj != null && obj.TryGetComponent<LineRenderer>(out LineRenderer lr))
        {
            Destroy(lr);
        }
        _grabLine = null;
    }
}
