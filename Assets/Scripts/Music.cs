using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Music : MonoBehaviour
{
    //Visible
    public enum Track
    {
        I_Will_Give_You_My_All_2017, 
        DONACDUM,
        Code_Silver,
        Profondo
    }

    [Header("Wave Music Properties")]
    public Track currentTrack;
    public Light2D backgroundLight;


    //Invisible
    string starter;
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
    float updateStep = .1f;
    int sampleDataLength = 1024 * 2;
    float currentUpdateTime = 0f;
    float clipLoudness;
    float[] clipSampleData;
    int fullLength;
    float currentSongLength;
    AudioSource source;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        source = GetComponent<AudioSource>();
        clipSampleData = new float[sampleDataLength];

        FindTracks();
    }

    void Update()
    {
        MusicBehaviour();
    }

    void FixedUpdate()
    {
        if (gameManager.currentStage == GameManager.WaveStages.Assault)
            VisualizeLights();
    }

    void VisualizeLights()
    {
        currentUpdateTime += Time.deltaTime;

        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            source.clip.GetData(clipSampleData, source.timeSamples);
            clipLoudness = 0f;

            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }

            clipLoudness /= sampleDataLength;
        }

        backgroundLight.intensity = clipLoudness * 2;

        /*if (backgroundLight.intensity > 1f)
            backgroundLight.intensity = 1f;*/
    }

    void FindTracks()
    {
        switch (currentTrack)
        {
            case Track.I_Will_Give_You_My_All_2017:
                starter = "diamond";
            break;

            case Track.DONACDUM:
                starter = "donacdum";
            break;

            case Track.Code_Silver:
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
                                    currentBuildupIniDuration = buildupIniDuration;
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
                        playedAssault = true;
                    }
                }
            break;
        }
    }
}
