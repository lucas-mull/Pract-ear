using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class GuideLoad : MonoBehaviour, IPointerUpHandler{

    public GameObject _panelDetails;
    public GameObject _selectedTab;
    public Image _instrumentDisplay;
    public Text _title;
    public Text _categoryText;
    public Text _infoText;
    public Dropdown _songChoices;

    GraphicRaycaster _rayCaster;
    List<RaycastResult> results = new List<RaycastResult>();
    PointerEventData ped = new PointerEventData(null);

    Instrument _lastHit;
    GameObject _activePanel;
    List<Partition> _partitions;
    Partition _selectedPartition;
    AudioSource _audioSource;    
    bool _play = false;

    // Use this for initialization
    void Start () {
        _rayCaster = this.GetComponent<GraphicRaycaster>();
        _activePanel = _selectedTab.transform.GetChild(1).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _partitions = Partition.LoadAll();
        _selectedPartition = _partitions[0];
        _songChoices.options.Clear();

        foreach (Partition partition in _partitions)
        {
            _songChoices.options.Add(new Dropdown.OptionData() { text = partition.Title});
        }

        _songChoices.value = 1;
        _songChoices.value = 0;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ped.position = Utils.GetClickedPosition();
        int size = results.Count;
        _rayCaster.Raycast(ped, results);
        if (results.Count > size)
        {
            GameObject hit = results[size].gameObject;
            if (hit.tag == "Instrument")
            {
                Debug.Log("Hit : " + hit.name);
                _lastHit = Instrument.GetInstanceFor(hit.name.ToLower());
                _panelDetails.SetActive(true);
                _activePanel.SetActive(false);
                _instrumentDisplay.sprite = hit.GetComponent<Image>().sprite;
                _title.text = _lastHit.Name;
                _categoryText.text = _lastHit.Category.ToString();              
            }
            
        }
    }

    public void onTabClick(BaseEventData data)
    {
        Debug.Log(data.selectedObject.transform.parent.name);
        GameObject tab = data.selectedObject.transform.parent.gameObject;
        if (tab != _selectedTab)
        {
            EnableButton(tab, true);
            _selectedTab = tab;
        }
    }

    public void Back()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    private void EnableButton(GameObject button, bool enable)
    {
        if (enable)
        {
            button.GetComponent<Image>().color = Colors.WHITE_BACKGROUND;
            Text label = button.GetComponentInChildren<Text>();
            label.color = Colors.BLACK;            
            _activePanel.SetActive(false);
            if (_panelDetails.activeInHierarchy)
            {
                _panelDetails.SetActive(false);
            }
            _activePanel = button.transform.GetChild(1).gameObject;
            _activePanel.SetActive(true);
            EnableButton(_selectedTab, false);
        }
        else
        {
            button.GetComponent<Image>().color = Colors.BLACK_BACKGROUND;
            Text label = button.GetComponentInChildren<Text>();
            label.color = Colors.YELLOW;
        }
    }

    public void StartListening()
    {
        _play = true;
        StartCoroutine(PlayPartition());
    }

    public void onValueChanged()
    {
        int value = _songChoices.value;
        if (_play)
            _play = false;

        _selectedPartition = _partitions[value];
    }

    System.Collections.IEnumerator PlayPartition()
    {
        _selectedPartition.StartReading();
        while (!_selectedPartition.isDoneReading() && _play)
        {
            Note toPlay = _selectedPartition.ReadNextNote();
            _audioSource.clip = Resources.Load<AudioClip>(toPlay.GetFileNameFor(_lastHit));
            _audioSource.Play();
            yield return new WaitForSeconds(toPlay.GetLengthInSeconds());
        }

        _play = false;
    }
}
