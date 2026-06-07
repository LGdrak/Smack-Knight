using UnityEngine;

[System.Serializable]
public class SaveFile
{

    public bool load;
    public int round;
    public int currentHealth;
    public int souls;
    public int[] upgrades;
    public int[] position;

    public SaveFile(bool _load, int _round,int  _currentHealth, int _souls, GameObject _equipped)
    {
        load = _load;
        round = _round;
        currentHealth = _currentHealth;
        souls = _souls;

        upgrades = new int[_equipped.transform.childCount];

        for (int i = 0; i < _equipped.transform.childCount; i++)
        {
            upgrades[i] = _equipped.transform.GetChild(i).gameObject.GetComponent<PowerUp>().index;
        }

        position = new int[3];

        position[0] = 0;
        position[1] = 15;
        position[2] = 0;
    }
}
