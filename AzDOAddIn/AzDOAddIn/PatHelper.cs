using System;
using System.Collections.Generic;
using CredentialManagement;

namespace AzDOAddIn
{
    /// <summary>
    /// Stores and retrieves Azure DevOps PATs using Windows Credential Manager.
    /// </summary>
    internal static class PatHelper
    {
        private const string CredentialTargetPrefix = "AzDOAddIn:";

        private static string CredentialTarget(string url) => CredentialTargetPrefix + (url ?? "").Trim().TrimEnd('/');

        internal static string GetPat(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return "";

            try
            {
                using (var cred = new Credential())
                {
                    cred.Target = CredentialTarget(url);
                    cred.Type = CredentialType.Generic;
                    if (cred.Load())
                        return cred.Password ?? "";
                }
            }
            catch
            {
                // Fallback: try migrating from legacy encrypted storage (one-time)
                return MigrateFromLegacyStorage(url);
            }

            return MigrateFromLegacyStorage(url);
        }

        /// <summary>
        /// One-time migration from old PATManager (encrypted in user settings) to Credential Manager.
        /// </summary>
        private static string MigrateFromLegacyStorage(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.PATManager))
                    return "";

                var legacy = LegacyPatStorage.DecryptAndGet(url);
                if (string.IsNullOrEmpty(legacy)) return "";

                SetPat(url, legacy);
                LegacyPatStorage.RemoveUrl(url);
                return legacy;
            }
            catch
            {
                return "";
            }
        }

        internal static List<string> GetStoredUrls()
        {
            var list = new List<string>();
            try
            {
                var stored = Properties.Settings.Default.StoredUrls;
                if (string.IsNullOrEmpty(stored)) return list;

                foreach (var u in stored.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmed = u.Trim();
                    if (trimmed.Length > 0 && !list.Contains(trimmed))
                        list.Add(trimmed);
                }
            }
            catch { }

            return list;
        }

        internal static void SetPat(string url, string pat)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            var target = CredentialTarget(url);

            if (string.IsNullOrEmpty(pat))
            {
                try
                {
                    using (var cred = new Credential { Target = target })
                        cred.Delete();
                }
                catch { }
                RemoveStoredUrl(url);
                return;
            }

            try
            {
                using (var cred = new Credential())
                {
                    cred.Target = target;
                    cred.Username = url;
                    cred.Password = pat;
                    cred.Type = CredentialType.Generic;
                    cred.PersistanceType = PersistanceType.LocalComputer;
                    cred.Save();
                }

                AddStoredUrl(url);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to store PAT in Windows Credential Manager.", ex);
            }
        }

        private static void AddStoredUrl(string url)
        {
            var list = GetStoredUrls();
            var normalized = (url ?? "").Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(normalized) || list.Contains(normalized)) return;
            list.Add(normalized);
            SaveStoredUrls(list);
        }

        private static void RemoveStoredUrl(string url)
        {
            var list = GetStoredUrls();
            var normalized = (url ?? "").Trim().TrimEnd('/');
            list.RemoveAll(s => string.Equals(s, normalized, StringComparison.OrdinalIgnoreCase));
            SaveStoredUrls(list);
        }

        private static void SaveStoredUrls(List<string> urls)
        {
            try
            {
                Properties.Settings.Default.StoredUrls = string.Join(";", urls ?? new List<string>());
                Properties.Settings.Default.Save();
            }
            catch { }
        }

        /// <summary>
        /// Legacy encrypted PAT storage (for one-time migration only).
        /// </summary>
        private static class LegacyPatStorage
        {
            public static string DecryptAndGet(string url)
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.PATManager)) return "";
                var pats = DecryptPats();
                if (pats?.Dictionary == null) return "";
                foreach (var kv in pats.Dictionary)
                    if (string.Equals(kv.Key, url, StringComparison.OrdinalIgnoreCase))
                        return kv.Value ?? "";
                return "";
            }

            public static void RemoveUrl(string url)
            {
                try
                {
                    if (string.IsNullOrEmpty(Properties.Settings.Default.PATManager)) return;
                    var pats = DecryptPats();
                    if (pats?.Dictionary == null) return;
                    pats.Dictionary.RemoveAll(kv => string.Equals(kv.Key, url, StringComparison.OrdinalIgnoreCase));
                    Properties.Settings.Default.PATManager = "";
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

            private static Pats DecryptPats()
            {
                try
                {
                    var key = GetKey();
                    var decstr = DecryptString(key, Properties.Settings.Default.PATManager);
                    if (string.IsNullOrEmpty(decstr)) return null;
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Pats));
                    using (var reader = new System.IO.StringReader(decstr))
                        return (Pats)serializer.Deserialize(reader);
                }
                catch { return null; }
            }

            private static string GetKey()
            {
                var mName = Environment.MachineName ?? "";
                var key = "";
                for (int i = 0; i < 32; i++)
                    key += (i < mName.Length ? char.ConvertToUtf32(mName, i) : i).ToString();
                return key.Length >= 32 ? key.Substring(0, 32) : key.PadRight(32).Substring(0, 32);
            }

            private static string DecryptString(string key, string cipherText)
            {
                if (string.IsNullOrEmpty(cipherText)) return "";
                var iv = new byte[16];
                var buffer = Convert.FromBase64String(cipherText);
                using (var aes = System.Security.Cryptography.Aes.Create())
                {
                    aes.Key = System.Text.Encoding.UTF8.GetBytes(key.Length >= 32 ? key.Substring(0, 32) : key.PadRight(32).Substring(0, 32));
                    aes.IV = iv;
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new System.IO.MemoryStream(buffer))
                    using (var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                    using (var sr = new System.IO.StreamReader(cs))
                        return sr.ReadToEnd();
                }
            }
        }
    }

    [Serializable]
    public class Pats
    {
        [System.Xml.Serialization.XmlElement("StringDictionary")]
        public List<KeyValuePair<string, string>> Dictionary { get; set; }
    }

    [Serializable]
    [System.Xml.Serialization.XmlType(TypeName = "StringDictionary")]
    public struct KeyValuePair<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }
}
