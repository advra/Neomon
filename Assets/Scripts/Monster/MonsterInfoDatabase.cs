using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class serves as a database for maintaining the monster class

public static class MonsterInfoDatabase
{

    //this will store all the monsters we generate
    //Remember, arrays are mutable and need to be resized if needed
    public static List<Monster> monsters = new List<Monster>();

    private static bool populated = false;

    public static bool IsPopulated
    {
        get { return populated; }
    }

    //Create the table of monsters
    //could insert this int a different thread in future if load is too long
    public static void Populate()
    {
        //Here we create a monster defining its name, file and stats
        // (int id, string type, string spriteFile, int base HP, Speed, Attack, Defense) int level                                
        Monster squid = new Monster(new MonsterAqua("Squidra", "An enemy with no armor. Shouldn't be difficult to defeat", "squid", 15, 5, 3, 4), 1);
        monsters.Add(squid);

        Monster toxicMushroom = new Monster(new MonsterFungi("Toxic Mushroom", "A purple poisonous mushroom. Definitely dangerous with those pair of glowing eyes!", "toxicmush", 25, 6, 2, 4.8f), 1);
        monsters.Add(toxicMushroom);

        Monster kidMushroom = new Monster(new MonsterFungi("Mush-Kid", "Both an odd mushroom and an odd kid...", "kidmush", 10, 7, 3, 3), 1);
        monsters.Add(kidMushroom);

        Monster flameWick = new Monster(new MonsterPyro("Flamed Wick", "Hes definitely quite hot to the touch.","wick", 20, 5, 5, 5), 1);
        monsters.Add(flameWick);

        Debug.Log("Length of MonsterDatabaseInfo[] is " + monsters.Count);

        populated = true;
    }

    //using the populated table as reference, spawn them then add them

}
