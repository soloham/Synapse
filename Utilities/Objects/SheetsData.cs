using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Synapse.Utilities.Objects
{
    public class SheetsData
    {
        #region Properties
        public bool SheetsLoaded { get; set; }
        public string[] GetSheetsPath { get => sheetsPath; }
        private string[] sheetsPath;
        public int[] GetNewDirIndexes { get => newDirIndexes; }
        private int[] newDirIndexes = new int[0];
        #endregion

        #region Variables
        string curPath = "";
        bool curIncludeSubDirs = false;
        #endregion

        #region Methods
        public static String[] GetSheetsFrom(String searchFolder, String[] filters, bool isRecursive, out int[] newDirIndices)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            List<int> _newDirIndices = new List<int>();

            foreach (var filter in filters)
            {
                try
                {
                    filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
                }
                catch
                {

                }
            }

            if (isRecursive)
            {
                var dirs = Directory.GetDirectories(searchFolder, "*", searchOption);

                for (int i = 0; i < dirs.Length; i++)
                {
                    var index = ((i != 0) ? _newDirIndices[i - 1] : 0) + Directory.GetFiles(dirs[i]).Length;
                    _newDirIndices.Add(index);
                }
            }

            newDirIndices = _newDirIndices.ToArray();
            return filesFound.ToArray();
        }

        public bool Scan(string path, bool includeSubDirs, out string err)
        {
            bool success = false;
            string error = "";

            if (path != "")
            {
                //GraderAI.UpdateStatus("Status: Scanning the directory...");

                var filters = new String[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "tif" };
                var imgFiles = GetSheetsFrom(path, filters, includeSubDirs, out newDirIndexes);
                if (imgFiles.Length > 0)
                {
                    sheetsPath = imgFiles;
                    //GraderAI.UpdateStatus("Status: Scan Completed successfully.");
                }
                else
                {
                    err = "Image files not found.";
                    //GraderAI.UpdateStatus("Status: Scan Completed un-successfully.");
                }
                //GraderAI.UpdateStatus("Status: Loading has been completed.");

                curPath = path;
                curIncludeSubDirs = includeSubDirs;
                success = true;
            }
            else
            {
                error = "Invalid Directory";
                success = false;
            }

            SheetsLoaded = success;
            err = error;
            return success;
        }
        public bool Rescan(out string err)
        {
            return Scan(curPath, curIncludeSubDirs, out err);
        }
        #endregion
    }
}
