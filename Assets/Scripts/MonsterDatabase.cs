using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class serves as a database for maintaining the monster class

public static class MonsterDatabase
{

    //this will store all the monsters we generate
    //Remember, arrays are mutable and need to be resized if needed
    public static Monster[] monsters = new Monster[10];
    //Element 0 and 1 are resereved for Player and Enemy monster respectively
    private static int index = 0;
    private static bool populated = false;

    public static bool IsPopulated
    {
        get { return populated; }
    }

    //Create the table of monsters
    public static void Populate()
    {
        //define player and enemy container classes
        Monster player = new Monster("Player", "p");
        monsters[0] = player;
        Monster enemy = new Monster("Enemy", "e");
        monsters[1] = player;

        //Here we create a monster defining its name, file and stats
        Monster squid = new Monster("Squidra", "mon_squid");
        squid.SetBaseAttributes(10, 5, 2, 3);
        monsters[2] = squid;

        Monster toxicMushroom = new Monster("Toxic Mushroom", "mon_toxicmush");
        toxicMushroom.SetBaseAttributes(25, 6, 2, 5);
        monsters[3] = toxicMushroom;
    }

}
