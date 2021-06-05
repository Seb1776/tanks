using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;

public class Music : MonoBehaviour
{
    //Visible
    public enum Track
    {
        IWillGiveYouMyAll2017, 
        DONACDUM,
        CodeSilver2012,
        Profondo
    }

    [Header("Wave Music Properties")]
    public Track currentTrack;
    public Light2D backgroundLight;
    public float bpm;
    public bool exactVersion;
    public bool exploded;
    public AudioClip explosionSFX;


    //Invisible
    string starter;
    bool visualize;
    bool playedExplosionEffect;
    AudioClip control;
    AudioClip buildup;
    AudioClip assault;
    AudioClip controlIni;
    AudioClip buildupIni;
    AudioClip assaultIni;
    bool playedControl;
    bool playedBuildup;
    bool playedAssault;
    bool playedControlIni;
    bool playedBuildupIni;
    bool playedAssaultIni;
    float controlIniDuration;
    float currentControlIniDuration;
    float buildupIniDuration;
    float currentBuildupIniDuration;
    float assaultIniDuration;
    float currentAssaultIniDuration;
    float currentBPSDuration;
    [SerializeField]
    AudioSource explosionEffect;
    AudioSource source;
    AudioLowPassFilter lowPassFilter;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        source = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();

        lowPassFilter.cutoffFrequency = 350f;

        FindTracks();
    }

    void Update()
    {
        MusicBehaviour();

        if (visualize)
            VisualizeLights();
        else
        {
            if (backgroundLight.intensity < 1f)
                backgroundLight.intensity += Time.deltaTime;
            
            backgroundLight.color = Color.white;
        }

        if (exploded)
        {
            lowPassFilter.enabled = true;

            if (lowPassFilter.cutoffFrequency >= 12000f)
            {
                lowPassFilter.cutoffFrequency = 350f;
                lowPassFilter.enabled = false;
                explosionEffect.volume = 1f;
                explosionEffect.Stop();
                exploded = false;
                playedExplosionEffect = false;
            }

            else
            {
                lowPassFilter.cutoffFrequency += Time.deltaTime * 1000f;
                explosionEffect.volume -= Time.deltaTime / 10f;
            }
        }
    }

    public void RequestExplosionEffect()
    {
        if (!exploded)
            exploded = true;

        else
        {
            lowPassFilter.cutoffFrequency = 350f;
            explosionEffect.volume = 1f;
        }
        
        if (!playedExplosionEffect)
        {
            explosionEffect.clip = explosionSFX;
            explosionEffect.Play();
            playedExplosionEffect = true;
        }
    }

    void VisualizeLights()
    {
        float baseValue = Mathf.Cos(((Time.time * Mathf.PI) * (bpm / 60f)) % Mathf.PI);

        if (exactVersion)
            backgroundLight.intensity = Mathf.RoundToInt(Mathf.Abs(baseValue));
        else
            backgroundLight.intensity = Mathf.Abs(baseValue);

        backgroundLight.color = Color.red;
    }

    void FindTracks()
    {
        switch (currentTrack)
        {
            case Track.IWillGiveYouMyAll2017:
                starter = "diamond";
            break;

            case Track.DONACDUM:
                starter = "donacdum";
            break;

            case Track.CodeSilver2012:
                starter = "blood";
            break;

            case Track.Profondo:
                starter = "profondo";
            break;
        }

        controlIni = Resources.Load<AudioClip>("Music/" + starter + "_control_ini");
        control = Resources.Load<AudioClip>("Music/" + starter + "_control");
        buildupIni = Resources.Load<AudioClip>("Music/" + starter + "_buildup_ini");
        buildup = Resources.Load<AudioClip>("Music/" + starter + "_buildup");
        assaultIni = Resources.Load<AudioClip>("Music/" + starter + "_assault_ini");
        assault = Resources.Load<AudioClip>("Music/" + starter + "_assault");
        
        if (controlIni != null)
            controlIniDuration = currentControlIniDuration = controlIni.length;
        
        if (buildupIni != null)
            buildupIniDuration = currentBuildupIniDuration = buildupIni.length;
        
        if (assaultIni != null)
            assaultIniDuration = currentAssaultIniDuration = assaultIni.length;
    }

    public void ResetMusic()
    {
        playedControl = false;
        playedBuildup = false;
        playedAssault = false;
        playedControlIni = false;
        playedBuildupIni = false;
        playedAssaultIni = false;
        currentControlIniDuration = controlIniDuration;
        currentBuildupIniDuration = buildupIniDuration;
    }

    void MusicBehaviour()
    {
        switch (gameManager.currentStage)
        {
            case GameManager.WaveStages.Control:
                if (controlIni != null)
                {
                    if (!playedControlIni)
                    {
                        source.clip = controlIni;
                        source.loop = false;
                        source.Play();
                        visualize = false;
                        playedControlIni = true;
                    }

                    else
                    {   
                        if (!playedControl)
                        {  
                            if (currentControlIniDuration <= 0)
                            {
                                if (!playedControl)
                                {
                                    source.clip = control;
                                    source.loop = true;
                                    source.Play();
                                    currentControlIniDuration = controlIniDuration;
                                    playedControl = true;
                                }
                            }

                            else
                                currentControlIniDuration -= Time.deltaTime;
                        }
                    }
                }

                else
                {
                    if (!playedControl)
                    {
                        source.clip = control;
                        source.loop = true;
                        source.Play();
                        currentControlIniDuration = controlIniDuration;
                        visualize = false;
                        playedControl = true;
                    }
                }
                
            break;

            case GameManager.WaveStages.Buildup:
                if (buildupIni != null)
                {
                    if (!playedBuildupIni)
                    {
                        source.clip = buildupIni;
                        source.loop = false;
                        source.Play();
                        playedBuildupIni = true;
                    }

                    else
                    {   
                        if (!playedBuildup)
                        {
                            if (currentBuildupIniDuration <= 0)
                            {
                                if (!playedBuildup)
                                {
                                    source.clip = buildup;
                                    source.loop = true;
                                    source.Play();
                                    playedBuildup = true;
                                }
                            }

                            else
                                currentBuildupIniDuration -= Time.deltaTime;
                        }
                    }
                }

                else
                {
                    if (!playedBuildup)
                    {
                        source.clip = buildup;
                        source.loop = true;
                        source.Play();
                        currentBuildupIniDuration = buildupIniDuration;
                        playedBuildup = true;
                    }
                }
            break;

            case GameManager.WaveStages.Assault:
                if (assaultIni != null)
                {
                    if (!playedAssaultIni)
                    {
                        source.clip = assaultIni;
                        source.loop = false;
                        source.Play();
                        playedAssaultIni = true;
                    }

                    else
                    {   
                        if (!playedAssault)
                        {  
                            if (currentAssaultIniDuration <= 0)
                            {
                                if (!playedAssault)
                                {
                                    source.clip = assault;
                                    source.loop = true;
                                    source.Play();
                                    currentAssaultIniDuration = assaultIniDuration;
                                    visualize = true;
                                    playedAssault = true;
                                }
                            }

                            else
                                currentAssaultIniDuration -= Time.deltaTime;
                        }
                    }
                }

                else
                {
                    if (!playedAssault)
                    {
                        source.clip = assault;
                        source.loop = true;
                        source.Play();
                        visualize = true;
                        playedAssault = true;
                    }
                }
            break;
        }
    }
}
