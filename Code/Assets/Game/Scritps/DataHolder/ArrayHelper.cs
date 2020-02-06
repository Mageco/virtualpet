
using System;
using System.Collections;
using UnityEngine;

public class ArrayHelper
{
    // array init helper
    public static int[] Create(int count, int initialValue)
    {
        int[] list = new int[count];
        for (int i = 0; i < count; i++) list[i] = initialValue;
        return list;
    }

    // base types
    public static string[] Add(string n, string[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (string str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(string)) as string[];
    }

    public static string[] Remove(int index, string[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (string str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(string)) as string[];
    }

    public static bool[] Add(bool n, bool[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (bool str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(bool)) as bool[];
    }

    public static bool[] Remove(int index, bool[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (bool str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(bool)) as bool[];
    }

    public static int[] Add(int n, int[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (int str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(int)) as int[];
    }

    public static int[] Remove(int index, int[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (int str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(int)) as int[];
    }

    public static float[] Add(float n, float[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (float str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(float)) as float[];
    }

    public static float[] Remove(int index, float[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (float str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(float)) as float[];
    }

    public static Color[] Add(Color n, Color[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Color str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Color)) as Color[];
    }

    public static Color[] Remove(int index, Color[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Color str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Color)) as Color[];
    }

    public static Vector2[] Add(Vector2 n, Vector2[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Vector2 str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Vector2)) as Vector2[];
    }

    public static Vector2[] Remove(int index, Vector2[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Vector2 str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Vector2)) as Vector2[];
    }

    public static Vector3[] Add(Vector3 n, Vector3[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Vector3 str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Vector3)) as Vector3[];
    }

    public static Vector3[] Remove(int index, Vector3[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Vector3 str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Vector3)) as Vector3[];
    }



    // special types
    public static Language[] Add(Language n, Language[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Language str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Language)) as Language[];
    }

    public static Language[] Remove(int index, Language[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Language str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Language)) as Language[];
    }


    public static Item[] Add(Item n, Item[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Item str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Item)) as Item[];
    }

    public static Item[] Remove(int index, Item[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Item str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Item)) as Item[];
    }

    public static Skill[] Add(Skill n, Skill[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Skill str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Skill)) as Skill[];
    }

    public static Skill[] Remove(int index, Skill[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Skill str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Skill)) as Skill[];
    }


    public static Dialog[] Add(Dialog n, Dialog[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Dialog str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Dialog)) as Dialog[];
    }

    public static Dialog[] Remove(int index, Dialog[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Dialog str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Dialog)) as Dialog[];
    }

    public static Quest[] Remove(int index, Quest[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Quest str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Quest)) as Quest[];
    }

    public static Quest[] Add(Quest n, Quest[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Quest str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Quest)) as Quest[];
    }

    public static Pet[] Add(Pet n, Pet[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Pet str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Pet)) as Pet[];
    }

    public static Pet[] Remove(int index, Pet[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Pet str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Pet)) as Pet[];
    }

    public static Skin[] Add(Skin n, Skin[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Skin str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Skin)) as Skin[];
    }

    public static Skin[] Remove(int index, Skin[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Skin str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Skin)) as Skin[];
    }

    public static Achivement[] Add(Achivement n, Achivement[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Achivement str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Achivement)) as Achivement[];
    }

    public static Achivement[] Remove(int index, Achivement[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Achivement str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Achivement)) as Achivement[];
    }


    public static LanguageItem[] Add(LanguageItem n, LanguageItem[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (LanguageItem str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(LanguageItem)) as LanguageItem[];
    }

    public static LanguageItem[] Remove(int index, LanguageItem[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (LanguageItem str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(LanguageItem)) as LanguageItem[];
    }


    public static Transform[] Add(Transform n, Transform[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Transform str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Transform)) as Transform[];
    }

    public static Transform[] Remove(int index, Transform[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Transform str in list) tmp.Add(str);
        if (tmp[index] == null) tmp[index] = "";
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Transform)) as Transform[];
    }

    public static GameObject[] Add(GameObject n, GameObject[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (GameObject str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(GameObject)) as GameObject[];
    }

    public static GameObject[] Remove(int index, GameObject[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (GameObject str in list) tmp.Add(str);
        if (tmp[index] == null) tmp[index] = "";
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(GameObject)) as GameObject[];
    }

    public static AudioClip[] Add(AudioClip n, AudioClip[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (AudioClip str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(AudioClip)) as AudioClip[];
    }

    public static AudioClip[] Remove(int index, AudioClip[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (AudioClip str in list) tmp.Add(str);
        if (tmp[index] == null) tmp[index] = "";
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(AudioClip)) as AudioClip[];
    }



    public static SpriteRenderer[] Add(SpriteRenderer n, SpriteRenderer[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (SpriteRenderer str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(SpriteRenderer)) as SpriteRenderer[];
    }

    public static SpriteRenderer[] Remove(int index, SpriteRenderer[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (SpriteRenderer str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(SpriteRenderer)) as SpriteRenderer[];
    }

    public static SpriteRenderer[] Remove(SpriteRenderer c, SpriteRenderer[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (SpriteRenderer str in list) tmp.Add(str);
        tmp.Remove(c);
        return tmp.ToArray(typeof(SpriteRenderer)) as SpriteRenderer[];
    }

    public static Texture2D[] Add(Texture2D n, Texture2D[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Texture2D str in list) tmp.Add(str);
        tmp.Add(n);
        return tmp.ToArray(typeof(Texture2D)) as Texture2D[];
    }

    public static Texture2D[] Remove(int index, Texture2D[] list)
    {
        ArrayList tmp = new ArrayList();
        foreach (Texture2D str in list) tmp.Add(str);
        tmp.RemoveAt(index);
        return tmp.ToArray(typeof(Texture2D)) as Texture2D[];
    }
}
