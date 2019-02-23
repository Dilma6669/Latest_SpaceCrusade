using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UIManager _instance;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    SyncedVars _syncedVars;
    Text playerIDText;
    Text playerNameText;
    Text totalPlayerText;
    Text seedNumText;

    ////////////////////////////////////////////////

    public delegate void ChangeLayerEvent(int change);
	public static event ChangeLayerEvent OnChangeLayerClick;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        playerIDText = transform.FindDeepChild("PlayerNum").GetComponent<Text>();
        playerNameText = transform.FindDeepChild("PlayerName").GetComponent<Text>();
        totalPlayerText = transform.FindDeepChild("TotalPlayersNum").GetComponent<Text>();
        seedNumText = transform.FindDeepChild("SeedNum").GetComponent<Text>();
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        _syncedVars = _gameManager._networkManager._syncedVars;
        if (_syncedVars == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // The players personal GUI
    public void SetUpPlayersGUI(int playerID)
    {
        GetComponent<Canvas>().enabled = true;

        playerIDText.text = playerID.ToString();
        playerNameText.text = _gameManager._playerManager.PlayerName;
        seedNumText.text = _syncedVars.GlobalSeed.ToString();
    }

	public void UpdateTotalPlayersGUI(int total) {

        totalPlayerText.text = total.ToString();
	}

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void ChangeLayer(bool UpDown)
    {
        if (UpDown)
        {
            if (OnChangeLayerClick != null)
                OnChangeLayerClick(1);
        }
        else
        {
            if (OnChangeLayerClick != null)
                OnChangeLayerClick(-1);
        }
    }
}
