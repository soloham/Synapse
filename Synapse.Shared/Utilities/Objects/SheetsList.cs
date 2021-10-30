namespace Synapse.Utilities.Objects
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SheetsList
    {
        #region Properties

        public bool SheetsLoaded { get; set; }
        public string[] GetSheetsPath { get; private set; }

        public int[] GetNewDirIndexes => newDirIndexes;
        private int[] newDirIndexes = new int[0];

        #endregion

        #region Variables

        private string curPath = "";
        private bool curIncludeSubDirs;

        private readonly Action<string> UpdateStatus;

        #endregion

        #region Methods

        public SheetsList(Action<string> UpdateStatus = null)
        {
            this.UpdateStatus = UpdateStatus;
        }

        public static string[] GetSheetsFrom(string searchFolder, string[] filters, bool isRecursive,
            out int[] newDirIndices)
        {
            var filesFound = new List<string>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var _newDirIndices = new List<int>();

            foreach (var filter in filters)
                try
                {
                    filesFound.AddRange(Directory.GetFiles(searchFolder, string.Format("*.{0}", filter), searchOption));
                }
                catch
                {
                }

            if (isRecursive)
            {
                var dirs = Directory.GetDirectories(searchFolder, "*", searchOption);

                for (var i = 0; i < dirs.Length; i++)
                {
                    var index = (i != 0 ? _newDirIndices[i - 1] : 0) + Directory.GetFiles(dirs[i]).Length;
                    _newDirIndices.Add(index);
                }
            }

            newDirIndices = _newDirIndices.ToArray();
            return filesFound.ToArray();
        }

        public bool Scan(string path, bool includeSubDirs, out string err)
        {
            var success = false;
            var error = "";

            if (path != "")
            {
                UpdateStatus?.Invoke("Status: Scanning the directory...");

                var filters = new[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "tif" };
                var imgFiles = GetSheetsFrom(path, filters, includeSubDirs, out newDirIndexes);
                if (imgFiles.Length > 0)
                {
                    this.GetSheetsPath = imgFiles;
                    UpdateStatus?.Invoke("Status: Scan Completed successfully.");

                    success = true;
                }
                else
                {
                    err = "Image files not found.";
                    UpdateStatus?.Invoke("Status: Scan Completed un-successfully.");

                    success = false;
                }

                UpdateStatus?.Invoke("Status: Loading has been completed.");

                curPath = path;
                curIncludeSubDirs = includeSubDirs;
            }
            else
            {
                error = "Invalid Directory";
                success = false;
            }

            this.SheetsLoaded = success;
            err = error;
            return success;
        }

        public bool Rescan(out string err)
        {
            return this.Scan(curPath, curIncludeSubDirs, out err);
        }

        #endregion
    }
}