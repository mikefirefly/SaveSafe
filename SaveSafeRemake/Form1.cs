using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Timers;
using System.Windows.Forms;

namespace SaveSafeRemake
{
    public partial class fMainForm : Form
    {
        private readonly string backupRepository = AppDomain.CurrentDomain.BaseDirectory + "BACKUPS";
        private DateTime lastRead = DateTime.MinValue;
        private bool cooldown;
        private readonly int cooldownTime = 3000;
        private FileSystemWatcher queuedFSW = new FileSystemWatcher();
        private List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
        private readonly int backupCount = 3;
        private readonly string activeIndicator = "✔ ";
        private readonly string inactiveIndicator = "✘ ";

        // ∘∙▶▷◆◇○●◉◈★☆
        // ⇒✽✸✹✺✔✖✘➤➡➠➭➫
        public fMainForm()
        {
            InitializeComponent();
            Directory.CreateDirectory(backupRepository);
            Directory.CreateDirectory(backupRepository + "\\MANUAL");
        }

        private void fMainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] names = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (names.Length != 1) return; // multiple folders dropped, not allowed
            if (!Directory.Exists(names[0])) return; // not a folder

            foreach (string name in names)
                lbFolderList.Items.Add(activeIndicator + name.ToUpper());// + "@" + GetDirectorySize(name) + " KB>");

            watchers.Add(AddWatcher(names[0]));
        }

        private void fMainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// Return directory size in kilobytes
        /// </summary>
        /// <param name="p">Directory path</param>
        /// <returns></returns>
        private static long GetDirectorySize(string p, string pattern)
        {
            if (!Directory.Exists(p)) return -1;
            string[] files = Directory.GetFiles(p, pattern, SearchOption.AllDirectories);

            long bytes = 0;
            foreach (string name in files)
            {
                FileInfo info = new FileInfo(name);
                bytes += info.Length;
            }

            return bytes / 1024;
        }

        private FileSystemWatcher AddWatcher(string folder)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(folder);
            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.FileName;
            fsw.IncludeSubdirectories = true;
            fsw.Changed += new FileSystemEventHandler(OnChanged);
            fsw.Created += new FileSystemEventHandler(OnChanged);
            fsw.Deleted += new FileSystemEventHandler(OnChanged);
            fsw.EnableRaisingEvents = true;

            return fsw;
        }

        private void fMainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("entries.txt")) return;

            foreach (string folder in File.ReadAllLines("entries.txt").ToList())
            {
                if (Directory.Exists(folder))
                {
                    watchers.Add(AddWatcher(folder));
                    lbFolderList.Items.Add(activeIndicator + folder.ToUpper());// + "," + GetDirectorySize(folder) + " KB>");
                    //AddWatcher(folder);
                }
                else
                {
                    lbFolderList.Items.Add(inactiveIndicator + folder.ToUpper());// + "<? KB>");
                    MessageBox.Show("The following folder doesn't exist: \n\n" + folder + "\n\nEntry will be disabled.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB"; });
        }

        private string changedPath = String.Empty;
        private string destPath = String.Empty;

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            ((FileSystemWatcher)sender).EnableRaisingEvents = false;

            if (cooldown) return;

            cooldown = true;
            queuedFSW = (FileSystemWatcher)sender;
            changedPath = queuedFSW.Path;
            destPath = backupRepository + "\\" + queuedFSW.Path.Replace(":", "⇒").Replace("\\", "⇒");
            var t = new System.Timers.Timer(cooldownTime);
            t.Enabled = true;
            t.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            t.Start();

            //Invoke((MethodInvoker)delegate {
            //statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB";
            //statusStrip.Items["tsSelectedSize"].Text = "ASD";
            //});
            //Invoke((MethodInvoker)delegate { });
            //var fileList = Directory.GetFiles(backupRepository, dest.Substring(dest.LastIndexOf("\\") + 1) +"*");
            //long ttlSize = 0;
            //DirectoryInfo di = new DirectoryInfo(backupRepository);
            //foreach (FileInfo f in new DirectoryInfo(backupRepository).GetFiles(dest.Substring(dest.LastIndexOf("\\") + 1) + "*"))
            //    ttlSize += f.Length;
            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsSelectedSize"].Text = $"Selected entry backup size: {ttlSize} KB"; });
            // Invoke((MethodInvoker)delegate { statusStrip.Items["tsSelectedSize"].Text = "ASD"; });
            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsSelectedLastTS"].Text = $"Selected entry last backup: {GetDirectorySize(backupRepository)} KB"; });
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine("change detected, processing...");
            //if (!queuedFSW.EnableRaisingEvents) return;

            // is it still enabled?
            //foreach (var watcher in watchers)
            //   if (queuedFSW.Path == watcher.Path && !watcher.EnableRaisingEvents)
            //       return;
            //if (!queuedFSW.isEnabledOnList) queuedFSW = null, timer remove, return;

            // this FSW was disabled in the meantime!
            if (queuedFSW == null)
            {
                ((System.Timers.Timer)sender).Dispose();
                cooldown = false;
                return;
            }

            if (ZipIt(queuedFSW.Path, destPath, true))
            {
                ((System.Timers.Timer)sender).Dispose();
                DoCleanup(backupRepository, destPath.Substring(destPath.LastIndexOf("\\") + 1));
                queuedFSW.EnableRaisingEvents = true;
                cooldown = false;
            }
            else
            {
                // will retry (timer wasnt destroyed)
                if (!Directory.Exists(queuedFSW.Path))
                {
                    ((System.Timers.Timer)sender).Dispose();
                    queuedFSW.EnableRaisingEvents = false;
                    cooldown = false;

                    foreach (string lvi in lbFolderList.Items)
                    {
                        if (StripEntry(lvi) == queuedFSW.Path.ToUpper())
                        {
                            for (int i = 0; i <= lbFolderList.Items.Count; i++)
                                if (lbFolderList.Items[i].ToString() == lvi)
                                {
                                    Invoke((MethodInvoker)delegate {
                                        lbFolderList.Items[i] = inactiveIndicator + lvi.Substring(activeIndicator.Length); //statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB";
                                    });
                                    break;
                                }
                            break;
                        }
                    }
                    MessageBox.Show("Entry folder for " + queuedFSW.Path + " doesn't exist, disabling entry!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //}
        //finally
        //{
        //((FileSystemWatcher)sender).EnableRaisingEvents = true;
        //}

        /*DateTime lastWriteTime = DateTime.MaxValue;

        bool needbackup = false;
        string[] subdirs = Directory.GetDirectories(((FileSystemWatcher)sender).Path, "*", System.IO.SearchOption.AllDirectories);
        string where = "";
        foreach (string subdir in subdirs)
        {
            if (File.GetLastWriteTime(subdir) > lastRead)
                {  needbackup = true; lastWriteTime = File.GetLastWriteTime(subdir); where = subdir; break; }
            //if (blockedDirs == null || !blockedDirs.Any(p => subDirPath.ToLower().Contains(p)))
            {
               // AddAllFilesOfAllSubdirsWithFilterToList(ref filesList, subDirPath, startDateUtc, optionalEndDateUtc, blockedDirs);
            }
        }

        if (!needbackup) return;
        //lastWriteTime = File.GetLastWriteTime(((FileSystemWatcher)sender).Path);
        if (lastWriteTime != lastRead)
        {
            string dest = backupRepository + "\\" + ((FileSystemWatcher)sender).Path.Replace(":", "_").Replace("\\", "_");
            string curTime = DateTime.Now.ToString().Replace("/", "-").Replace(":", "_").Replace(" ", "+");

            ZipIt(((FileSystemWatcher)sender).Path, dest);

            DoCleanup(3, backupRepository, dest.Substring(dest.LastIndexOf("\\") + 1));
            lastRead = lastWriteTime;
        }*/

        private void DoCleanup(string location, string name)
        {
            var fileList = Directory.GetFiles(location, name + "*");
            var filesToBackup_fi = new List<FileInfo>();

            if (fileList.Length > backupCount)
            {
                foreach (String f in fileList)
                    filesToBackup_fi.Add(new FileInfo(f));

                var sortedFiles = filesToBackup_fi.OrderBy(fi => fi.CreationTime).ToList();

                for (int i = 0; i < sortedFiles.Count - backupCount; i++)
                    File.Delete(sortedFiles[i].FullName);
            }

            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB"; });
        }

        private bool ZipIt(string folderToBackup, string backupDestination, bool addTimestamp)
        {
            //{ 11 / 12 / 2017 09:15:24}
            string timestamp = addTimestamp ? " (" + (DateTime.Now.ToString().Replace("/", "-").Replace(":", "⇒").Replace(" ", "+")) + ").zip" : "";

            try
            {
                if (File.Exists(backupDestination + timestamp)) File.Delete(backupDestination + timestamp);
                ZipFile.CreateFromDirectory(folderToBackup, backupDestination + timestamp, CompressionLevel.Fastest, false);
            }
            catch (Exception ex)
            {
                using (var sp = new System.Media.SoundPlayer("_error.wav")) sp.Play();
                // delete partially created archive
                File.Delete(backupDestination + timestamp);
                return false;
            }

            using (var sp = new System.Media.SoundPlayer("_success.wav")) sp.Play();

            return true;
            // update Form's title asynchronously
            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB"; });
        }

        private void activeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeToolStripMenuItem.Checked)
            {
                if (!Directory.Exists(lbFolderList.Items[lbFolderList.SelectedIndex].ToString().Substring(activeIndicator.Length)))
                {
                    MessageBox.Show("No such folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    activeToolStripMenuItem.Checked = false;
                    return;
                }
                lbFolderList.Items[lbFolderList.SelectedIndex] = activeIndicator + lbFolderList.Items[lbFolderList.SelectedIndex].ToString().Substring(activeIndicator.Length);
            }
            else
                lbFolderList.Items[lbFolderList.SelectedIndex] = inactiveIndicator + lbFolderList.Items[lbFolderList.SelectedIndex].ToString().Substring(activeIndicator.Length);

            foreach (var watcher in watchers)
            {
                if (lbFolderList.SelectedItem.ToString().Contains(watcher.Path.ToUpper()))
                {
                    watcher.EnableRaisingEvents = activeToolStripMenuItem.Checked;
                }
                if (queuedFSW == watcher)
                 queuedFSW = null;
                     
            }
        }

        private void browseSaveBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", backupRepository);
        }

        private void browseTargetFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", StripEntry(lbFolderList.Items[lbFolderList.SelectedIndex].ToString()));//.Replace("(*) ","").Substring(0, );
        }

        /// <summary>
        /// Remove active/inactive indicator and trim string
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string StripEntry(string entry)
        {
            string t = entry.Replace(activeIndicator, "").Replace(inactiveIndicator, "");
            //if (t.Contains("<")) t = t.Substring(0, t.IndexOf("<"));
            return t.Trim(new char[] { ' ' });
        }

        private void fMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var tmp = new List<string>(lbFolderList.Items.Count);
            foreach (string s in lbFolderList.Items)
                tmp.Add(StripEntry(s));
            File.WriteAllLines("entries.txt", tmp);
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var watcher in watchers)
                if (lbFolderList.SelectedItem.ToString().Contains(watcher.Path))
                {
                    watcher.EnableRaisingEvents = false;
                    watchers.Remove(watcher);
                    break;
                }

            if (MessageBox.Show("Do you also want to delete all save backups for this entry?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                deleteSaveBackupsToolStripMenuItem_Click(this, null);

            lbFolderList.Items.Remove(lbFolderList.SelectedItem);
        }

        private void deleteSaveBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all save backups for this entry?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string pattern = lbFolderList.SelectedItem.ToString().Replace("\\", "⇒").Replace(":", "⇒");
                pattern = StripEntry(pattern);
                foreach (var file in Directory.GetFiles(backupRepository, $"{pattern} (*.zip"))
                    File.Delete(file);
            }

            //Invoke((MethodInvoker)delegate { statusStrip.Items["tsTotalRepositorySize"].Text = $"Total repository size: {GetDirectorySize(backupRepository)} KB"; });
        }

        private void lbFolderList_MouseDown(object sender, MouseEventArgs e)
        {
            lbFolderList.SelectedIndex = lbFolderList.IndexFromPoint(e.X, e.Y);
            if (lbFolderList.SelectedIndex == -1)
                lbFolderList.ContextMenuStrip = null;
            else
            {
                lbFolderList.ContextMenuStrip = mnuPopup;
                activeToolStripMenuItem.Checked = lbFolderList.SelectedItem.ToString().Contains(activeIndicator);
            }
        }

        private void lbFolderList_DoubleClick(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", backupRepository);
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Entry: " + StripEntry(lbFolderList.SelectedItem.ToString()) + "\n" +
                "Entry folder size: " + GetDirectorySize(lbFolderList.SelectedItem.ToString().Substring(activeIndicator.Length), "*.*") + " KB\n" +
                "Entry backup size: " + GetDirectorySize(backupRepository, StripEntry(lbFolderList.SelectedItem.ToString().Replace(":", "⇒").Replace("\\", "⇒") + "*")) + " KB",
                "Entry information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*
        private void deleteCurrentSaveAndRestoreMostRecentSaveFromBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var watcher in watchers)
                if (lbFolderList.SelectedItem.ToString().Contains(watcher.Path))
                {
                    watcher.EnableRaisingEvents = false;

                    // delete current save folder
                    Directory.Delete(StripEntry(lbFolderList.SelectedItem.ToString()), true);

                    // unzip most recent save backup to save folder
                    //string mostRecentSave = "";
                    var fileList = Directory.GetFiles(backupRepository, StripEntry(lbFolderList.SelectedItem.ToString()).Replace("\\", "_").Replace(":", "_") + "*");
                    var backupFiles_fi = new List<FileInfo>();

                    foreach (String f in fileList)
                        backupFiles_fi.Add(new FileInfo(f));

                    var mostRecentSave = backupFiles_fi.OrderBy(fi => fi.CreationTime).ToList()[backupFiles_fi.Count - 2];
                    //sortedFiles.Reverse();
                    //  mostRecentSave = sortedFiles[0].FullName;

                    ZipFile.ExtractToDirectory(mostRecentSave.ToString(), StripEntry(lbFolderList.SelectedItem.ToString()+"\\.."));
                    //foreach (var file in Directory.GetFiles(StripEntry(lbFolderList.SelectedItem.ToString())))//, $"{pattern} (*.zip"))
                    //File.Delete(file);
                    watcher.EnableRaisingEvents = true;
                    break;
                }
        }*/

        private void performManualBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*",
                AddExtension = false,
                InitialDirectory = Directory.Exists(backupRepository + "\\MANUAL") ? backupRepository + "\\MANUAL" : backupRepository,
                Title = "Choose a file to backup to - " + lbFolderList.SelectedItem
            };

            if (sfd.ShowDialog() == DialogResult.OK)
                ZipIt(StripEntry(lbFolderList.SelectedItem.ToString()), sfd.FileName, false);
        }

        private void restoreFromSpecificBackupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                Filter = "ZIP files (*.zip)|*.zip|All files (*.*)|*.*",
                AddExtension = false,
                InitialDirectory = Directory.Exists(backupRepository + "\\MANUAL") ? backupRepository + "\\MANUAL" : backupRepository,
                Title = "Choose a backup to restore from - " + lbFolderList.SelectedItem
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (var watcher in watchers)
                    if (lbFolderList.SelectedItem.ToString().Contains(watcher.Path))
                    {
                        watcher.EnableRaisingEvents = false;
                        Directory.Delete(StripEntry(lbFolderList.SelectedItem.ToString()), true);
                        ZipFile.ExtractToDirectory(ofd.FileName, StripEntry(lbFolderList.SelectedItem.ToString()));
                        watcher.EnableRaisingEvents = true;
                        break;
                    }
            }
        }
    }
}

// right click: Active (checkbox - musi to pokazywac na liscie! (*)), Delete ALL saves, Remove from watchlist, Browse folder..
// to disable: EnableRaisingEvents = false;
// musi na poczatku wykrywac czy w ogole folder istnieje, jesli nie to pokazac komunikat i nie dodawac folderu do listy
// ∘∙▶▷◆◇○●◉◈★☆
//⇒✽✸✹✺✔✖✘➤➡➠➭➫
/*
+create new file in top folder
+create new folder in top folder
+create new file in empty subfolder
+create new file in non-empty subfolder
+create new folder in empty subfolder
+create new folder in non-empty subfolder
+rename file in subfolder

.rename file in top folder
.rename empty subfolder
.rename subfolder with files
+usunac folder monitorowany - co sie dzieje z fsw?
- problem kiedy archiwum nie moze byc utworzone - caly czas probuje je stworzyc a chyba nie powinien?
- entry disabled - nadal probuje zrobic backup (ale chyba tylko jesli sie nie udalo za 1. razem)
*/