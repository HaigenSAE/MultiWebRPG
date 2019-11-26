using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public List<Skill> skills;
    //public string skillString;

    void Awake()
    {
        for(int i = 0; i < 6; i++)
        {
            Skill t = ScriptableObject.CreateInstance<Skill>();
            skills.Add(t);
        }
        skills[0].skillName = "Cooking";
        skills[1].skillName = "Mining";
        skills[2].skillName = "Fishing";
        skills[3].skillName = "Smithing";
        skills[4].skillName = "Woodcutting";
        skills[5].skillName = "Firemaking";

        //Load skills data
    }

    public static string ObjectListToString(List<Skill> listOfObjects)
    {
        string dataAsString = "[";
        dataAsString += System.Environment.NewLine;
        for (int i = 0; i < listOfObjects.Count; i++)
        {
            dataAsString += JsonUtility.ToJson(listOfObjects[i]);

            // Adds a ',' unless it is the last object
            if (i < (listOfObjects.Count - 1))
            {
                dataAsString += ",";
            }
            dataAsString += System.Environment.NewLine;
        }
        dataAsString += "]";
        return dataAsString;
    }

    public static List<Skill> StringToObjectList(string jsonStructuredString)
    {
        List<Skill> dataAsObjectList = new List<Skill>();

        if (jsonStructuredString != "")
        {
            Wrapper<Skill> objectInsideJsonString;
            jsonStructuredString = "{ \"items\": " + jsonStructuredString + "}";
            objectInsideJsonString = JsonUtility.FromJson<Wrapper<Skill>>(jsonStructuredString);
            dataAsObjectList = new List<Skill>(objectInsideJsonString.items);
        }

        return dataAsObjectList;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}
