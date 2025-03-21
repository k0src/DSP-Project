using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContextMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject buttonPrefab;
    [SerializeField]
    public GameObject menuPanel;
    [SerializeField]
    public DSPGraphManager dspGraphManager;
    [SerializeField]
    public UIManager uiManager;

    private static float zPos = -1f;

    private void Update() 
    {
        if (Input.GetMouseButtonDown(1))
        {
            ShowContextMenu();
        }
    }

    private void ShowContextMenu()
    {
        menuPanel.SetActive(true);
        menuPanel.transform.position = Input.mousePosition;

        foreach (Transform child in menuPanel.transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = zPos;

        CreateButton("Connect", () =>
        {
            uiManager.ConnectNodes(worldPosition);
            menuPanel.SetActive(false);
        });

        CreateButton("Create Sine Oscillator", () =>
        {
            dspGraphManager.CreateSineOscillatorNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });

        CreateButton("Create Square Oscillator", () =>
        {
            dspGraphManager.CreateSquareOscillatorNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });

        CreateButton("Create Triangle Oscillator", () =>
        {
            dspGraphManager.CreateTriangleOscillatorNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });

        CreateButton("Create Sawtooth Oscillator", () =>
        {
            dspGraphManager.CreateSawtoothOscillatorNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });
        
        CreateButton("Create Gate Generator", () =>
        {
            dspGraphManager.CreateGateNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });

        CreateButton("Create Pitch Controller", () =>
        {
            dspGraphManager.CreatePitchNode(worldPosition);
            menuPanel.SetActive(false);
            zPos -= 1f;
        });

        CreateButton("Delete", () =>
        {
            uiManager.DeleteNode(worldPosition);
            menuPanel.SetActive(false);
        });

        CreateButton("Close", () =>
        {
            menuPanel.SetActive(false);
        });

        LayoutRebuilder.ForceRebuildLayoutImmediate(menuPanel.GetComponent<RectTransform>());
    }

    private void CreateButton(string buttonText, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject button = Instantiate(buttonPrefab, menuPanel.transform);
        button.GetComponentInChildren<Text>().text = buttonText;
        button.GetComponent<Button>().onClick.AddListener(onClickAction);
    }
}