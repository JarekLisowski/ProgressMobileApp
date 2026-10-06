using System.Collections.Concurrent;

namespace Progress.Navireo.Managers
{
  public class PersistantData
  {
    private ConcurrentDictionary<string, DateTime> _documentUUID = new ConcurrentDictionary<string, DateTime>();

    public bool DocumentExists(string key)
    {
      return _documentUUID.TryGetValue(key, out _);
    }

    public void AddDocumentInfo(string key)
    {
      RemoveOld();
      if (!_documentUUID.TryAdd(key, DateTime.Now))
        throw new Exception("The document already exists!");
    }

    private void RemoveOld()
    {
      var cutoff = DateTime.UtcNow.AddHours(-24);
      foreach (var kvp in _documentUUID)
      {
        if (kvp.Value < cutoff)
        {
          _documentUUID.TryRemove(kvp);
        }
      }
    }

  }
}
