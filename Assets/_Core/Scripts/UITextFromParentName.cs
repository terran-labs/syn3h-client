using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UITextFromParentName : MonoBehaviour
{
    private Text myText;
    private Transform myParent;

    void Start()
    {
        myParent = transform.parent;
        myText = GetComponent<Text>();
        myText.text = myParent.name;
    }

#if UNITY_EDITOR
    void Update()
    {
        myText.text = myParent.name;
    }
#endif
}