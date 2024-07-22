using UnityEngine;
using UnityEngine.UI;

public class InputFieldFocus : MonoBehaviour
{
    public InputField myInputField;
    private int lastCaretPosition = 0;

    void Start()
    {
        myInputField = GetComponent<InputField>();
        myInputField.ActivateInputField();

        // 커서를 텍스트의 끝으로 이동 (선택사항)
        myInputField.caretPosition = myInputField.text.Length;

        myInputField.caretBlinkRate = 0.5f;

        myInputField.caretColor = Color.red; 
    }
    private void Update()
    {
        int currentCaretPosition = myInputField.caretPosition;

        // 커서 위치가 변경되었는지 확인
        if (currentCaretPosition != lastCaretPosition)
        {
            Debug.Log("Current cursor position: " + currentCaretPosition);
            lastCaretPosition = currentCaretPosition;
        }
    }
}
