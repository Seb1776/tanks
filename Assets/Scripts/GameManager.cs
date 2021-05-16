using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    //Visible
    public enum WaveStages {Control, Buildup, Assault}
    [Header("Wave Properties")]
    public WaveStages currentStage;
    public enum Difficulty {Easy, Normal, Hard, VeryHard, Overkill, Mayhem,
    DeathWish, DeathSentence}
    public Difficulty currentDifficulty;
    public int score;
    public List<Spawner> spawner = new List<Spawner>();
    public float controlStageDuration;
    public float buildupStageDuration;
    public float assaultStageDuration;
    public bool canSpawn;
    public int shapeLimit;
    public float timeBtwShapeSpawn;
    public float shapeSpawnRadius;
    public List<GameObject> shapes = new List<GameObject>();
    public Transform shapeSpawner;
    public Transform shapeSpawnerTmp;
    public Volume postProcessing;

    [Header("UI")]
    public Animator assaultBanner;
    public Color assaultBannerOverdue;
    public Color cornerColorOverdue;
    public Image assaultBannerBack;
    public Text assaultPhaseText;
    public Text clockText;
    public Image[] corners;
    public Text scoreText;


    //Invisible
    float clockSeconds;
    float clockMinutes;
    float clockHours;
    Color originalAssaultBannerColor;
    Color originalCornerColor;
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
    int currentShapeLimit;
    float currentTimeBtwSpawns;
    float currentControlStageDuration;
    float currentBuildupStageDuration;
    float currentAssaultStageDuration;
    string totalClocks;
    Transform tmpPos;
    Tank player;
    Music music;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Tank>();
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<Music>();

        currentControlStageDuration = controlStageDuration;
        currentBuildupStageDuration = buildupStageDuration;
        currentAssaultStageDuration = assaultStageDuration;

        originalAssaultBannerColor = assaultBannerBack.color;

        currentTimeBtwSpawns = timeBtwShapeSpawn;

        foreach (Image corner in corners)
            originalCornerColor = corner.color;
    }

    void Update()
    {
        WaveStagesBehaviour();
        UI();
        Clock();
        SpawnShapes();

        if (canSpawn)
            foreach(Spawner spawn in spawner)
                spawn.canSpawn = true;
        else
            foreach(Spawner spawn in spawner)
                spawn.canSpawn = false;
    }

    void UI()
    {
        scoreText.text = score.ToString("#,#000000000");

        if (currentStage == WaveStages.Control || currentStage == WaveStages.Buildup)
            assaultBanner.SetBool("show", false);
        else
            assaultBanner.SetBool("show", true);
        
        if (currentAssaultStageDuration <= 15)
        {
            assaultBannerBack.color = assaultPhaseText.color = assaultBannerOverdue;
            assaultPhaseText.text = "/// OVERDUE ///";

            foreach(Image corner in corners)
                corner.color = cornerColorOverdue;
        }

        else
        {
            assaultBannerBack.color = assaultPhaseText.color = originalAssaultBannerColor;

            foreach(Image corner in corners)
                corner.color = originalCornerColor;
        }
    }

    void SpawnShapes()
    {   
        if (shapes.Count != 0 || shapes != null)
        {
            if (currentShapeLimit < shapeLimit)
            {
                if (currentTimeBtwSpawns <= 0)
                {
                    int randInx = Random.Range(0, shapes.Count);

                    shapeSpawnerTmp.position = Random.insideUnitCircle * shapeSpawnRadius;
                    GameObject tmp = Instantiate(shapes[randInx], shapeSpawnerTmp);
                    tmp.transform.parent = null;
                    currentShapeLimit++;
                    currentTimeBtwSpawns = timeBtwShapeSpawn;
                }

                else
                    currentTimeBtwSpawns -= Time.deltaTime;
            }
        }
    }

    public void DestroyShape(GameObject shape, int _score)
    {
        score += _score;
        currentShapeLimit--;
        Destroy(shape);
    }

    void Clock()
    {
        clockSeconds += Time.deltaTime;

        if (clockSeconds >= 59)
        {
            clockMinutes++;
            clockSeconds = 0;

            if (clockMinutes >= 59)
            {
                clockMinutes = 0;

                if (clockHours < 99)
                    clockHours++;
            }
        }

        totalClocks = clockHours.ToString("00") + ":" + clockMinutes.ToString("00") + ":" + clockSeconds.ToString("00");
        clockText.text = totalClocks;
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
                    canSpawn = false;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(shapeSpawner.position, shapeSpawnRadius);
    }
}
