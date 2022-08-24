using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SA.IOSDeploy
{

    [System.Serializable]
    public class AssetFile
    {

        //Editor Use Only
        public bool IsOpen = false;


        public string XCodePath = string.Empty;
        public Object Asset = null;


        public string FileName {
            get {
                #if UNITY_EDITOR

                if (Asset == null) { return "No File"; }
                return Path.GetFileName(FilePath);

               
                #else
                return string.Empty;
                #endif
            }
        }

        public string FilePath {
            get {
                #if UNITY_EDITOR
                if (Asset == null) { return string.Empty; }  
                return AssetDatabase.GetAssetPath(Asset);
                #else
                return string.Empty;
                #endif
            }
        }

        public string XCodeRelativePath {
            get {
                return XCodePath + FileName;
            }
        }

        public bool IsDirectory {
            get {
                FileAttributes attr = File.GetAttributes(FilePath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                    return true;
                } else {
                    return false;
                }
                    
            }
        }

    }
}
