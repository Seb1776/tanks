using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Visible
    public enum WaveStages {Control, Buildup, Assault}
    [Header("Wave Properties")]
    public WaveStages currentStage;
    public List<Spawner> spawner = new List<Spawner>();
    public float controlStageDuration;
    public float buildupStageDuration;
    public float assaultStageDuration;
    public bool canSpawn;


    //Invisible
    public int limitLightHeavyEnemy;
    public int limitShieldEnemy;
    public int limitTaserEnemy;
    public int limitMedicEnemy;
    public int limitBulldozerLight;
    public int limitBulldozerMedium;
    public int limitBulldozerHeavy;
    public int currentLimitLightHeavyEnemy;
    public int currentLimitShieldEnemy;
    public int currentLimitTaserEnemy;
    public int currentLimitMedicEnemy;
    public int currentLimitBulldozerLight;
    public int currentLimitBulldozerMedium;
    public int currentLimitBulldozerHeavy;
    float currentControlStageDuration;
    float currentBuildupStageDuration;
    float currentAssaultStageDuration;
    Music music;

    void Start()
    {
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<Music>();

        currentControlStageDuration = controlStageDuration;
        currentBuildupStageDuration = buildupStageDuration;
        currentAssaultStageDuration = assaultStageDuration;
    }

    void Update()
    {
        WaveStagesBehaviour();

        if (canSpawn)
            foreach(Spawner spawn in spawner)
                spawn.canSpawn = true;
    }

    public void SetLimit(string faction)
    {
        switch (faction)
        {
            case "LightHeavy":
                currentLimitLightHeavyEnemy--;
            break;

            case "Shield":
                currentLimitShieldEnemy--;
            break;

            case "Taser":
                currentLimitTaserEnemy--;
            break;

            case "Medic":
                currentLimitMedicEnemy--;
            break;

            case "BulldozerLight":
                currentLimitBulldozerLight--;
            break;

            case "BulldozerMedium":
                currentLimitBulldozerMedium--;
            break;

            case "BulldozerHeavy":
                currentLimitBulldozerHeavy--;
            break;
        }
    }

    void WaveStagesBehaviour()
    {
        switch (currentStage)
        {
            case WaveStages.Control:
                if (currentControlStageDuration <= 0)
                {
                    currentControlStageDuration = controlStageDuration;
                    currentStage = WaveStages.Buildup;
                }

                else
                {
                    if (canSpawn)
                        canSpawn = false;

                    currentControlStageDuration -= Time.deltaTime;
                }

            break;

            case WaveStages.Buildup:
                if (currentBuildupStageDuration <= 0)
                {
                    currentBuildupStageDuration = buildupStageDuration;
                    currentStage = WaveStages.Assault;
                }

                else
                    currentBuildupStageDuration -= Time.deltaTime;
            break;

            case WaveStages.Assault:
                if (currentAssaultStageDuration <= 0)
                {
                    currentAssaultStageDuration = assaultStageDuration;
                    music.ResetMusic();
                    currentStage = WaveStages.Control;
                }

                else
                {
                    if (!canSpawn)
                        canSpawn = true;

                    currentAssaultStageDuration -= Time.deltaTime;
                }
            break;
        }
    }
}
