using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ntw.CurvedTextMeshPro;

public class RenameAnchorHandler : MonoBehaviour
{

    [SerializeField]
    private Transform cameraContainerTransform;
    [SerializeField]

    private InputField inputField;
    private GameObject objectToRename;

    private TMP_Text circleMesh;

    public bool isInputFinished
    {
        get; private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 28;

    }

    void Update()
    {
        string anchorText = inputField.text;
        if (circleMesh == null)
        {
            return;
        }
        if (inputField.text.Length == 0)
        {
            if(objectToRename.GetComponent<AnchorProperties>().anchorLabel != null && 
                    objectToRename.GetComponent<AnchorProperties>().anchorLabel.Length > 0)
            {
                circleMesh.text = LoopLabel(objectToRename.GetComponent<AnchorProperties>().anchorLabel);
            }
            else
            {
                circleMesh.text = LoopLabel("New Anchor");
            }
        }
        else
        {
            GenerateLabel();
        }
    }

    public void GenerateLabel()
    {
        string label = inputField.text;
        int anchorLabelLength = 0;
        circleMesh.text = "";
        while (anchorLabelLength + label.Length <= 28)
        {
            circleMesh.text += label;
            int spaceAmount = 0;
            for (int i = 0; i <= label.Length; i += 4)
            {
                spaceAmount++;
                circleMesh.text += " ";
            }
            if (label.Length >= 9 && label.Length <= 11)
            {
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees = 320;
            }
            else
            {
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees = 340;
            }
            if (circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees % 2 == 0)
            {
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees++;
            }
            else
            {
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees--;
            }

            anchorLabelLength += label.Length + spaceAmount;


        }



    }

    public void SelectInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }
    public void DeselectInputField()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
    public void SubmitAnchorUpdate()
    {
        objectToRename.GetComponent<AnchorProperties>().anchorLabel = inputField.text;
        DeselectInputField();
        inputField.text = "";
        isInputFinished = true;

    }
    public void SetObjectToRename(GameObject anchorToRename)
    {
        objectToRename = anchorToRename;
        circleMesh = cameraContainerTransform.GetComponentInChildren<TMP_Text>();
    }
    public GameObject GetObjectToRename()
    {
        return objectToRename;
    }
    public static string LoopLabel(string label)
    {
        string loopedLabel = "";
        int anchorLabelLength = 0;
        while (anchorLabelLength + label.Length <= 28)
        {
            loopedLabel += label;
            int spaceAmount = 0;
            for (int i = 0; i <= label.Length; i += 4)
            {
                spaceAmount++;
                loopedLabel += " ";
            }

            anchorLabelLength += label.Length + spaceAmount;


        }
        return loopedLabel;
    }

}
