using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script is currently unused but contains useful functionality for annotating individual points, which may be useful for some applications
public class Annotation : MonoBehaviour
{   
    private bool isAnnotationMode = false;
    public Button annotationButton;
    public Color annotationColor1;
    public Color annotationColor2;
    private GameObject selectedPoint;
    public TMP_FontAsset labelFont;
    public Color labelColor = Color.red;
    public Transform parent;
    public int labelFontSize = 30;
    public Canvas referenceCanvas;
    private TMP_InputField label;

    void Update()
    {
        if (isAnnotationMode)
        {
            if (Input.GetMouseButtonDown(0))
            {   
                float parentWidth = parent.lossyScale.x;
                float parentHeight = parent.lossyScale.y;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                if (hit.collider != null)
                {
                    // Is clicked object a point?
                    GameObject clickedObject = hit.collider.gameObject;
                    if (clickedObject.CompareTag("Point"))
                    {
                        // Enlarge point
                        SelectPoint(clickedObject);

                        // Does an annotation already exist?
                        if (LabelExistsAlready(clickedObject) == false)
                        {
                            GameObject labelObj = new GameObject("Label");
                            labelObj.transform.SetParent(clickedObject.transform, false);
                            labelObj.tag = "Annotation";

                            Canvas canvasComponent = labelObj.AddComponent<Canvas>();
                            canvasComponent.renderMode = RenderMode.WorldSpace;
                            labelObj.AddComponent<GraphicRaycaster>();

                            GameObject textBox = new GameObject("TextBox");
                            textBox.transform.SetParent(labelObj.transform, false);
                            label = textBox.AddComponent<TMP_InputField>();

                            TextMeshProUGUI textComponent = textBox.AddComponent<TextMeshProUGUI>();
                            textComponent.font = labelFont;
                            textComponent.fontSize = labelFontSize;
                            textComponent.color = labelColor;
                            label.textComponent = textComponent;
                            label.text = "Add your text";

                            RectTransform textBoxRectTransform = textBox.GetComponent<RectTransform>();

                            textBoxRectTransform.localPosition = new Vector3(0f, 0f, 0f); // Adjust position if needed
                            textBoxRectTransform.localScale = new Vector3(parentWidth, parentHeight, 1f);
                            textBox.SetActive(true);
                        }
                        else
                        {
                            label = clickedObject.GetComponentsInChildren<TMP_InputField>()[0];
                        }
                        
                    }
                }
            }
            

            if (Input.anyKeyDown && label != null)
            {
                // Add pressed keys to annotation
                string key = GetKeyPressed();
                if (label.text == "Add your text" && key != "")
                {
                    label.text = key;
                }
                else
                {
                    label.text += key;
                }  
            }
        
        }
        else if (selectedPoint != null)
        {
            selectedPoint.transform.localScale = new Vector3(-15.82343f, -15.28404f, 1.673122f);
            
        }
    }

    public void ToggleAnnotationMode()
    {
        isAnnotationMode = !isAnnotationMode;

        if (isAnnotationMode)
        {
            annotationColor1.a = 1f;
            annotationButton.image.color = annotationColor1;
            
        }
        else
        {
            annotationColor2.a = 1f;
            annotationButton.image.color = annotationColor2;
        }
    }

    public bool GetIsAnnotationMode()
    {
        return isAnnotationMode;
    }

    void SelectPoint(GameObject point)
    {
        if (selectedPoint != null)
        {
            selectedPoint.transform.localScale = new Vector3(-15.82343f, -15.28404f, 1.673122f);
        }

        selectedPoint = point;
        selectedPoint.transform.localScale = new Vector3(60f, 60f, 2f);
    }

    public bool LabelExistsAlready(GameObject point)
    {
        bool labelExists = false;
        foreach (Transform child in point.transform)
        {
            if (child.CompareTag("Annotation"))
            {
                labelExists = true;
                break;
            }
        }

        return labelExists;
    }

    private string GetKeyPressed()
    {
        string keyPressed = "";

        if (Input.GetKeyDown(KeyCode.Return))
        {
            keyPressed = "\n";
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (label.text.Length > 0)
            {
                label.text = label.text.Substring(0, label.text.Length - 1);
            }
        }
        else
        {
            keyPressed = Input.inputString;
        }

        return keyPressed;
    }

}
