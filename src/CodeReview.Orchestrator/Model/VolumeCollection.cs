using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class VolumeCollection : IDictionary<string, Volume>
    {
        private readonly Dictionary<string, Volume> _volumes = new();

        private ICollection<KeyValuePair<string, Volume>> VolumesCollection => _volumes;

        public ICollection<Volume> ListVolumes() => _volumes.Values;
        
        public ICollection<Volume> ListVolumesToImport()
        {
            return _volumes
                .Where(p => !string.IsNullOrWhiteSpace(p.Value.FolderToImport))
                .Select(p => p.Value)
                .ToList();
        }

        public ICollection<Volume> ListVolumesToExport()
        {
            return _volumes
                .Where(p => !string.IsNullOrWhiteSpace(p.Value.FolderToOutput))
                .Select(p => p.Value)
                .ToList();
        }
        
        #region ICollection
        
        public int Count => _volumes.Count;
        public bool IsReadOnly => false;

        public IEnumerator<KeyValuePair<string, Volume>> GetEnumerator() => _volumes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(KeyValuePair<string, Volume> item) => Add(item.Key, item.Value);

        public void Clear() => _volumes.Clear();

        public bool Contains(KeyValuePair<string, Volume> item) => _volumes.Contains(item);

        public void CopyTo(KeyValuePair<string, Volume>[] array, int arrayIndex) => VolumesCollection.CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, Volume> item) => VolumesCollection.Remove(item);

        #endregion

        #region IDictionary

        ICollection<string> IDictionary<string, Volume>.Keys => _volumes.Keys;
        ICollection<Volume> IDictionary<string, Volume>.Values => _volumes.Values;
        
        public void Add(string name, Volume volume)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (volume == null) throw new ArgumentNullException(nameof(volume));

            volume.Name = name;
            
            _volumes.Add(name, volume);
        }

        public bool ContainsKey(string name) => _volumes.ContainsKey(name);

        public bool Remove(string name) => _volumes.Remove(name);

        public bool TryGetValue(string name, out Volume volume) => _volumes.TryGetValue(name, out volume);

        public Volume this[string name]
        {
            get => _volumes[name];
            set => Add(name, value);
        }

        #endregion
    }
}