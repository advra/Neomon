using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class serves as a database for maintaining the monster class

public static class MonsterInfoDatabase
{

    //this will store all the monsters we generate
    //Remember, arrays are mutable and need to be resized if needed
    public static Monster[] monsters = new Monster[10];

    private static int index = 0;
    private static bool populated = false;

    public static bool IsPopulated
    {
        get { return populated; }
    }

    //Create the table of monsters
    public static void Populate()
    {
        //Here we create a monster defining its name, file and stats
        // (int id, string type, string spriteFile, int base HP, Speed, Attack, Defense) int level                                
        Monster squid = new Monster(new MonsterInfo(1, "Squidra", "mon_squid", 15, 5, 3, 4), 1);
        //Monster squid = new Monster("Squidra", "mon_squid");
        monsters[0] = squid;

        Monster toxicMushroom = new Monster(new MonsterInfo(1, "Toxic Mushroom", "mon_toxicmush", 25, 6, 2, 5), 1);
        //Monster toxicMushroom = new Monster("Toxic Mushroom", "mon_toxicmush");
        //toxicMushroom.SetBaseAttributes(25, 6, 2, 5);
        monsters[1] = toxicMushroom;

        Monster kidMushroom = new Monster(new MonsterInfo(1, "Mush-Kid", "mon_kidmush", 10, 7, 3, 3), 1);
        //Monster toxicMushroom = new Monster("Toxic Mushroom", "mon_toxicmush");
        //toxicMushroom.SetBaseAttributes(25, 6, 2, 5);
        monsters[2] = kidMushroom;
    }

    //using the populated table as reference, spawn them then add them

}
