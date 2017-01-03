using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TamtamInstrument : BlindTestInstrument {

    private Button _playButton;
    private Sprite _playSprite;
    private Sprite _pauseSprite;

    // attribut indiquant si le tamtam est l'intru
    public bool isIntrus = false;
    public static int tamtamId = 1;
    public Extract  extrait { get; set; }
    public Button play
    {
        get { return this._playButton; }
        set
        {
            this._playButton = value;
            this.initButtonListener();
        }
    }
    public AudioSource source { get; set; }


    public TamtamInstrument(bool isIntru) : base(Instrument.TAMTAM)
    {
        this.isIntrus = isIntru;
        this.Instrument.Name += tamtamId;
        tamtamId++;
    }

    public TamtamInstrument(bool isIntru, Extract extract) : this(isIntru)
    {
        this.extrait = extract;
    }

    public TamtamInstrument(bool isIntru, Extract extract, Light spotlight) : this(isIntru, extract)
    {
        this.SpotLight = spotlight;
    }
    public TamtamInstrument(bool isIntru, Extract extract, Light spotlight, Button play) : this(isIntru, extract, spotlight)
    {
        this.play = play;
    }



    


    /// <summary>
    /// Méthode dérivée de la classe BlindTestInstrument
    /// </summary>
    /// <param name="affirmative"></param>
    /// <returns></returns>

    public new bool ToggleLightAnswerFor(bool affirmative)
    {
        if (isIntrus && this.isLit)
        {
            this.SpotLight.color = Color.green;
            return true;
        }
        else if (!isIntrus && this.isLit)
        {
            this.SpotLight.color = Color.red;
            return false;
        }
        else if (isIntrus && !this.isLit)
        {
            this.ToggleLight();
            this.SpotLight.color = Color.green;
            return false;
        }
        else
            return true;
    }

    public void PlayExtract()
    {
        if (_playSprite == null || _pauseSprite == null)
        {
            initSprites();
        }

        if (source.isPlaying)
        {
            play.image.sprite = _playSprite;
            source.Stop();
            Instrument.EnableParticles(false);
        }
        else
        {
            play.image.sprite = _pauseSprite;
            source.clip = this.extrait.Clip;
            source.Play();
            Instrument.EnableParticles(true);

        }

    }

    private void initSprites()
    {
        _playSprite = Resources.Load<Sprite>("Sprites/play_button");
        _pauseSprite = Resources.Load<Sprite>("Sprites/pause");
    }

    public void initButtonListener()
    {
        initSprites();
        this.play.onClick.AddListener(() =>
        {
            
            this.PlayExtract();
        });
    }
  
}
