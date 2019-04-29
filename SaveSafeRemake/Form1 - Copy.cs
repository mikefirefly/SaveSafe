using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Windows.Forms;
using System.Diagnostics;

namespace SaveSafeRemake
{
    public partial class fMainForm : Form
    {
        readonly string backupRepository = AppDomain.CurrentDomain.BaseDirectory + "BACKUPS";
        private DateTime lastRead = DateTime.MinValue;

        public fMainForm()
        {
            InitializeComponent();
            Directory.CreateDirectory(backupRepository);
        }

        private void fMainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] names = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (names.Length != 1) return; // multiple folders dropped, not allowed
            if (!Directory.Exists(names[0])) return; // not a folder

            foreach (string name in names)
                lbFolderList.Items.Add(name + " <" + GetDirectorySize(name) + " KB>");

            AddWatcher(names[0]);
        }

        private void fMainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private static long GetDirectorySize(string p)
        {
            string[] a = Directory.GetFiles(p, "*.*", SearchOption.AllDirectories);

            long b = 0;
            foreach (string name in a)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }

            return b / 1024;
        }

        private void AddWatcher(string folder)
        {
//            foreach (string folder in lbFolderList.Items)
            //{
                FileSystemWatcher fsw = new FileSystemWatcher(folder);
                fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName;
                fsw.IncludeSubdirectories = true;
                fsw.Changed += new FileSystemEventHandler(OnChanged);
                fsw.Created += new FileSystemEventHandler(OnChanged);
                fsw.Deleted += new FileSystemEventHandler(OnChanged);
                fsw.EnableRaisingEvents = true;
            //}

        }

        private void fMainForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists("entries.txt")) return;

            foreach (string folder in File.ReadAllLines("entries.txt").ToList())
            {
                if (Directory.Exists(folder))
                {
                    lbFolderList.Items.Add(folder + " <" + GetDirectorySize(folder) + " KB>");
                    AddWatcher(folder);
                }
                else MessageBox.Show("The following folder doesn't exist: \n" + folder, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // 1st fix for double events
            try
            {
                ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                string dest = backupRepository + "\\" + ((FileSystemWatcher)sender).Path.Replace(":", "_").Replace("\\", "_");
                string curTime = DateTime.Now.ToString().Replace("/", "-").Replace(":", "_").Replace(" ", "+");

                ZipIt(((FileSystemWatcher)sender).Path, dest);

                DoCleanup(3, backupRepository, dest.Substring(dest.LastIndexOf("\\") + 1));
            }
            finally
            {
                ((FileSystemWatcher)sender).EnableRaisingEvents = true;
            }
            
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
        }

        private void DoCleanup(int fileCount, string location, string name)
        {
            var fileList = Directory.GetFiles(location, name + "*");
            var filesToBackup_fi = new List<FileInfo>();

            if (fileList.Length > fileCount)
            {
                foreach (String f in fileList)
                    filesToBackup_fi.Add(new FileInfo(f));

                var sortedFiles = filesToBackup_fi.OrderBy(fi => fi.CreationTime).ToList();

                for (int i = 0; i < sortedFiles.Count - fileCount; i++)
                    File.Delete(sortedFiles[i].FullName);
            }
        }

        private void ZipIt(string folderToBackup, string backupDestination)
        {
            //{ 11 / 12 / 2017 09:15:24}
            string curTime = DateTime.Now.ToString().Replace("/", "-").Replace(":", "_").Replace(" ", "+");
            //if (File.Exists(backupDestination + " (" + curTime + ").zip")) return; // ugly hack to avoid firing multiple events
            
            try
            {
                ZipFile.CreateFromDirectory(folderToBackup, backupDestination + " (" + curTime + ").zip", CompressionLevel.Fastest, true);
            }
            catch (Exception ex)
            {
                using (var sp = new System.Media.SoundPlayer("error.wav")) sp.Play();
                throw;
            }

            using (var sp = new System.Media.SoundPlayer("success.wav")) sp.Play();

        }

        private void activeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activeToolStripMenuItem.Checked)
                lbFolderList.Items[lbFolderList.SelectedIndex] = "(+) " + lbFolderList.Items[lbFolderList.SelectedIndex];
            else lbFolderList.Items[lbFolderList.SelectedIndex] = lbFolderList.Items[lbFolderList.SelectedIndex].ToString().Replace("(+) ", "");
            // remove from watchlist. Needs a list of fsws.
        }

        private void browseSaveBackupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", backupRepository);
        }

        private void browseTargetFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string tmp = lbFolderList.Items[lbFolderList.SelectedIndex].ToString();
            //tmp = tmp.Replace("(*) ", "");
            //tmp = tmp.Substring(0, tmp.IndexOf(" <"));// Replace()
            Process.Start("explorer.exe", StripEntry(lbFolderList.Items[lbFolderList.SelectedIndex].ToString()));//.Replace("(*) ","").Substring(0, );
        }

        private string StripEntry(string entry)
        {
            string t = entry.Replace("(+) ", "");
            if (t.Contains(" <")) t = t.Substring(0, t.IndexOf(" <"));
            return t;
        }

        private void fMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var tmp = new List<string>(lbFolderList.Items.Count);
            foreach (string s in lbFolderList.Items)
            {
                tmp.Add(StripEntry(s));
            }
            File.WriteAllLines("entries.txt", tmp);
        }
    }
}
// right click: Active (checkbox - musi to pokazywac na liscie! (*)), Delete ALL saves, Remove from watchlist, Browse folder..
// to disable: EnableRaisingEvents = false;
// musi na poczatku wykrywac czy w ogole folder istnieje, jesli nie to oznaczac (!) na liscie