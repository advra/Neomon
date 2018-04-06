using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class MonsterFungi : MonsterBase {

    public MonsterFungi(string name, string description, string spriteFile, int HP, float ATK, float DEF, float SPD)
    {
        this.name = name;
        this.description = description;
        this.spriteFile = spriteFile;
        this.baseHealth = HP;
        this.baseAttack = ATK;
        this.baseDefense = DEF;
        this.baseSpeed = SPD;
        this.baseType = BaseType.FUNGI;

        AddToMoveSet(new Attack(TargetArea.SINGLE, "Stem Slap", "The enemy body slams you with their stem", 3, 0, 1f, false));
        AddToMoveSet(new Attack(TargetArea.SINGLE, "Boomerang Cap", "The enemy throws their cap towards you", 2, 0, 1.25f, false));
        AddToMoveSet(new Attack(TargetArea.ALL, "Spore'n Rain", "Poisonous spore particles begin to fill the air", 2, 0, 2f, false));
    }
}
