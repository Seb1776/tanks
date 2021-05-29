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
    public enum Difficulty {Normal, Hard, VeryHard, Overkill, Mayhem,
    DeathWish, DeathSentence, OneDown}
    public Difficulty currentDifficulty;
    public int score;
    public List<DifficultySettings> difficultySettings = new List<DifficultySettings>();
    public List<Spawner> spawner = new List<Spawner>();
    public Vector2 randControlStageDuration;
    public Vector2 randBuildupStageDuration;
    public Vector2 randAssaultStageDuration;
    public bool canSpawn;
    public bool settedTime;
    public int shapeLimit;
    public int winnedWaves;
    public float timeBtwShapeSpawn;
    public float shapeSpawnRadius;
    public string[] waveIncomingUnits;
    public bool spawnedTurret;
    [Range(1, 4)]
    public int[] wavesRequiredToProgress;
    public List<GameObject> shapes = new List<GameObject>();
    public Transform shapeSpawner;
    public Transform shapeSpawnerTmp;
    public Volume postProcessing;

    [Header("UI")]
    public Animator assaultBanner;
    public Animator enemyBanner;
    public Animator difficultyBanner;
    public Color assaultBannerOverdue;
    public Color cornerColorOverdue;
    public Image assaultBannerBack;
    public Text assaultPhaseText;
    public Text clockText;
    public Text difficultyName;
    public Image[] corners;
    public Text scoreText;
    public Text enemyBannerFaction;
    public GameObject crossHair;


    //Invisible
    float clockSeconds;
    float clockMinutes;
    float clockHours;
    float controlStageDuration;
    float buildupStageDuration;
    float assaultStageDuration;
    bool engaged;
    List<string> difficulties = new List<string>();
    Color originalAssaultBannerColor;
    Color originalCornerColor;
    public int limitLightHeavyEnemy;
    public int limitShieldEnemy;
    public int limitSniperEnemy;
    public int limitTaserEnemy;
    public int limitMedicEnemy;
    public int limitSmokerEnemy;
    public int limitBulldozerLight;
    public int limitBulldozerMedium;
    public int limitBulldozerHeavy;
    public int limitTurret;
    public int currentLimitLightHeavyEnemy;
    public int currentLimitShieldEnemy;
    public int currentLimitSniperEnemy;
    public int currentLimitTaserEnemy;
    public int currentLimitMedicEnemy;
    public int currentLimitSmokerEnemy;
    public int currentLimitBulldozerLight;
    public int currentLimitBulldozerMedium;
    public int currentLimitBulldozerHeavy;
    public int currentLimitTurret;
    int currentShapeLimit;
    int currentWinnedWaves;
    int enemyFactionIndex;
    float currentTimeBtwSpawns;
    float currentControlStageDuration;
    float currentBuildupStageDuration;
    float currentAssaultStageDuration;
    float colorSmoothTime;
    public List<float> originalSpawnTimes = new List<float>();
    string totalClocks;
    Transform tmpPos;
    Tank player;
    Music music;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Tank>();
        music = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<Music>();

        currentControlStageDuration = controlStageDuration = Random.Range(randControlStageDuration.x, randControlStageDuration.y);
        currentBuildupStageDuration = buildupStageDuration = Random.Range(randBuildupStageDuration.x, randBuildupStageDuration.y);
        currentAssaultStageDuration = assaultStageDuration = Random.Range(randAssaultStageDuration.x, randAssaultStageDuration.y);

        originalAssaultBannerColor = assaultBannerBack.color;

        currentTimeBtwSpawns = timeBtwShapeSpawn;

        foreach (Image corner in corners)
            originalCornerColor = corner.color;
        
        CheckDifficulty();

        if (currentDifficulty != Difficulty.OneDown)
            ShowWave();
        
        Cursor.visible = false;
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
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crossHair.transform.position = mousePos;
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
            assaultPhaseText.text = "/// ASSAULT ///";

            foreach(Image corner in corners)
                corner.color = originalCornerColor;
        }
    }

    void CheckDifficulty()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Hard:
                enemyFactionIndex = 1;

                foreach (Spawner spawn in spawner)
                {
                    spawn.shieldEnemy = true;
                    difficultySettings[0].ApplySettings(this);
                }
                
                difficultyName.text = "HARD";
                difficultyBanner.SetBool("hard", true);
            break;

            case Difficulty.VeryHard:
                enemyFactionIndex = 2;

                foreach (Spawner spawn in spawner)
                {
                    spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    difficultySettings[1].ApplySettings(this);
                }
                
                difficultyName.text = "VERY HARD";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
            break;

            case Difficulty.Overkill:
                enemyFactionIndex = 3;

                foreach (Spawner spawn in spawner)
                {
                    spawn.medicEnemy = spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    difficultySettings[2].ApplySettings(this);
                }
                
                difficultyName.text = "OVERKILL";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
                difficultyBanner.SetBool("overkill", true);
            break;

            case Difficulty.Mayhem:
                enemyFactionIndex = 4;

                foreach (Spawner spawn in spawner)
                {
                    spawn.smokerEnemy = spawn.medicEnemy = spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    spawn.turret = true;
                    difficultySettings[3].ApplySettings(this);
                }

                difficultyName.text = "MAYHEM";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
                difficultyBanner.SetBool("overkill", true);
                difficultyBanner.SetBool("mayhem", true);
            break;

            case Difficulty.DeathWish:
                enemyFactionIndex = 5;

                foreach (Spawner spawn in spawner)
                {
                    spawn.bulldozerLight = spawn.smokerEnemy = spawn.medicEnemy = spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    difficultySettings[4].ApplySettings(this);
                }

                difficultyName.text = "DEATH WISH";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
                difficultyBanner.SetBool("overkill", true);
                difficultyBanner.SetBool("mayhem", true);
                difficultyBanner.SetBool("deathwish", true);
            break;

            case Difficulty.DeathSentence:
                enemyFactionIndex = 6;

                foreach (Spawner spawn in spawner)
                {
                    spawn.bulldozerHeavy = spawn.bulldozerMedium = spawn.bulldozerLight = spawn.smokerEnemy = spawn.medicEnemy = spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    difficultySettings[5].ApplySettings(this);
                }
                
                difficultyName.text = "DEATH SENTENCE";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
                difficultyBanner.SetBool("overkill", true);
                difficultyBanner.SetBool("mayhem", true);
                difficultyBanner.SetBool("deathwish", true);
                difficultyBanner.SetBool("deathsentence", true);
            break;

            case Difficulty.OneDown:
                enemyFactionIndex = 7;

                foreach (Spawner spawn in spawner)
                {
                    spawn.bulldozerHeavy = spawn.bulldozerMedium = spawn.bulldozerLight = spawn.smokerEnemy = spawn.medicEnemy = spawn.shieldEnemy = spawn.sniperEnemy = spawn.taserEnemy = true;
                    difficultySettings[6].ApplySettings(this);
                }

                difficultyName.text = "ONE DOWN";
                difficultyBanner.SetBool("hard", true);
                difficultyBanner.SetBool("veryhard", true);
                difficultyBanner.SetBool("overkill", true);
                difficultyBanner.SetBool("mayhem", true);
                difficultyBanner.SetBool("deathwish", true);
                difficultyBanner.SetBool("deathsentence", true);
                difficultyBanner.SetBool("onedown", true);

                ShowWave();
                engaged = true;
            break;
        }

        foreach (Spawner spawn in spawner)
            spawn.CountLimit();
    }

    void ShowWave()
    {   
        if (currentDifficulty != Difficulty.OneDown)
        {   
            if (currentDifficulty != Difficulty.DeathSentence)
                enemyBannerFaction.text = "/// " + waveIncomingUnits[enemyFactionIndex] + " UNITS INCOMING ///";
            else
                enemyBannerFaction.text = "/// " + waveIncomingUnits[enemyFactionIndex] + " ///";

            StartCoroutine(HideWave(5.9f));
            enemyBanner.SetBool("show", true);
        }

        else
        {
            enemyBannerFaction.text = "/// ONE DOWN ENGAGED, NO MORE HEALTH REGEN FROM KILLS ///";
            StartCoroutine(HideWave(5.9f));
            enemyBanner.SetBool("show", true);
        }
    }

    IEnumerator HideWave(float duration)
    {
        yield return new WaitForSeconds(duration);
        enemyBanner.SetBool("show", false);
    }

    void WinWave()
    {
        winnedWaves++;

        Debug.Log("called WinWave");
        
        switch (currentDifficulty.ToString())
        {
            case "Normal":
                if (currentWinnedWaves <= wavesRequiredToProgress[0])
                    currentWinnedWaves++;
                
                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                        spawn.shieldEnemy = true;
                    
                    enemyFactionIndex++;
                
                    ShowWave();

                    difficultyName.text = "HARD";
                    difficultyBanner.SetBool("hard", true);
                    difficultySettings[0].ApplySettings(this);
                    currentDifficulty = Difficulty.Hard;
                }
            break;

            case "Hard":
                if (currentWinnedWaves <= wavesRequiredToProgress[1])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                    {
                        spawn.sniperEnemy = true;
                        spawn.taserEnemy = true;
                    }
                    
                    enemyFactionIndex++;

                    ShowWave();

                    difficultyName.text = "VERY HARD";
                    difficultyBanner.SetBool("veryhard", true);
                    difficultySettings[1].ApplySettings(this);
                    currentDifficulty = Difficulty.VeryHard;
                }
            break;

            case "VeryHard":
                if (currentWinnedWaves <= wavesRequiredToProgress[2])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                        spawn.medicEnemy = true;
                    
                    enemyFactionIndex++;

                    ShowWave();

                    difficultyName.text = "OVERKILL";
                    difficultyBanner.SetBool("overkill", true);
                    difficultySettings[2].ApplySettings(this);
                    currentDifficulty = Difficulty.Overkill;
                }
            break;

            case "Overkill":
                if (currentWinnedWaves <= wavesRequiredToProgress[3])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                        spawn.smokerEnemy = true;
                    
                    enemyFactionIndex++;

                    ShowWave();
                    
                    difficultyName.text = "MAYHEM";
                    difficultyBanner.SetBool("mayhem", true);
                    difficultySettings[3].ApplySettings(this);
                    currentDifficulty = Difficulty.Mayhem;
                }
            break;

            case "Mayhem":
                if (currentWinnedWaves <= wavesRequiredToProgress[4])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                    {
                        spawn.bulldozerLight = true;
                        spawn.turret = true;
                    }

                    enemyFactionIndex++;

                    ShowWave();

                    difficultyName.text = "DEATH WISH";
                    difficultyBanner.SetBool("deathwish", true);
                    difficultySettings[4].ApplySettings(this);
                    currentDifficulty = Difficulty.DeathWish;
                }
            break;

            case "DeathWish":
                if (currentWinnedWaves < wavesRequiredToProgress[5])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                        spawn.bulldozerMedium = true;

                    enemyFactionIndex++;
                    ShowWave();

                    difficultyName.text = "DEATH SENTENCE";
                    difficultyBanner.SetBool("deathsentence", true);
                    difficultySettings[5].ApplySettings(this);
                    currentDifficulty = Difficulty.DeathSentence;
                }
            break;

            case "DeathSentence":
                if (currentWinnedWaves <= wavesRequiredToProgress[6])
                    currentWinnedWaves++;

                else
                {
                    currentWinnedWaves = 0;

                    foreach (Spawner spawn in spawner)
                        spawn.bulldozerHeavy = true;
                    
                    enemyFactionIndex++;

                    ShowWave();

                    difficultyName.text = "ONE DOWN";
                    difficultyBanner.SetBool("onedown", true);
                    difficultySettings[6].ApplySettings(this);
                    currentDifficulty = Difficulty.OneDown;
                }
            break;

            case "OneDown":
                if(!engaged)
                {
                    ShowWave();
                    engaged = true;
                }

                currentWinnedWaves++;
            break;
        }

        if (currentDifficulty != Difficulty.OneDown)
            foreach (Spawner spawn in spawner)
                spawn.CountLimit();
        
        originalSpawnTimes.Clear();

        foreach (Spawner spawn in spawner)
            spawn.ResetTimers();
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

                if (currentLimitLightHeavyEnemy < 0)
                    currentLimitLightHeavyEnemy = 0;
            break;

            case "Shield":
                currentLimitShieldEnemy--;

                if (currentLimitShieldEnemy < 0)
                    currentLimitShieldEnemy = 0;
            break;

            case "Sniper":
                currentLimitSniperEnemy--;

                if (currentLimitSniperEnemy < 0)
                    currentLimitSniperEnemy = 0;
            break;

            case "Taser":
                currentLimitTaserEnemy--;

                if (currentLimitTaserEnemy < 0)
                    currentLimitTaserEnemy = 0;
            break;

            case "Medic":
                currentLimitMedicEnemy--;

                if (currentLimitMedicEnemy < 0)
                    currentLimitMedicEnemy = 0;
            break;

            case "Smoker":
                currentLimitSmokerEnemy--;

                if (currentLimitSmokerEnemy < 0)
                    currentLimitSmokerEnemy = 0;
            break;

            case "BulldozerLight":
                currentLimitBulldozerLight--;

                if (currentLimitBulldozerLight < 0)
                    currentLimitBulldozerLight = 0;
            break;

            case "BulldozerMedium":
                currentLimitBulldozerMedium--;

                if (currentLimitBulldozerMedium < 0)
                    currentLimitBulldozerMedium = 0;
            break;

            case "BulldozerHeavy":
                currentLimitBulldozerHeavy--;

                if (currentLimitBulldozerHeavy < 0)
                    currentLimitBulldozerHeavy = 0;
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
                    settedTime = false;
                }

                else
                {
                    currentControlStageDuration -= Time.deltaTime;

                    if (!settedTime)
                    {
                        for (int i = 0; i < originalSpawnTimes.Count; i++)
                            spawner[i].currentTimeBtwSpawns = originalSpawnTimes[i] * 4f;
                        
                        settedTime = true;
                    }
                }

            break;

            case WaveStages.Buildup:
                if (currentBuildupStageDuration <= 0)
                {
                    currentBuildupStageDuration = buildupStageDuration;
                    currentStage = WaveStages.Assault;
                    settedTime = false;
                }

                else
                {
                    currentBuildupStageDuration -= Time.deltaTime;

                    if (!settedTime)
                    {
                        for (int i = 0; i < originalSpawnTimes.Count; i++)
                            spawner[i].currentTimeBtwSpawns = originalSpawnTimes[i] * 2f;
                        
                        settedTime = true;
                    }
                }
            break;

            case WaveStages.Assault:
                if (currentAssaultStageDuration <= 0)
                {
                    music.ResetMusic();
                    settedTime = false;
                    currentStage = WaveStages.Control;
                    currentAssaultStageDuration = assaultStageDuration;
                    Debug.Log(currentAssaultStageDuration);
                    Debug.Log(assaultStageDuration);
                    WinWave();
                }

                else
                {
                    currentAssaultStageDuration -= Time.deltaTime;

                    if (!settedTime)
                    {
                        for (int i = 0; i < originalSpawnTimes.Count; i++)
                            spawner[i].currentTimeBtwSpawns = originalSpawnTimes[i];
                        
                        settedTime = true;
                    }
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
