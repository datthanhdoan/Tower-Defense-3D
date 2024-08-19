using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    private List<IObserver> _observers = new List<IObserver>();

    // add the observer to the subject's collection
    public void AddObserver(IObserver observer)
    {
        _observers.Add(observer);
    }

    // remove the observer from the subject's collection
    public void RemoveObserver(IObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void NotifyObservers()
    {
        _observers.ForEach((_observers) =>
        {
            _observers.OnNotify();
        });
    }
}