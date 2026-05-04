using UnityEngine;

public class MockSaveService : ISaveService
{
    public int SaveCallCount { get; private set; }

    public void Save()
    {
        SaveCallCount++;
        Debug.Log("Saved Game...");
    }

    public void Load() { }
}
