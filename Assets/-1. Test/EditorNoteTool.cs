using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorNoteTool : MonoBehaviour
{
    [TextArea]
    public string noteText = "Note";
    
    public Color backgroundColor = Color.yellow;
    public Color textColor = Color.black;
    public Vector3 calloutOffset = new Vector3(0, 2.0f, 0);
    public float labelSize = 1.0f;

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position + calloutOffset;

        // Calculate the size of the label in world units
        Vector3 labelWorldSize = new Vector3(labelSize, labelSize * 0.5f, 1);

        // Calculate the world space corners of the label
        Vector3 screenPosition = Camera.current.WorldToScreenPoint(position);
        if (screenPosition.z > 0)
        {
            Vector3 labelSizeInScreen = Camera.current.WorldToScreenPoint(position + labelWorldSize) - screenPosition;
            Vector2 labelSizeInPixels = new Vector2(labelSizeInScreen.x, labelSizeInScreen.y);

            Handles.BeginGUI();
            Vector2 labelCenter = HandleUtility.WorldToGUIPoint(position);
            Rect rect = new Rect(labelCenter.x - labelSizeInPixels.x / 2, labelCenter.y - labelSizeInPixels.y / 2, labelSizeInPixels.x, labelSizeInPixels.y);

            // Adjust font size based on the label size
            int fontSize = Mathf.RoundToInt(labelSizeInPixels.y * 0.5f);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = textColor;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = fontSize;
            style.padding = new RectOffset(10, 10, 5, 5);
            style.normal.background = MakeTex(1, 1, backgroundColor);

            // Calculate the actual size of the text
            Vector2 textSize = style.CalcSize(new GUIContent(noteText));
            Rect textRect = new Rect(labelCenter.x - textSize.x / 2, labelCenter.y - textSize.y / 2, textSize.x, textSize.y);

            // Draw the background box
            EditorGUI.DrawRect(textRect, backgroundColor);

            // Draw the label
            GUI.Label(textRect, noteText, style);
            Handles.EndGUI();
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
