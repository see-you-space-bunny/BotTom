using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Widget.CardGame.Enums;

namespace Widget.CardGame.MatchEntities
{
    public struct SummonCard(short power,short health,CharacterStat cardArchetype)
    {
        internal short Power = power;
        internal short Health = health;
        internal CharacterStat CardArchetype = cardArchetype;
    }
}