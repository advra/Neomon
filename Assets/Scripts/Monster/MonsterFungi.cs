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

        AddToMoveSet(new Attack(AttackType.SINGLE, "Stem Slap", "The enemy body slams you with their stem", 3, 1f, false));
        AddToMoveSet(new Attack(AttackType.SINGLE, "Boomerang Cap", "The enemy throws their cap towards you", 2, 1.25f, false));
        AddToMoveSet(new Attack(AttackType.ALL, "Spore'n Rain", "Poisonous spore particles begin to fill the air", 2, 2f, false));
    }
}
