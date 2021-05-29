using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings : MonoBehaviour
{
    public int limitLightHeavyEnemy;
    public int limitShieldEnemy;
    public int limitSniperEnemy;
    public int limitTaserEnemy;
    public int limitMedicEnemy;
    public int limitSmokerEnemy;
    public int limitBulldozerLight;
    public int limitBulldozerMedium;
    public int limitBulldozerHeavy;

    public void ApplySettings(GameManager big)
    {
        big.limitLightHeavyEnemy = limitLightHeavyEnemy;
        big.limitShieldEnemy = limitShieldEnemy;
        big.limitSniperEnemy = limitSniperEnemy;
        big.limitTaserEnemy = limitTaserEnemy;
        big.limitMedicEnemy = limitMedicEnemy;
        big.limitSmokerEnemy = limitSmokerEnemy;
        big.limitBulldozerLight = limitBulldozerLight;
        big.limitBulldozerMedium = limitBulldozerMedium;
        big.limitBulldozerHeavy = limitBulldozerHeavy;
    }
}
