using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAqua : MonsterBase
{

    public MonsterAqua(string name, string description, string spriteFile, int HP, int ATK, int DEF, int SPD)
    {
        this.name = name;
        this.description = description;
        this.spriteFile = spriteFile;
        this.baseHealth = HP;
        this.baseAttack = ATK;
        this.baseDefense = DEF;
        this.baseSpeed = SPD;
        this.baseType = BaseType.ACQUA;

        AddToMoveSet(new Attack(AttackType.SINGLE, "Liquidate", "All forms of mass compressed into the tiniest of particles", 3, 1f));
        AddToMoveSet(new Attack(AttackType.SINGLE, "Liquid Razor", "A deep incision with high pressurized water", 1, 1f));
        AddToMoveSet(new Attack(AttackType.SINGLE, "Hydro Cannon", "High velocity of water pressurized towards you", 2, 1f));
        AddToMoveSet(new Attack(AttackType.SINGLE, "Water Shuriken", "The arts of tranquil water combined the fury of the ninja", 1, 1f));
    }
}