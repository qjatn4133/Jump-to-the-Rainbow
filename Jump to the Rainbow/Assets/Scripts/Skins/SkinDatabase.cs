using UnityEngine;
using AssetBundles;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This allows us to store a database of all characters currently in the bundles, indexed by name.
/// </summary>
public class SkinDatabase
{
    static protected Dictionary<string, Skin> m_SkinsDict;
    static public Dictionary<string, Skin> dictionary { get { return m_SkinsDict; } }

    static protected List<string> m_skinNameList = new List<string>();
    static public List<string> skinNameList { get { return m_skinNameList; } }

    static protected List<string> overlapCharNameList = new List<string>();
    static protected List<string> m_charNameList = new List<string>();
    static public List<string> charNameList { get { return m_charNameList; } }

    static protected bool m_Loaded = false;
    static public bool loaded { get { return m_Loaded; } }

    static public Skin GetSkin(string type)
    {
        Skin s;
        if (m_SkinsDict == null || !m_SkinsDict.TryGetValue(type, out s))
            return null;

        return s;
    }

    static public IEnumerator LoadDatabase(List<string> packages)
    {
        if (m_SkinsDict == null)
        {
            m_SkinsDict = new Dictionary<string, Skin>();

            foreach (string s in packages)
            {
                AssetBundleLoadAssetOperation op = AssetBundleManager.LoadAssetAsync(s, "skin", typeof(GameObject));
                yield return CoroutineHandler.StartStaticCoroutine(op);

                Skin c = op.GetAsset<GameObject>().GetComponent<Skin>();
                if (c != null)
                {
                    m_SkinsDict.Add(c.skinName, c);
                    m_skinNameList.Add(c.skinName);
                    overlapCharNameList.Add(c.characterName);
                }
            }

            for(int i = 0; i < overlapCharNameList.Count; i++)
            {
                if (!m_charNameList.Contains(overlapCharNameList[i]))
                    m_charNameList.Add(overlapCharNameList[i]);
            }

            m_Loaded = true;
        }
    }
}
