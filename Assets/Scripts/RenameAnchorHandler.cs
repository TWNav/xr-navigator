using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ntw.CurvedTextMeshPro;

public class RenameAnchorHandler : MonoBehaviour
{

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private GameObject objectToRename;

    private TMP_Text circleMesh;
    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 28;
        circleMesh = objectToRename.GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        string anchorText = inputField.text;
        if(inputField.text.Length == 0)
        {
            circleMesh.text = "TEST   TEST    TEST   TEST";
        }
        else
        {
            GenerateLabel();
        }
    }

    private void GenerateLabel()
    {
        string label = inputField.text;
        int anchorLabelLength = 0;
        circleMesh.text = "";
        while(anchorLabelLength <= 28)
        {
            circleMesh.text += label;
            int spaceAmount = 0;
            for(int i = 0; i <= label.Length; i+=4)
            {
                spaceAmount++;
                circleMesh.text += " ";
            }
            if(label.Length >= 9 && label.Length <= 11)
            {
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees = 320;
            }
            else{
                circleMesh.GetComponent<TextProOnACircle>().m_arcDegrees = 340;
            }
            anchorLabelLength += label.Length + spaceAmount;
            if(anchorLabelLength + label.Length > 28)
            {
                return;
            }
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
        
    }
}
