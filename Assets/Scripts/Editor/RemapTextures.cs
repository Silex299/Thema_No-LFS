using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class RemapTextures : EditorWindow
{


    private Texture2D _redChannel;
    private Color _defaultRedChannel = Color.black;
    
    private Texture2D _blueChannel;
    private Color _defaultBlueChannel = Color.white;
    
    private Texture2D _greenChannel;
    private Color _defaultGreenChannel = Color.white;
    
    private Texture2D _alphaChannel;
    private Color _defaultAlphaChannel = Color.gray;


    private Texture2D _diffuseTexture;
    private Texture2D _opacity;
    

    private string _selectedFolderPath;
    private readonly List<int> _resolutions = new List<int> { 512, 1024, 2048, 4096, 8192 };
    private int _selectedResolution = 512;
    
    public string _outputFileName = "mask_output";
    
    
    
    [MenuItem("Editor Window/Remap Textures")]
    public static void ShowExample()
    {
        RemapTextures wnd = GetWindow<RemapTextures>();
        wnd.titleContent = new GUIContent("Remap Textures");
    }


    public void OnEnable()
    {


        #region Other

        // Clear existing items.
        rootVisualElement.Clear();

        // Load and apply the USS style sheet.
        var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/RemapTextures.uss");
        if (stylesheet != null)
        {
            rootVisualElement.styleSheets.Add(stylesheet);
        }
        else
        {
            Debug.LogError("Failed to load stylesheet.");
            return;
        }

        #endregion

        #region Info

        var infoElement1 = new VisualElement();
        infoElement1.AddToClassList("info-element-1");
        
        

        // Add the info icon
        var infoIcon = new Image
        {
            image = EditorGUIUtility.IconContent("console.infoicon").image,
            style =
            {
                height = 16,
                width = 16,
                marginRight = 5 // Some spacing between icon and text
            }
        };
        
        infoIcon.style.marginLeft = new StyleLength(StyleKeyword.Auto);
        infoIcon.style.marginRight = new StyleLength(StyleKeyword.Auto);
        infoElement1.Add(infoIcon);

        // Add text next to the icon
        var infoText =
            new Label(
                "Red Channel is metallic, green channel is occlusion, blue channel is detail mask, alpha channel is smoothness")
                {
                    style =
                    {
                        whiteSpace = WhiteSpace.Normal,
                        marginTop = new StyleLength(5),
                        marginLeft = new StyleLength(StyleKeyword.Auto),
                        marginRight = new StyleLength(StyleKeyword.Auto)
                    }
                };

        infoElement1.Add(infoText);
        

        #endregion

        #region Output

        var outputElement = new VisualElement();
        outputElement.AddToClassList("output");

        
        var outputLabel = new Label("Output");
        outputLabel.AddToClassList("center-label");
        outputElement.Add(outputLabel);
        
        
        var resolutionDropdown = new PopupField<int>("Output Resolution", _resolutions, 0);
        resolutionDropdown.RegisterValueChangedCallback(evt => 
        {
            _selectedResolution = evt.newValue;
        });
        outputElement.Add(resolutionDropdown);
        
        var outputName = new TextField("File Name")
        {
            value = _outputFileName
        };
        outputName.RegisterValueChangedCallback(evt => 
        {
            _outputFileName = evt.newValue;
        });
        
        outputElement.Add(outputName);
        
        var folderField = new TextField("Output Folder ")
        {
            value = _selectedFolderPath,
            isReadOnly = true // Make sure users can't edit this directly
        };
        
        
        outputElement.Add(folderField);

        // Button to open folder selection dialog
        var selectFolderButton = new Button(SelectFolder)
        {
            text = "Select Folder"
        };
        
        selectFolderButton.AddToClassList("select-folder-button");
        
        
        outputElement.Add(selectFolderButton);
        

        #endregion
        
        #region Element_1

        var element1 = new VisualElement();
        element1.AddToClassList("element-1");
        
        
        Label firstTextureSetLabel = new Label("RGBA Channel Packing");
        firstTextureSetLabel.AddToClassList("center-label");
        element1.Add(firstTextureSetLabel);
        
        #region RGBA texture set

        var textureContainer = new VisualElement();


        #region redChannel

            var red = new VisualElement();
            
            var redTexture = new IMGUIContainer(() => {
                _redChannel = (Texture2D)EditorGUILayout.ObjectField("", _redChannel, typeof(Texture2D), false);
            });
            
            
            ColorField defaultRed = new ColorField
            {
                value = _defaultRedChannel
            };

            defaultRed.RegisterValueChangedCallback(evt => {
                _defaultRedChannel = ToMonochrome(evt.newValue);
                defaultRed.value = _defaultRedChannel; // Update the ColorField's display
            });

            defaultRed.AddToClassList("color-field");
            red.Add(redTexture);
            red.Add(defaultRed);

        #endregion


        #region Green Channel

        var green = new VisualElement();
        
        var greenTexture = new IMGUIContainer(() => {
            _greenChannel = (Texture2D)EditorGUILayout.ObjectField("", _greenChannel, typeof(Texture2D), false);
        });
        
        ColorField defaultGreen = new ColorField
        {
            value = _defaultGreenChannel
        };

        defaultGreen.RegisterValueChangedCallback(evt => {
            _defaultGreenChannel = ToMonochrome(evt.newValue);
            defaultGreen.value = _defaultGreenChannel; // Update the ColorField's display
        });

        defaultGreen.AddToClassList("color-field");
        
        green.Add(greenTexture);
        green.Add(defaultGreen);

        #endregion
        
        
        #region blue Channel

        var blue = new VisualElement();
        
        var blueTexture = new IMGUIContainer(() => {
            _blueChannel = (Texture2D)EditorGUILayout.ObjectField("", _blueChannel, typeof(Texture2D), false);
        });
        
        ColorField defaultBlue = new ColorField
        {
            value = _defaultBlueChannel
        };

        defaultBlue.RegisterValueChangedCallback(evt => {
            _defaultBlueChannel = ToMonochrome(evt.newValue);
            defaultBlue.value = _defaultBlueChannel; // Update the ColorField's display
        });

        defaultBlue.AddToClassList("color-field");
        
        blue.Add(blueTexture);
        blue.Add(defaultBlue);
        

        #endregion
        
        
        #region alpha Channel

        var alpha = new VisualElement();
        
        var alphaTexture = new IMGUIContainer(() => {
            _alphaChannel = (Texture2D)EditorGUILayout.ObjectField("", _alphaChannel, typeof(Texture2D), false);
        });
        
        ColorField defaultAlpha = new ColorField
        {
            value = _defaultAlphaChannel
        };

        defaultAlpha.RegisterValueChangedCallback(evt => {
            _defaultAlphaChannel = ToMonochrome(evt.newValue);
            defaultAlpha.value = _defaultAlphaChannel; // Update the ColorField's display
        });

        defaultAlpha.AddToClassList("color-field");
        
        alpha.Add(alphaTexture);
        alpha.Add(defaultAlpha);
        

        #endregion

        
        red.AddToClassList("image_selection");
        blue.AddToClassList("image_selection");
        green.AddToClassList("image_selection");
        alpha.AddToClassList("image_selection");
        textureContainer.AddToClassList("texture-container");


        textureContainer.Add(red);
        textureContainer.Add(green);
        textureContainer.Add(blue);
        textureContainer.Add(alpha);
        
        element1.Add(textureContainer);

        #endregion
        
        #region Map Button

        var myButton = new Button(MapTextures)
        {
            text = "Map RGBA Textures"
        };
        myButton.AddToClassList("my-button");
        element1.Add(myButton);

        #endregion

        #endregion

        #region Info2

        var infoElement2 = new VisualElement();
        infoElement2.AddToClassList("info-element-1");
        
        
        // Add the info icon
        var infoIcon2 = new Image
        {
            image = EditorGUIUtility.IconContent("console.infoicon").image,
            style =
            {
                height = 16,
                width = 16,
                marginRight = 5, // Some spacing between icon and text\
            }
        };
        infoIcon2.style.marginLeft = new StyleLength(StyleKeyword.Auto);
        infoIcon2.style.marginRight = new StyleLength(StyleKeyword.Auto);
        
        infoElement2.Add(infoIcon2);

        // Add text next to the icon
        var infoText2 =
            new Label(
                "RGB channel is Diffuse Texture and alpha channel is opacity")
            {
                style =
                {
                    whiteSpace = WhiteSpace.Normal,
                    marginTop = new StyleLength(5),
                    marginLeft = new StyleLength(StyleKeyword.Auto),
                    marginRight = new StyleLength(StyleKeyword.Auto)
                }
            };
        
        infoElement2.Add(infoText2);
        

        #endregion
        
        
        #region Element_2

        var element2 = new VisualElement();
        element2.AddToClassList("element-1");
        element2.AddToClassList("element-2");
        
        //Label
        Label secondTextureSetLabel = new Label("(RGB)A Channel Packing");
        secondTextureSetLabel.AddToClassList("center-label");
        element2.Add(secondTextureSetLabel);



        var TextureSet2 = new VisualElement();
        TextureSet2.AddToClassList("texture-container");
        
        #region Diffuse

            var diffuse = new VisualElement();
            
            var diffuseTexture = new IMGUIContainer(() => {
                _diffuseTexture = (Texture2D)EditorGUILayout.ObjectField("", _diffuseTexture, typeof(Texture2D), false);
            });
            
            diffuse.Add(diffuseTexture);
            diffuse.AddToClassList("image_selection");
            

        #endregion 
        
        #region Opacity

            var opacity = new VisualElement();
            
            var opacityTexture = new IMGUIContainer(() => {
                _opacity = (Texture2D)EditorGUILayout.ObjectField("", _opacity, typeof(Texture2D), false);
            });
            
            opacity.Add(opacityTexture);
            opacity.AddToClassList("image_selection");
            

        #endregion
        
        TextureSet2.Add(diffuse);
        TextureSet2.Add(opacity);
        
        
        var mapTextureButton2 = new Button(MapTextures2)
        {
            text = "Map RGB-A Textures"
        };
        mapTextureButton2.AddToClassList("my-button-2");
        
        element2.Add(TextureSet2);
        element2.Add(mapTextureButton2);
        
        

        #endregion

        
        

        var root = new ScrollView();
        root.AddToClassList("root");
        
        root.Add(outputElement);
        root.Add(infoElement1);
        root.Add(element1);
        root.Add(infoElement2);
        root.Add(element2);
        
        
        rootVisualElement.Add(root);
    }
    
    
    private Color ToMonochrome(Color originalColor)
    {
        float grayValue = (originalColor.r + originalColor.g + originalColor.b) / 3f;
        return new Color(grayValue, grayValue, grayValue, originalColor.a);
    }
    
    private void SelectFolder()
    {
        _selectedFolderPath = EditorUtility.OpenFolderPanel("Select Folder", "", "");
        OnEnable();  // Refresh the UI
    }
    
    /// <summary>
    /// Maps R-G-B-A textures
    /// </summary>
    private void MapTextures()
    {
        Texture2D red, green, blue, alpha;

        if (_redChannel)
        {
            red = _redChannel;
        }
        else
        {
            
            Color[] newPixels = new Color[_selectedResolution * _selectedResolution];
            for (int i = 0; i < newPixels.Length; i++)
            {
                newPixels[i] = _defaultRedChannel;
            }
            
            
            red = new Texture2D(_selectedResolution, _selectedResolution);
            red.SetPixels(newPixels);
            red.Apply();
        }

        if (_greenChannel)
        {
            green = _greenChannel;
        }
        else
        {
            Color[] newPixels = new Color[_selectedResolution * _selectedResolution];
            for (int i = 0; i < newPixels.Length; i++)
            {
                newPixels[i] = _defaultGreenChannel;
            }
            
            
            green = new Texture2D(_selectedResolution, _selectedResolution);
            green.SetPixels(newPixels);
            green.Apply();
        }

        if (_blueChannel)
        {
            blue = _blueChannel;
        }
        else
        {
            Color[] newPixels = new Color[_selectedResolution * _selectedResolution];
            for (int i = 0; i < newPixels.Length; i++)
            {
                newPixels[i] = _defaultBlueChannel;
            }
            
            
            blue = new Texture2D(_selectedResolution, _selectedResolution);
            blue.SetPixels(newPixels);
            blue.Apply();
        }

        if (_alphaChannel)
        {
            alpha = _alphaChannel;
        }
        else
        {
            Color[] newPixels = new Color[_selectedResolution * _selectedResolution];
            for (int i = 0; i < newPixels.Length; i++)
            {
                newPixels[i] = _defaultAlphaChannel;
            }
            
            
            alpha = new Texture2D(_selectedResolution, _selectedResolution);
            alpha.SetPixels(newPixels);
            alpha.Apply();
        }
       
        
        CombineTextures(red, green, blue, alpha);
    }

    /// <summary>
    /// Maps RGB-A textures
    /// </summary>
    private void MapTextures2()
    {
        Debug.Log("Hello");
        
        if (_diffuseTexture && _opacity)
        {
            CombineTextures(_diffuseTexture, _opacity);
        }
        else
        {
            EditorUtility.DisplayDialog("Image missing", "Add textures to the proper fields",
                "Okay");
        }
    }

    private static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        // Create a temporary RenderTexture of the target size.
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        rt.filterMode = FilterMode.Bilinear;

        // Render the source texture onto the RenderTexture.
        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        // Read the rendered texture into a new Texture2D object.
        Texture2D result = new Texture2D(targetWidth, targetHeight);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        // Clean up.
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }
    
    private void CombineTextures(Texture2D red, Texture2D green, Texture2D blue, Texture2D alpha)
    {


        if (_selectedFolderPath == "")
        {
            EditorUtility.DisplayDialog("Output path missing", "Add a folder path, where you want to save the texture to",
                "Okay");
            return;
        }
        
        // Resize each texture to the target resolution.
        red = ResizeTexture(red, _selectedResolution, _selectedResolution);
        green = ResizeTexture(green, _selectedResolution, _selectedResolution);
        blue = ResizeTexture(blue, _selectedResolution, _selectedResolution);
        alpha = ResizeTexture(alpha, _selectedResolution, _selectedResolution);

        // Create a new texture with the desired resolution
        Texture2D combinedTexture = new Texture2D(_selectedResolution, _selectedResolution);

        // Read pixel data from the source textures
        Color[] redPixels = red.GetPixels();
        Color[] greenPixels = green.GetPixels();
        Color[] bluePixels = blue.GetPixels();
        Color[] alphaPixels = alpha.GetPixels();

        Color[] newPixels = new Color[_selectedResolution * _selectedResolution];

        for (int i = 0; i < newPixels.Length; i++)
        {
            // Get channel colors
            float r = redPixels[i].grayscale;
            float g = greenPixels[i].grayscale;
            float b = bluePixels[i].grayscale;
            float a = alphaPixels[i].grayscale;

            newPixels[i] = new Color(r, g, b, a);
        }

        combinedTexture.SetPixels(newPixels);
        combinedTexture.Apply();

        string filePath = Path.Combine(_selectedFolderPath, _outputFileName + ".png");

        // Check if the file already exists.
        if (File.Exists(filePath))
        {
            // Ask the user if they want to overwrite the existing file.
            bool overwrite = EditorUtility.DisplayDialog("File Exists", 
                "The file already exists. Do you want to overwrite?", 
                "Yes", "No");
            if (!overwrite)
            {
                // If not overwriting, add a suffix before the file extension.
                filePath = Path.Combine(_selectedFolderPath,  _outputFileName +".png");
        
                // You can add further logic to find a unique name if desired
            }
        }

        // Save the combined texture
        byte[] bytes = combinedTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Refresh the asset database in case you're saving within the Unity project
        AssetDatabase.Refresh();
        
        
    }
    
    private void CombineTextures(Texture2D diffuse, Texture2D opacity)
    {
        if (_selectedFolderPath == "")
        {
            EditorUtility.DisplayDialog("Output path missing", "Add a folder path, where you want to save the texture to",
                "Okay");
            return;
        }

        diffuse = ResizeTexture(diffuse, _selectedResolution, _selectedResolution);
        opacity = ResizeTexture(opacity, _selectedResolution, _selectedResolution);

        Color[] diffusePixels = diffuse.GetPixels();
        Color[] opacityPixels = opacity.GetPixels();
        
        Color[] newPixels = new Color[_selectedResolution * _selectedResolution];

        for (int i = 0; i < newPixels.Length; i++)
        {
            // Get channel colors
            float r = diffusePixels[i].r;
            float g = diffusePixels[i].g;
            float b = diffusePixels[i].b;
            float a = opacityPixels[i].grayscale;

            newPixels[i] = new Color(r, g, b, a);
        }

        Texture2D combinedTexture = new Texture2D(_selectedResolution, _selectedResolution);
        
        combinedTexture.SetPixels(newPixels);
        combinedTexture.Apply();
        
        
        string filePath = Path.Combine(_selectedFolderPath, _outputFileName + ".png");

        // Check if the file already exists.
        if (File.Exists(filePath))
        {
            // Ask the user if they want to overwrite the existing file.
            bool overwrite = EditorUtility.DisplayDialog("File Exists", 
                "The file already exists. Do you want to overwrite?", 
                "Yes", "No");
            if (!overwrite)
            {
                // If not overwriting, add a suffix before the file extension.
                filePath = Path.Combine(_selectedFolderPath,  _outputFileName +".png");
        
                // You can add further logic to find a unique name if desired
            }
        }

        // Save the combined texture
        byte[] bytes = combinedTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        // Refresh the asset database in case you're saving within the Unity project
        AssetDatabase.Refresh();
    }
    
}