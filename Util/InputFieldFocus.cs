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

        // Ŀ���� �ؽ�Ʈ�� ������ �̵� (���û���)
        myInputField.caretPosition = myInputField.text.Length;

        myInputField.caretBlinkRate = 0.5f;

        myInputField.caretColor = Color.red; 
    }
    private void Update()
    {
        int currentCaretPosition = myInputField.caretPosition;

        // Ŀ�� ��ġ�� ����Ǿ����� Ȯ��
        if (currentCaretPosition != lastCaretPosition)
        {
            Debug.Log("Current cursor position: " + currentCaretPosition);
            lastCaretPosition = currentCaretPosition;
        }
    }
}
