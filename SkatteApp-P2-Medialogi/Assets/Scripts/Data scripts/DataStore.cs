using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataStore : MonoBehaviour
{

    List<float> bNumbers = new List<float>();
    Dictionary<string, int> qAwnsers = new Dictionary<string, int>();
    public string fileName;
    public bool printFile;
    int prints;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        prints = 0; 
        printFile = false;
        fileName = txtScript.Instance.name4File;
    }

    // Update is called once per frame
    private void Update()
    {
        if (printFile == true && prints == 0)
        {
            prints++;
        SaveToFile(bNumbers, qAwnsers, fileName+"outputData.csv");
        }
    }

    //call this in Bayesian script
    public void LogBayesian(float x)
    {
       bNumbers.Add(x);
    }

    public void LogQuestion(string awnser, int x) 
    {
        qAwnsers.Add(awnser, x);
      
    } 
    public void onKlik()
    {
        SaveToFile(bNumbers, qAwnsers, fileName+"outputData.csv");

    }

    static void SaveToFile(List<float> list, Dictionary<string, int> dict, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Bayesian List Contents:");
            foreach (float item in list)
            {
                writer.WriteLine(item);
            }

            writer.WriteLine(); // Empty line

            writer.WriteLine("Awnsers Contents:");
            foreach (KeyValuePair<string, int> entry in dict)
            {
                writer.WriteLine($"{entry.Key}: {entry.Value}");
            }
            Debug.Log("parinted " + "outputData.csv");
        }

        Console.WriteLine($"Data saved to {filePath}");
    }
}

