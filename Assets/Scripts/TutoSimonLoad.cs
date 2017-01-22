using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TutoSimonLoad : MonoBehaviour {

    const int MAX_ERROR = 3;
    string[] WIN_MESSAGES = new string[] { "Bravo !", "Champion !", "Quel talent !", "Incroyable !", "Bien joué !" };
    string[] FAIL_MESSAGES = new string[] { "Aïe, raté !", "Dommage !", "Essaye encore !", "Presque !" };

    public Canvas _canvasIntro;
    public Canvas _canvasSuccess;
    public Canvas _canvasGameOver;
    public GameObject _rideaux;
    public Text _bulleAdvice;
    public Text _goalText;
    public Text _currentScoreText;
    public Camera _camera;
    public GameObject _panelArrows;
    public Image[] _lifeSprites;

    Instrument _piano;
    Instrument _guitare;
    Instrument _trompette;
    Instrument _marimba;
    Instrument _correct;

    Partition _song;

    AudioSource _audioSource;

    AudioClip _booh;
    AudioClip _yeaah;

    Animator _mainAnimator;

    int _sequenceIndex = 0;
    int _currentIndex = 0;
    int _errorCount = 0;
    int _step = 0;
    bool _nextStep = false;
    bool _gameOver = false;

    List<Instrument> _sequence = new List<Instrument>(4);

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();

        _goalText.text = "" + 4;
        _currentScoreText.text = "" + 0;

        _piano = new Piano();
        _piano.PutFarLeft(null);

        _guitare = new Guitare();
        _guitare.PutMiddleLeft(null);

        _trompette = new Trompette();
        _trompette.PutMiddleRight(null);

        _marimba = new Marimba();
        _marimba.PutFarRight(null);

        _sequence.Add(_trompette);
        _sequence.Add(_piano);
        _sequence.Add(_marimba);
        _sequence.Add(_guitare);

        _song = Partition.LoadPartitionFromJson("petit_papa_noel");
        _yeaah = Resources.Load<AudioClip>("SFX/crowd_applause");
        _booh = Resources.Load<AudioClip>("SFX/crowd_boo");

        _mainAnimator = _canvasIntro.GetComponent<Animator>();

        EnableColliders(false);
    }

    public void Restart()
    {
        _step = 1;
        foreach(Image sprite in _lifeSprites)
        {
            sprite.GetComponent<Animator>().SetBool("died", false);
        }

        _canvasGameOver.enabled = false;
        _gameOver = false;        
    }
	
	// Update is called once per frame
	void Update () {

        if (_nextStep)
        {
            if (_gameOver)
                return;

            if (_step == 1)
            {
                _mainAnimator.SetInteger("step", _step);                
                StartCoroutine(step1());
            }
            else if (_step == 2)
            {
                _mainAnimator.SetBool("done", false);
                _mainAnimator.SetInteger("step", _step);
            }
            else if (_step == 3)
            {
                StartCoroutine(step3());
            }
            else if (_step == 4)
            {
                StartCoroutine(step4());
            }
            else if (_step == 5)
            {
                StartCoroutine(step5());
            }
            else if (_step > 5)
            {
                _canvasSuccess.enabled = true;
                DifficultyManager.UnlockDifficulty(DifficultyManager.EASY, DifficultyManager.SIMON);
            }
            _nextStep = false;
        }

        StartCoroutine(InterceptClicks());
	}

    public void DismissIntro()
    {
        _mainAnimator.SetBool("done", true);
        _rideaux.GetComponent<Animator>().enabled = true;
        _canvasIntro.sortingOrder = 0;
        _nextStep = true;
        _step++;
    }

    public void DismissStep2()
    {
        _mainAnimator.SetBool("done", true);
        _step++;
        _nextStep = true;
    }

    public void BackToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void LoadSimon()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("simon");
    }

    Note PlayNote(Instrument instrument)
    {
        Note note = _song.GetNoteAt(_sequenceIndex);
        _audioSource.clip = note.GetClipFor(instrument);
        _audioSource.Play();

        return note;
    }

    IEnumerator step1()
    {
        _bulleAdvice.text = "ÉCOUTE !";
        yield return new WaitForSeconds(1);
        _correct = _trompette;
        StartCoroutine(Play());
    }

    IEnumerator step3()
    {
        _bulleAdvice.text = "ÉCOUTE !";
        yield return new WaitForSeconds(1);
        _correct = _piano;
        StartCoroutine(Play());
    }

    IEnumerator step4()
    {
        _bulleAdvice.text = "ÉCOUTE !";
        yield return new WaitForSeconds(1);
        _correct = _marimba;
        StartCoroutine(Play());
    }

    IEnumerator step5()
    {
        _bulleAdvice.text = "ÉCOUTE !";
        yield return new WaitForSeconds(1);
        _correct = _guitare;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        if (_audioSource.isPlaying)
            _audioSource.Stop();

        AnimateInstruments(true);

        for (int i = 0; i < _sequenceIndex + 1; i++)
        {
            Note note = PlayNote(_sequence[i]);
            yield return new WaitForSeconds(note.GetLengthInSeconds());
        }
        
        _bulleAdvice.text = "CHOISIS !";

        AnimateInstruments(false);
        EnableColliders(true);
    }

    IEnumerator InterceptClicks()
    {
        if (Utils.Clicked())
        {
            Ray ray = _camera.ScreenPointToRay(Utils.GetClickedPosition());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {                
                if (hit.transform.name == _sequence[_currentIndex].Name)
                {
                    _sequence[_currentIndex].StartAnimation(true);

                    if (_currentIndex == _sequenceIndex)
                    {
                        _bulleAdvice.text = WIN_MESSAGES[Random.Range(0, WIN_MESSAGES.Length)];

                        Note note = PlayNote(_sequence[_currentIndex]);
                        yield return new WaitForSeconds(note.GetLengthInSeconds());

                        EnableColliders(false);
                        _audioSource.clip = _yeaah;
                        _audioSource.Play();

                        for (int i = _errorCount; i < _lifeSprites.Length; i++)
                        {
                            _lifeSprites[i].GetComponent<Animator>().Play("Happy");
                        }

                        yield return new WaitForSeconds(2);

                        _step++;
                        _sequenceIndex++;
                        _currentScoreText.text = "" + (_sequenceIndex);

                        _currentIndex = 0;
                        _nextStep = true;
                    }
                    else
                    {                        
                        Note note = PlayNote(_sequence[_currentIndex]);
                        _currentIndex++;
                        yield return new WaitForSeconds(note.GetLengthInSeconds());                        
                    }

                    _sequence[_currentIndex].StopAnimation();
                }
                else
                {
                    EnableColliders(false);
                    _bulleAdvice.text = FAIL_MESSAGES[Random.Range(0, FAIL_MESSAGES.Length)];
                    _audioSource.clip = _booh;
                    _audioSource.Play();

                    yield return new WaitForSeconds(2);

                    if (_errorCount < MAX_ERROR)
                    {
                        _lifeSprites[_errorCount].GetComponent<Animator>().SetBool("died", true);
                        _errorCount++;
                    }
                    else
                    {
                        _canvasGameOver.enabled = true;
                        _gameOver = true;
                    }

                    _currentIndex = 0;
                    _nextStep = true;
                }
                
                yield return new WaitForSeconds(2);                
            }
        }
    }

    void EnableColliders(bool enable)
    {
        foreach(Instrument instrument in _sequence)
        {
            instrument.Collider.enabled = enable;
        }
        _panelArrows.SetActive(enable);
    }

    void AnimateInstruments(bool animate)
    {
        foreach(Instrument instrument in _sequence)
        {
            if (animate)
                instrument.StartAnimation(true);
            else
                instrument.StopAnimation();
        }
    }
}
