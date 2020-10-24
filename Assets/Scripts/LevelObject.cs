using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LevelObject
{
    public int Id { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public string Prefab { get; private set; }

    public int Probability { get; private set; }

    public LevelObject(int id, string prefab, int width, int height, int proba)
    {
        Id = id;
        Prefab = prefab;
        Width = width;
        Height = height;
        Probability = proba;
    }
}
