#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CasToon.CasToon
{

// Modified from https://gist.github.com/daniel-ilett/99992dfcb565324c76a655e16115aedf
    public class GradientEditor : EditorWindow
    {
        private Gradient gradient = new Gradient();
        private int resolution = 256;
        private string filename = "Shadow Gradient";
        private string filenameWithExtension;

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Shadow Gradient");
            gradient = EditorGUILayout.GradientField("Gradient", gradient);
            filename = EditorGUILayout.TextField("Filename", filename);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Convert to asset", GUILayout.Height(50)))
            {
                ConvertGradient();
                CasToonGUIV2.SetShadowGradient(filenameWithExtension);
            }
        }

        private void ConvertGradient()
        {
            Texture2D tex = new Texture2D(resolution, 1);
            Color[] texColors = new Color[resolution];

            for (int x = 0; x < resolution; ++x)
            {
                texColors[x] = gradient.Evaluate((float)x / resolution);
            }

            tex.SetPixels(texColors);
            byte[] bytes = tex.EncodeToPNG();
            filenameWithExtension = "ShadowGradients/" + filename + ".png";
            
            try
            {
                string path = Application.dataPath + "/" + filenameWithExtension;

                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllBytes(path, bytes);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
            catch (IOException e)
            {
                Debug.LogError(e);
            }
        }
    }
}

#endif