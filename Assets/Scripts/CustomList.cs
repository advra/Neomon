//Script name : CustomList.cs
using UnityEngine;
using System;
using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>
[System.Serializable]
public class CustomList : MonoBehaviour
{
    Card card;
    PlayerHandController playerHandController;

    //This is our custom class with our variables
    //[System.Serializable]
    //public class MyClass
    //{
    //    public GameObject AnGO;
    //    public int AnInt;
    //    public float AnFloat;
    //    public Vector3 AnVector3;
    //    public int[] AnIntArray = new int[0];
    //}

    //This is our list we want to use to represent our class as an array.
    //public List<Card> MyList = new List<Card>(1);
    //public List<Card> deck = new List<Card>(10);


    //void AddNew()
    //{
    //    //Add a new index position to the end of our list
    //    MyList.Add(new card());
    //}

    //void Remove(int index)
    //{
    //    //Remove an index position from our list at a point in our list array
    //    MyList.RemoveAt(index);
    //}
}