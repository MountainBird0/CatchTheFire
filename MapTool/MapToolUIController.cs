using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapToolUIController : MonoBehaviour
{
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button loadBtn;

    [SerializeField] private TMP_InputField fileNameInput;
    [SerializeField] private TMP_Text text;

    [SerializeField] private TMP_Dropdown dropDown;

    private EMapInformation mapInfor = EMapInformation.MAP1;

    public CustomFloor customFloor;
    private void Awake()
    {
        saveBtn.onClick.AddListener(()=>
        {
            //CSVDataReader.Instance.WriteMapDataToCSV(customFloor.CreateMapBoundary());
            GoogleSheetDataReader.Instance.WriteMapDataToGoogle(customFloor.CreateMapBoundary());

        });

        loadBtn.onClick.AddListener(() =>
        {
            //CustomFloor.Instance.mapDatas = CSVDataReader.Instance.ReadCSVToMapData();

            //CustomFloor.Instance.mapDatas = GoogleSheetDataReader.Instance.ReadGenerateGoogleToMapData();

            CustomFloor.Instance.mapDatas = GoogleSheetDataReader.Instance.ReadRealTimeGoogleToMapData();

        });

        dropDown.onValueChanged.AddListener(delegate
        {
            ChangeDropDown(dropDown.value);
        });

        //fileNameInput.onValueChanged.AddListener(ValueChanged);

        //UpdateButtonInteraction(fileNameInput.text);
    }
    private void Start()
    {
        CSVDataReader.Instance.onSaveResult += ShowSaveResult;
    }

    private void ValueChanged(string text)
    {
        CSVDataReader.Instance.fileName = text;
        UpdateButtonInteraction(text);
    }

    private void ShowSaveResult(string result)
    {
        text.text = result;
    }

    private void UpdateButtonInteraction(string text)
    {
        bool isInteract = !string.IsNullOrEmpty(text);

        saveBtn.interactable = isInteract;
        loadBtn.interactable = isInteract;
    }

    public void ChangeDropDown(int index)
    {
        mapInfor = (EMapInformation)index;
        GoogleSheetDataReader.Instance.currentMapInfor = mapInfor;
    }
}
