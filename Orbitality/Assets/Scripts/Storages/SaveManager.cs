using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Storages
{
    public class SaveManager<T>
    {
        public SaveManager() => _savePath = Path.Combine(Application.persistentDataPath, "Saves");

        private readonly string _savePath;
        public string GetDefaultSaveName() => DateTime.Now.ToFileTime().ToString();

        public IEnumerable<string> GetSaves()
        {
            EnsureDirectory(_savePath);
            return Directory.GetFiles(_savePath);
        }

        public void Save(T data) => Save(data, GetDefaultSaveName());

        public void Save(T data, string saveName)
        {
            EnsureDirectory(_savePath);
            var stringData = JsonConvert.SerializeObject(data);
            File.WriteAllText(GetFilePath(saveName), stringData);
        }

        public T LoadLast() => Load(GetSaves().Last());
        public T Load(string saveName) => JsonConvert.DeserializeObject<T>(File.ReadAllText(GetFilePath(saveName)));
        public void RemoveAll()
        {
            if (Directory.Exists(_savePath))
                Directory.Delete(_savePath, true);
        }

        public void Remove(string saveName) => File.Delete(GetFilePath(saveName));
        private string GetFilePath(string saveName) => Path.Combine(_savePath, saveName);

        private void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public string GetDisplayName(string save)
        {
            var displayName = Path.GetFileName(save);
            if (long.TryParse(displayName, out var ticks))
                displayName = DateTime.FromFileTime(ticks).ToString(CultureInfo.CurrentCulture);
            return displayName;
        }
    }
}