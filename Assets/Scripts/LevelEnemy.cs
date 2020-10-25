using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LevelEnemy
{
    public int Id { get; private set; }

    public string Prefab { get; private set; }

    public int Probability { get; private set; }

    public LevelEnemy(int id, string prefab, int proba)
    {
        Id = id;
        Prefab = prefab;
        Probability = proba;
    }
}
