using System.Collections.Generic;
using UnityEngine;
using DS_Environment;
public class INS_StarSpawner : UnityEngine.UI.HorizontalLayoutGroup
{
    private static GameObject levelStarPrefab;
    private static GameObject rankStarPrefab;

    private List<GameObject> activeStars = new();

    protected override void Awake()
    {
        base.Awake();

        if(levelStarPrefab == null)
            levelStarPrefab = Resources.Load<GameObject>(ResourcePath.levelStar);
        if(rankStarPrefab == null)
            rankStarPrefab = Resources.Load<GameObject>(ResourcePath.rankStar);
    }

    public void SetStars(Card card)
    {
        Despawn();
        Spawn(card.Info.level, card.Info.isXYZ());
    }

    private void Spawn(uint level, bool isRank = false)
    {
        if (!isRank) {
            childAlignment = TextAnchor.UpperRight;
            for (int i = 0; i < level; i++)
            {
                activeStars.Add(Instantiate(levelStarPrefab, transform));
            }
            SetLayoutHorizontal();
        }
        else
        {
            childAlignment = TextAnchor.UpperLeft;
            for (int i = 0; i < level; i++)
            {
                activeStars.Add(Instantiate(rankStarPrefab, transform));
            }
            SetLayoutHorizontal();
        }
    }
    private void Despawn()
    {
        foreach(GameObject star in activeStars)
        {
            Destroy(star);
        }
        activeStars.Clear();
    }

}
