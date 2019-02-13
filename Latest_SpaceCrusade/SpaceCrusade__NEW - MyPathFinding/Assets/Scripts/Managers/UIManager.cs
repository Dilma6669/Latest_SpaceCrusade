using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UIManager _instance;

    ////////////////////////////////////////////////

    GameManager _gameManager;
    
    ////////////////////////////////////////////////

    public delegate void ChangeLayerEvent(int change);
	public static event ChangeLayerEvent OnChangeLayerClick;

	public PlayerAgent _playerAgent;

	public Text _playerIDGUINum;

	public Text _playerTotalGUINum;


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
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

	public void ChangeLayer(bool UpDown) {

		if (UpDown) {
			if(OnChangeLayerClick != null)
				OnChangeLayerClick(1);
		} else {
			if(OnChangeLayerClick != null)
				OnChangeLayerClick(-1);
		}
	}


	public void UpdatePlayerGUINum(int ID, int total) {

		_playerIDGUINum.text = ID.ToString();
		_playerTotalGUINum.text = total.ToString();

	}

}
