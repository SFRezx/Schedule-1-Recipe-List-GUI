using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.StationFramework;
using MelonLoader;
using UnityEngine;
using MelonLoader.Utils;
[assembly: MelonInfo(typeof(RecipeListGui.RecipeListGuiClass), "Recipe List", "1.1.1", "Rezx, Community Updates By: ispa (Translation), pyst4r (effect colors)")]

namespace RecipeListGui
{
    public class RecipeListGuiClass : MelonMod
    {
        private static Dictionary<string, string> effectColors = new()
        {
            { "Anti-gravity", "#235BCD" },
            { "Athletic", "#75C8FD" },
            { "Balding", "#C79232" },
            { "Bright-Eyed", "#BEF7FD" },
            { "Calming", "#FED09B" },
            { "Calorie-Dense", "#FE84F4" },
            { "Cyclopean", "#FEC174" },
            { "Disorienting", "#D16546" },
            { "Electrifying", "#55C8FD" },
            { "Energizing", "#9AFE6D" },
            { "Euphoric", "#FEEA74" },
            { "Explosive", "#FE4B40" },
            { "Focused", "#75F1FD" },
            { "Foggy", "#B0B0AF" },
            { "Gingeritis", "#FE8829" },
            { "Glowing", "#85E459" },
            { "Jennerising", "#FE8DF8" },
            { "Laxative", "#763C25" },
            { "Lethal", "#AB2232" },
            { "Long faced", "#FED961" },
            { "Munchies", "#C96E57" },
            { "Paranoia", "#C46762" },
            { "Refreshing", "#B2FE98" },
            { "Schizophrenic", "#645AFD" },
            { "Sedating", "#6B5FD8" },
            { "Seizure-Inducing", "#FEE900" },
            { "Shrinking", "#B6FEDA" },
            { "Slippery", "#A2DFFD" },
            { "Smelly", "#7DBC31" },
            { "Sneaky", "#7B7B7B" },
            { "Spicy", "#FE6B4C" },
            { "Thought-Provoking", "#FEA0CB" },
            { "Toxic", "#5F9A31" },
            { "Tropic Thunder", "#FE9F47" },
            { "Zombifying", "#71AB5D" }
        };
        
        
        private class DataForFullIngredentsList
        {
            public string Name;
            public int Qnt;
        }

        // Dictionary for translating names of products, ingredients and ui text
        private static Dictionary<string, string> _translationDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static string Translate(string englishText)
        {
            if (!string.IsNullOrEmpty(englishText))
            {
                if (_translationDictionary.TryGetValue(englishText, out string translation))
                    return translation;
            }
            
            return englishText; //return the original text if the translation is not found
        }

        //load translations from a file
        private static void LoadTranslations()
        {
            string translationFilePath = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "Translations", "RecipeListGUI_translations.txt");

            Melon<RecipeListGuiClass>.Logger.Msg($"Trying to load translations from: {translationFilePath}");

            if (File.Exists(translationFilePath))
            {
                try
                {
                    string[] lines = File.ReadAllLines(translationFilePath);
                    Melon<RecipeListGuiClass>.Logger.Msg($"Read {lines.Length} lines from translation file");

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                            continue;

                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            _translationDictionary[key] = value;
                        }
                    }
                    Melon<RecipeListGuiClass>.Logger.Msg($"Loaded {_translationDictionary.Count} translations");
                }
                catch (Exception ex)
                {
                    Melon<RecipeListGuiClass>.Logger.Error($"Error loading translations: {ex.Message}");
                }
            }
            else
            {
                // Create a directory if it does not exist
                string translationDir = Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "Translations");
                if (!Directory.Exists(translationDir))
                {
                    try
                    {
                        Directory.CreateDirectory(translationDir);
                        Melon<RecipeListGuiClass>.Logger.Msg("Translation directory created. Please add RecipeListGUI_translations.txt file to the folder Mods/Translations/");
                    }
                    catch (Exception ex)
                    {
                        Melon<RecipeListGuiClass>.Logger.Error($"Failed to create directory for translations: {ex.Message}");
                    }
                }
                else
                {
                    Melon<RecipeListGuiClass>.Logger.Msg("Translation file not found. Please add RecipeListGUI_translations.txt Mods/Translations/");
                }
            }
        }
        
        private static MelonPreferences_Category _melonCfgCategory;
        private static MelonPreferences_Entry<float> _guiScale;
        private static MelonPreferences_Entry<KeyCode> _toggleKeyCode;
        private static MelonPreferences_Entry<KeyCode> _resetKeyCode;
        private static MelonPreferences_Entry<float> _transparency;
        private static MelonPreferences_Entry<Color> _pageColor;
        public override void OnInitializeMelon()
        {

            LoadTranslations();
            _melonCfgCategory = MelonPreferences.CreateCategory("RecipeListGUI");
            _guiScale = _melonCfgCategory.CreateEntry<float>("GUI_Scale", 1f);
            _toggleKeyCode = _melonCfgCategory.CreateEntry<KeyCode>("Open_And_Close_Button", KeyCode.F5);
            _resetKeyCode = _melonCfgCategory.CreateEntry<KeyCode>("Reset_Button", KeyCode.F6);
            _transparency = _melonCfgCategory.CreateEntry<float>("Transparency", 0.56f);
            _pageColor = _melonCfgCategory.CreateEntry<Color>("Page_Color", new Color32(51, 51, 51, 255));
            _melonCfgCategory.SetFilePath( Path.Combine(MelonEnvironment.GameRootDirectory, "Mods", "RecipeGUI.cfg"));
            _melonCfgCategory.SaveToFile();
            
            Melon<RecipeListGuiClass>.Logger.Msg($"{_toggleKeyCode.Value} to open");
            Melon<RecipeListGuiClass>.Logger.Msg($"{_resetKeyCode.Value} while gui is open to reset gui location");
        }

        public override void OnLateUpdate()
        {
            if (Input.GetKeyDown(_toggleKeyCode.Value))
            {
                ToggleMenu();
            }
            if (Input.GetKeyDown(_resetKeyCode.Value) && _guiShowen)
            {
                MelonLogger.Msg("Reset button pressed gui location reset");
                _productListPageRect = new Rect(100, 20, _productListPageRect.width, _productListPageRect.height);
                _favsListPageRect = new Rect(100, 325, _favsListPageRect.width, _favsListPageRect.height);
                _recipeResultPageRect = new Rect(600, 20, _recipeResultPageRect.width, _recipeResultPageRect.height);
                _settingsPageRect = new Rect(1000, 20, _settingsPageRect.width, _settingsPageRect.height);
            }
        }


        private static bool _guiShowen;
        private static void ToggleMenu()
        {
            _guiShowen = !_guiShowen;
            if (_guiShowen)
            {
                MelonEvents.OnGUI.Subscribe(DrawPages, 50);
                MelonLogger.Msg("GUI Enabled");

            }
            else
            {
                _listOfCreatedProducts = null;
                MelonEvents.OnGUI.Unsubscribe(DrawPages);
                MelonLogger.Msg("GUI Disabled");

            }
        }


        private static void DrawPages()
        {
            Color pageColor = _pageColor.Value;
            float guiScale = _guiScale.Value;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(guiScale, guiScale, 1));
            Texture2D transparentTex = new Texture2D(1, 1);
            transparentTex.SetPixel(0, 0, new Color(pageColor.r, pageColor.g, pageColor.b, _transparency.Value));
            transparentTex.Apply();

            GUIStyle customWindowStyle = new GUIStyle(GUI.skin.window);
            customWindowStyle.normal.textColor = Color.white;
            customWindowStyle.normal.background = transparentTex;
            customWindowStyle.onNormal.background = transparentTex;

            _productListPageRect = GUI.Window(651, _productListPageRect, (GUI.WindowFunction)ProductListPage, Translate("Product List"), customWindowStyle);
            _favsListPageRect = GUI.Window(652, _favsListPageRect, (GUI.WindowFunction)FavListPage, Translate("Favorite List"), customWindowStyle);
            if (_hasSelectedBud)
            {
                _recipeResultPageRect = GUI.Window(653, _recipeResultPageRect, (GUI.WindowFunction)RecipePage, Translate("Recipe"), customWindowStyle);
            }   

            _settingsPageRect = GUI.Window(654, _settingsPageRect, (GUI.WindowFunction)SettingsPage, Translate("Settings"), customWindowStyle);


        }
        
        private static Rect _settingsPageRect = new Rect(1000, 20, 250, 55);
        private static bool _shouldMinimizeSettingsPage = true;


        static void SettingsPage(int windowId)
        {
            if (_shouldMinimizeSettingsPage)
            {
                if (GUI.Button(new Rect(_settingsPageRect.width - 25, 2, 17, 17), "+"))
                {
                    _shouldMinimizeSettingsPage = false;

                    _settingsPageRect = new Rect(_settingsPageRect.x, _settingsPageRect.y, _settingsPageRect.width, 300);
                    return;
                }
                GUI.DragWindow(new Rect(0, 0, _settingsPageRect.width, _settingsPageRect.height));
                return;
            }
            else
            {
                if (GUI.Button(new Rect(_settingsPageRect.width - 25, 2, 17, 17), "-"))
                {
                    _shouldMinimizeSettingsPage = true;

                    _settingsPageRect = new Rect(_settingsPageRect.x, _settingsPageRect.y, _settingsPageRect.width, 55);
                }
            }

            
            Color32 pageColor = _pageColor.Value;

            _transparency.Value = GUI.HorizontalSlider(new Rect(25, 35, 200, 30), _transparency.Value, 0.0F, 1.0F);
            GUI.Label(new Rect(25, 19, _settingsPageRect.width, 20), $"{Translate("Transparency")}:{_transparency.Value.ToString()}");
            

            pageColor.r = (byte)GUI.HorizontalSlider(new Rect(25, 65, 200, 30), pageColor.r, 0, 255);
            GUI.Label(new Rect(25, 45, _settingsPageRect.width, 20), $"R:{pageColor.r}");

            pageColor.g = (byte)GUI.HorizontalSlider(new Rect(25, 95, 200, 30), pageColor.g, 0, 255);
            GUI.Label(new Rect(25, 75, _settingsPageRect.width, 20), $"G:{pageColor.g}");

            pageColor.b = (byte)GUI.HorizontalSlider(new Rect(25, 125, 200, 30), pageColor.b, 0, 255);
            GUI.Label(new Rect(25, 105, _settingsPageRect.width, 20), $"B:{pageColor.b}");
            _pageColor.Value = pageColor;
            
            GUI.Label(new Rect(25, 135, _settingsPageRect.width, 20), $"Scale");

            if (GUI.Button(new Rect(95, 155, 19, 19), "+"))
            {
                _guiScale.Value += 0.1f;
            }
            
            if (GUI.Button(new Rect(25, 155, 17, 17), "-"))
            {
                _guiScale.Value -= 0.1f;
            }
            if (GUI.Button(new Rect(65, 230, 125, 20), "Save"))
            {
                _melonCfgCategory.SaveToFile();
            }
            GUI.Label(new Rect(65, 153, _settingsPageRect.width, 20), $"{Math.Round(_guiScale.Value,2)}");
            GUI.DragWindow(new Rect(0, 0, _settingsPageRect.width, _settingsPageRect.height));
        }

        private static Il2CppSystem.Collections.Generic.List<ProductDefinition> GetlistOfCreatedProducts()
        {
            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                //Printy("Product object not found");
                return null;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                //Printy("ProductManager component not found");
                return null;
            }
            
            return productManagerComp.AllProducts;
        }


        private static Il2CppSystem.Collections.Generic.List<ProductDefinition> GetlistOf_FavProducts()
        {
            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                //Printy("Product object not found");
                return null;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                //Printy("ProductManager component not found");
                return null;
            }
            Il2CppSystem.Collections.Generic.List<ProductDefinition> favourites = ProductManager.FavouritedProducts;

            return favourites;
        }


        private static float _costToMake;
        static Il2CppSystem.Collections.Generic.List<string> GetIngredientList(ProductDefinition product, int selectedProductRecipe)
        {
            Il2CppSystem.Collections.Generic.List<string> outputLines = new();
            List<DataForFullIngredentsList> ingredientListTemp = new();

            _costToMake = 0;
            //Printy($"Selected {product.name} it has {product.Recipes.Count} recipes.");
            ProcessProduct(product, outputLines, selectedProductRecipe, true, ingredientListTemp);

            outputLines.Add("-----------------------------------------------------------------------------------");
            outputLines.Add($"{Translate("Market Price")} <b><color=#3ebf05>{product.MarketValue}</color></b>, {Translate("Addictiv")}: {Math.Round(product.GetAddictiveness())}, {Translate("Cost")}: <b><color=#ff3737>{_costToMake}</color></b>, {Translate("After Cost")}: <b><color=#3ebf05>{product.MarketValue - _costToMake}</color></b> ({Translate("base ingredients not included")})");
            outputLines.Add("-----------------------------------------------------------------------------------");

            foreach (var ingredient in ingredientListTemp)
            {
                outputLines.Add($"{ingredient.Qnt}x {Translate(ingredient.Name)} ");
            }
            return outputLines;
        }
        
        
      
                
                
        static void ProcessProduct(ProductDefinition product, Il2CppSystem.Collections.Generic.List<string> outputLines, int recipeToUse, bool isSelectedProductRecipe, System.Collections.Generic.List<DataForFullIngredentsList> ingredientListTemp)
        {
            //Printy($"ProcessProduct recipeToUse {recipeToUse}");

            if (!isSelectedProductRecipe && product == _selectedBud)
            {
                return;
            }

            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                //Printy("ProcessProduct: Product object not found");
                return;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                //Printy("ProcessProduct: ProductManager component not found");
                return;
            }

            if (product.Recipes.Count >= 1 && !productManagerComp.DefaultKnownProducts.Contains(product))
            {
                var recipes = product.Recipes;

                StationRecipe recipe = recipes.ToArray().ToList()[recipeToUse]; // use the selected recipe 

                if (isSelectedProductRecipe) // check if the recipe is the for the selected product
                {
                    outputLines.Add($"{Translate("Recipe for")} <b>{Translate(recipe.RecipeTitle)}</b>");
                    string effects = "";
                    foreach (Effect effect in product.Properties)
                    {
                        
                        string colorHex = effectColors.ContainsKey(effect.Name) ? effectColors[effect.Name] : "#FFFFFF";
                        string coloredEffect = $"<b><color={colorHex}>{Translate(effect.Name)}</color></b>";

                        if (string.IsNullOrEmpty(effects))
                        {
                            effects = coloredEffect;
                        }
                        else
                        {
                            effects += $", {coloredEffect}";
                        }
                    }
                    outputLines.Add(effects);
                    outputLines.Add($"-----------------------------------------------------------------------------------");

                    isSelectedProductRecipe = false;
                }
                else
                {
                    string recipeTitle = Translate(recipe.RecipeTitle);
                    outputLines.Add($"---------------------------------------------------");
                    outputLines.Add($"{Translate("Recipe")}: <b>{recipeTitle}</b>");
                }
                // list for products with their own recipe
                Il2CppSystem.Collections.Generic.List<ProductDefinition> nestedProducts = new Il2CppSystem.Collections.Generic.List<ProductDefinition>();

                foreach (var ingredient in recipe.Ingredients)
                {
                    string cat = ingredient.Item.Category.ToString();

                    if (cat == "Consumable" || cat == "Ingredient")
                    {
                        StorableItemDefinition prop = ingredient.Item.TryCast<StorableItemDefinition>();
                        //PropertyItemDefinition prop = ingredient.Item as PropertyItemDefinition;}

                        if (prop != null)
                        {
                            outputLines.Add($"{ingredient.Quantity}x {Translate(prop.Name)} {prop.BasePurchasePrice}$");
                            // list for doing ingredient qnt count
                            var existingIngredient = ingredientListTemp.ToArray().ToList().FirstOrDefault(i => i.Name == prop.Name);
                            if (existingIngredient != null)
                            {
                                existingIngredient.Qnt += ingredient.Quantity;
                            }
                            else
                            {
                                ingredientListTemp.Add(new DataForFullIngredentsList() { Name = prop.Name, Qnt = ingredient.Quantity });
                            }

                            _costToMake += prop.BasePurchasePrice * ingredient.Quantity;
                            if (!ingredientIcons.ContainsKey(prop.Name))
                            {
                                ingredientIcons.Add(prop.Name,prop.Icon);
                            }

                        }
                        else
                        {
                            outputLines.Add($"{ingredient.Quantity} Ingredient: {ingredient.Item}");
                            Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProduct: prop null {cat}");
                        }
                    }
                    else if (cat == "Product")
                    {
                        // For product ingredients, print the ingredient line first.
                        ProductDefinition drug = ingredient.Item.TryCast<ProductDefinition>();
                        if (drug != null)
                        {
                            outputLines.Add($"{ingredient.Quantity}x {Translate(drug.Name)}");
                            // If this product has a recipe store it to process after all normal ingredients
                            if (drug.Recipes.Count >= 1)
                            {
                                nestedProducts.Add(drug);
                            }
                        }
                        else
                        {
                            outputLines.Add($"{ingredient.Quantity}xIngredient: {ingredient.Item}");
                            Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProduct: Drug null {cat}");
                        }
                    }
                    else
                    {
                        outputLines.Add($"{ingredient.Quantity}xIngredient: {ingredient.Item}");
                        Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProduct: Unknown Category {cat}");
                    }
                }

                // after listing all ingredients for current product repeat for any drug products found in ingredient list.
                foreach (var nestedProduct in nestedProducts)
                {
                    ProcessProduct(nestedProduct, outputLines, 0, false, ingredientListTemp); // pass 0 for recipe to use because the nested isnt the selected dumbass
                }
            }
        }


        private static Il2CppSystem.Collections.Generic.List<string> _ingredientListRecipePage;
        private static Vector2 _recipeResultPageScrollViewVector = Vector2.zero;
        private static Rect _recipeResultPageRect = new Rect(600, 20, 600, 600);
        private static bool _hasSelectedProductRecipe;
        private static int _selectedProductRecipeIndex;
        private static ProductDefinition _lastSelectedBud;
        private static Dictionary<string, Sprite> ingredientIcons = new();

        static void RecipePage(int windowId)
        {
            Rect resizeHandleRect = new Rect(_recipeResultPageRect.width - 50, _recipeResultPageRect.height - 50, 25, 25);
            GUI.Box(resizeHandleRect, "");

            // close button
            if (GUI.Button(new Rect(_recipeResultPageRect.width - 45, 20, 30, 30), "X"))
            {
                _selectedBud = null;
                _hasSelectedBud = false;
                return;
            }
            // if product has more then 1 recipe make a list otherwise just load the only recipe
            //_selectedProductRecipeIndex is set to 0 when a new product is selected
            if (_selectedBud.Recipes.Count > 1)
            {
                if (_hasSelectedProductRecipe)
                {
                    // Back button
                    if (GUI.Button(new Rect(_recipeResultPageRect.width - 85, 20, 30, 30), Translate("B")))
                    {
                        _hasSelectedProductRecipe = false;
                        _ingredientListRecipePage = null;
                    }
                }


                if (!_hasSelectedProductRecipe)
                {
                    for (int i = 0; i < _selectedBud.Recipes.Count; i++)
                    {
                        if (GUI.Button(new Rect(_recipeResultPageRect.width / 3, 50 + 20 * i, 200, 20), $"{Translate("Recipe")} {i + 1}"))
                        {
                            _hasSelectedProductRecipe = true;

                            _selectedProductRecipeIndex = i;
                        }
                    }

                    ProcessResize(resizeHandleRect);

                    GUI.DragWindow(new Rect(0, 0, _recipeResultPageRect.width, _recipeResultPageRect.height - 30));

                    return;
                }

            }

            if (_selectedBud != null)
            {
                // get new ingredientList when selection changes.
                if (_ingredientListRecipePage == null || _selectedBud != _lastSelectedBud)
                {
                    _ingredientListRecipePage = GetIngredientList(_selectedBud, _selectedProductRecipeIndex);

                    _lastSelectedBud = _selectedBud;
                }

                _recipeResultPageScrollViewVector = GUI.BeginScrollView(new Rect(55, 20, _recipeResultPageRect.width - 55, _recipeResultPageRect.height), _recipeResultPageScrollViewVector, new Rect(0, 0, _recipeResultPageRect.width - 55, _ingredientListRecipePage.Count * 30));

                var ingredientListTemp = _ingredientListRecipePage.ToArray().ToList();
                
                // make lables with the info from GetIngredientList
                for (int i = 0; i < _ingredientListRecipePage.Count; i++)
                {
                    
                    string currentingredient = ingredientListTemp[i];
                    if (!currentingredient.StartsWith(" ") && !currentingredient.StartsWith("Price") && !currentingredient.StartsWith("Recipe") && !currentingredient.StartsWith("----"))
                    {
                        string[] currentingredientSplit = currentingredient.Split(' ');
                        string ingredientFromSplit = "";
                        string lastWord = currentingredientSplit.Last();
                        
                        if (lastWord.Contains("$")) // remove the ingredient quantity from the start and price from end then rebuild string
                        {
                            ingredientFromSplit = string.Join(" ", currentingredientSplit.Skip(1).Take(currentingredientSplit.Length - 2));
                        }
                        else // remove the ingredient quantity from the start then rebuild string
                        {
                            ingredientFromSplit = string.Join(" ", currentingredientSplit.Skip(1));
                        }

                        ingredientFromSplit = ingredientFromSplit.Trim();
                        

                        Sprite icon = ingredientIcons.ContainsKey(ingredientFromSplit) ? ingredientIcons[ingredientFromSplit] : null;
                        if (icon != null)
                        {
                            GUI.DrawTexture(new Rect(8, 40 + (20 * i), 35, 22), icon.texture);
                        }
                    }


                    GUI.Label(new Rect(50, 40 + (20 * i), _recipeResultPageRect.width - 75, 20), ingredientListTemp[i]);
                }
                GUI.EndScrollView();
            }
            
            // resize
            ProcessResize(resizeHandleRect);
            GUI.DragWindow(new Rect(0, 0, _recipeResultPageRect.width, _recipeResultPageRect.height - 30));
        }
        

        private static Vector2 _productListPageScrollViewVector = Vector2.zero;
        private static Rect _productListPageRect = new Rect(100, 20, 295, 55);
        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? _listOfCreatedProducts;
        private static bool _hasSelectedProductType;
        private static string _typeOfDrugToFilter = "";
        private static bool _hasSelectedBud;
        private static ProductDefinition _selectedBud;
        private static bool _shouldMinimizeProductListPage = true;
        private static bool _sortProductListPageByPrice = false;
        private static bool _sortProductListPageByIngredientCount = false;
        private static List<DetailedProduct> _detailedListOfCreatedProducts = new();
        private static bool _shouldShowDetailedView = false;

        
        static void ProductListPage(int windowID)
        {

            if (_shouldMinimizeProductListPage)
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 25, 2, 18, 17), "+"))
                {
                    _shouldMinimizeProductListPage = false;

                    _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, _productListPageRect.width, 300);
                }
                GUI.DragWindow(new Rect(20, 10, 500, 500));
                return;
            }
            else
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 25, 2, 17, 17), "-"))
                {
                    _shouldMinimizeProductListPage = true;

                    _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, _productListPageRect.width, 55);
                    return;
                }
            }

            _listOfCreatedProducts ??= GetlistOfCreatedProducts();
            if (_listOfCreatedProducts == null)
            {
                return;
            }


            if (!_hasSelectedProductType)
            {
                if (GUI.Button(new Rect(50, 20, 200, 20), Translate("Marijuana")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Marijuana";
                }
                if (GUI.Button(new Rect(50, 40, 200, 20), Translate("Methamphetamine")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Methamphetamine";
                }
                if (GUI.Button(new Rect(50, 60, 200, 20), Translate("Cocaine")))
                {
                    _hasSelectedProductType = true;
                    _typeOfDrugToFilter = "Cocaine";
                }
                if (GUI.Button(new Rect(50, 80, 200, 20), Translate("All Products")))
                {
                    _hasSelectedProductType = true;
                }

                GUI.DragWindow(new Rect(40, 10, 500, 500));

                if (!_hasSelectedProductType)
                {
                    return;
                }
            }
            else  // When a product type is selected
            {
                if (GUI.Button(new Rect(_productListPageRect.width - 37, 20, 29, 27), "B"))
                {
                    _shouldShowDetailedView = false;
                    _hasSelectedProductType = false;
                    _typeOfDrugToFilter = "";
                    _productListPageScrollViewVector = Vector2.zero;
                    _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, 295, 300);

                }
                
                if (GUI.Button(new Rect(_productListPageRect.width - 37, 50, 29, 27), _sortProductListPageByPrice ? "<b><color=#3ebf05>$</color></b>" : "$"))
                {
                    _sortProductListPageByIngredientCount = false;
                    _sortProductListPageByPrice = !_sortProductListPageByPrice;
                    _productListPageScrollViewVector = Vector2.zero;
                }
                
             
                if (GUI.Button(new Rect(_productListPageRect.width - 37, 80, 29, 27), _shouldShowDetailedView ? "<b><color=#3ebf05>DV</color></b>" : "DV"))
                {
                    _shouldShowDetailedView = !_shouldShowDetailedView;

                    if (_shouldShowDetailedView)
                    {
                        _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, 400, 300);
                    }
                    else
                    {
                        _productListPageRect = new Rect(_productListPageRect.x, _productListPageRect.y, 295, 300);
                    }
                }
                
            }
            
            int spacer = 0;
            if (_shouldShowDetailedView) // lol
            {
                if (_detailedListOfCreatedProducts.Count < _listOfCreatedProducts.Count)
                {
                    foreach (ProductDefinition product in _listOfCreatedProducts)
                    {
                        if (!_detailedListOfCreatedProducts.Any(bs => bs.Product == product))
                        {
                            _costToMakeDetailedProductList = 0;
                            _ingredientsNeededForCraft = 0;
                            ProcessProducts(product, true);
                            _detailedListOfCreatedProducts.Add(new DetailedProduct() { Product = product,CostToMake = _costToMakeDetailedProductList, IngredientsNeededForCraft = _ingredientsNeededForCraft });
                            
                        }
                    }
                }

                var sortedProducts = _detailedListOfCreatedProducts;
                if (_typeOfDrugToFilter != "")
                {
                    sortedProducts = sortedProducts.Where(bs => bs.Product.DrugType.ToString() == _typeOfDrugToFilter).ToList();
                }
                if (GUI.Button(new Rect(_productListPageRect.width - 37, 110, 29, 27), _sortProductListPageByIngredientCount ? "<b><color=#3ebf05>I</color></b>" : "I"))
                {
                    _sortProductListPageByPrice = false;
                    _sortProductListPageByIngredientCount = !_sortProductListPageByIngredientCount;

                }

                if (_sortProductListPageByIngredientCount)
                {
                    sortedProducts = sortedProducts.OrderBy(bs => bs.IngredientsNeededForCraft).ToList();
                    
                }else if (_sortProductListPageByPrice)
                {
                    sortedProducts = sortedProducts.OrderByDescending(bs => bs.Product.MarketValue - bs.CostToMake).ToList();
                }
                
                _productListPageScrollViewVector = GUI.BeginScrollView(new Rect(10, 20, 420, 300), _productListPageScrollViewVector, new Rect(0, 0, 300, sortedProducts.Count * 20 + 20));

                foreach (var detailedProduct in sortedProducts)
                {
                    if (GUI.Button(new Rect(20, 20 * spacer, 160, 20), Translate(detailedProduct.Product.Name)))
                    {
                        _selectedBud = detailedProduct.Product;
                        _hasSelectedBud = true;
                        _ingredientListRecipePage = null;
                        _hasSelectedProductRecipe = false;
                        _selectedProductRecipeIndex = 0;
                    }
                    if (GUI.Button(new Rect(0, 20 * spacer, 22, 20), "F"))
                    {
                        SetProductFav(detailedProduct.Product.ID,true);
                    }
                    GUI.Label(new Rect(185, 20 * spacer, _productListPageRect.width-10, 20), $"{Translate("Profit")}: <b><color=#3ebf05>{detailedProduct.Product.MarketValue - detailedProduct.CostToMake}</color></b>  {Translate("Ingredients")}: <b><color=#ff3737>{detailedProduct.IngredientsNeededForCraft}</color></b> ");
                    spacer++;
                }
            }
            else
            {
                var sortedProducts = _listOfCreatedProducts.ToArray().ToList();
                if (_typeOfDrugToFilter != "")
                {
                    sortedProducts = sortedProducts.Where(product => product.DrugType.ToString() == _typeOfDrugToFilter).ToList();
                }


                if (_sortProductListPageByPrice)
                {
                    sortedProducts = sortedProducts.OrderByDescending(product => product.MarketValue).ToList();
                }
                _productListPageScrollViewVector = GUI.BeginScrollView(new Rect(55, 20, 420, 300), _productListPageScrollViewVector, new Rect(0, 0, 300, sortedProducts.Count * 20 + 20));

                // i should probably not render buttons that are offscreen or do a page system but 2 brain cells
                foreach (var createdProduct in sortedProducts)
                {
                
                    if (GUI.Button(new Rect(0, 20 * spacer, 160, 20), Translate(createdProduct.name)))
                    {
                        //Printy($"Selected {createdProduct.name}");
                        _selectedBud = createdProduct;
                        _hasSelectedBud = true;
                        _ingredientListRecipePage = null;
                        _hasSelectedProductRecipe = false;
                        _selectedProductRecipeIndex = 0;
                    }
                    GUI.Label(new Rect(165, 20 * spacer, _productListPageRect.width-10, 20), $"<b><color=#3ebf05>{createdProduct.MarketValue}</color></b>$");
                    spacer++;
                }
            }
            
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(20, 10, 500, 500));
        }
        private class DetailedProduct
        {
            public ProductDefinition Product; 
            public float CostToMake;
            public float IngredientsNeededForCraft;
        }
        
        private static int _ingredientsNeededForCraft;
        private static float _costToMakeDetailedProductList;

        static void ProcessProducts(ProductDefinition product, bool isSelectedProductRecipe)
        {
            if (!isSelectedProductRecipe && product == _selectedBud)
            {
                return;
            }

            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                return;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                return;
            }

            if (product.Recipes.Count >= 1 && !productManagerComp.DefaultKnownProducts.Contains(product))
            {
                var recipes = product.Recipes;

                StationRecipe recipe = recipes.ToArray().ToList()[0]; 

                // list for products with their own recipe
                Il2CppSystem.Collections.Generic.List<ProductDefinition> nestedProducts = new Il2CppSystem.Collections.Generic.List<ProductDefinition>();

                foreach (var ingredient in recipe.Ingredients)
                {
                    string cat = ingredient.Item.Category.ToString();

                    if (cat == "Consumable" || cat == "Ingredient")
                    {
                        StorableItemDefinition prop = ingredient.Item.TryCast<StorableItemDefinition>();
                        //PropertyItemDefinition prop = ingredient.Item as PropertyItemDefinition;}

                        if (prop != null)
                        {
                            _ingredientsNeededForCraft++;
                            _costToMakeDetailedProductList += prop.BasePurchasePrice * ingredient.Quantity;
                        }
                        else
                        {
                            Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProducts: prop null {cat}");
                        }
                    }
                    else if (cat == "Product")
                    {
                        // For product ingredients, print the ingredient line first.
                        ProductDefinition drug = ingredient.Item.TryCast<ProductDefinition>();
                        if (drug != null)
                        {
                            // If this product has a recipe store it to process after all normal ingredients
                            if (drug.Recipes.Count >= 1)
                            {
                                nestedProducts.Add(drug);
                            }
                        }
                        else
                        {
                            Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProducts: Drug null {cat}");
                        }
                    }
                    else
                    {
                        Melon<RecipeListGuiClass>.Logger.Msg($"ERROR ProcessProducts: Unknown Category {cat}");
                    }
                }

                // after listing all ingredients for current product repeat for any drug products found in ingredient list.
                foreach (var nestedProduct in nestedProducts)
                {
                    ProcessProducts(nestedProduct,false);
                }
            }
        }

        
        private static Vector2 _favsListPageScrollViewVector = Vector2.zero;
        private static Rect _favsListPageRect = new Rect(100, 325, 300, 55);
        private static Il2CppSystem.Collections.Generic.List<ProductDefinition>? _listOf_FavsProducts;
        private static bool _shouldMinimizeFavListPage = true;
        private static bool _sortFavListPageByPrice = false;

        static void FavListPage(int windowID)
        {

            if (_shouldMinimizeFavListPage)
            {
                if (GUI.Button(new Rect(_favsListPageRect.width - 25, 2, 18, 17), "+"))
                {
                    _shouldMinimizeFavListPage = false;

                    _favsListPageRect = new Rect(_favsListPageRect.x, _favsListPageRect.y, 295, 300);
                    return;
                }
                GUI.DragWindow(new Rect(20, 10, 500, 500));
                return;

            }
            else
            {
                if (GUI.Button(new Rect(_favsListPageRect.width - 25, 2, 17, 17), "-"))
                {
                    _shouldMinimizeFavListPage = true;

                    _favsListPageRect = new Rect(_favsListPageRect.x, _favsListPageRect.y, 295, 55);
                    return;
                }
            }
            // filter button
            if (GUI.Button(new Rect(_favsListPageRect.width - 37, 20, 29, 27), _sortFavListPageByPrice ? "<b><color=#3ebf05>$</color></b>" : "$"))
            {
                _sortFavListPageByPrice = !_sortFavListPageByPrice;
                _favsListPageScrollViewVector = Vector2.zero;
            }
            
            _listOf_FavsProducts ??= GetlistOf_FavProducts();
            if (_listOf_FavsProducts == null)
            {
                //Printy("listOf_FavsProducts is null");
                return;
            }

            
            var filteredFavProducts = _listOf_FavsProducts.ToArray().ToList();
            
            if (_sortFavListPageByPrice)
            {
                filteredFavProducts = filteredFavProducts.OrderByDescending(product => product.MarketValue).ToList();
            }

            _favsListPageScrollViewVector = GUI.BeginScrollView(new Rect(10, 20, 300, 300), _favsListPageScrollViewVector, new Rect(0, 0, 300, filteredFavProducts.Count * 20 + 10));

            int spacer = 0;
            foreach (var favProduct in filteredFavProducts)
            {
                if (GUI.Button(new Rect(45, 20 * spacer, 160, 20), Translate(favProduct.name)))
                {
                    //Printy($"Selected Fav: {favProduct.name}");
                    _selectedBud = favProduct;
                    _hasSelectedBud = true;
                    _ingredientListRecipePage = null;
                    _hasSelectedProductRecipe = false;
                    _selectedProductRecipeIndex = 0;
                }
                if (GUI.Button(new Rect(25, 20 * spacer, 22, 20), "X"))
                {
                    SetProductFav(favProduct.ID,false);
                }
                // draw price lable
                GUI.Label(new Rect(209, 20 * spacer, 50, 20), $"<b><color=#3ebf05>{favProduct.MarketValue}</color></b>$");

                spacer++;
            }
            GUI.EndScrollView();
            GUI.DragWindow(new Rect(20, 10, 500, 500));
        }


        static void SetProductFav(string id,bool doFavorite)
        {
            GameObject productObject = GameObject.Find("@Product");
            if (productObject == null)
            {
                return;
            }
            ProductManager productManagerComp = productObject.GetComponent<ProductManager>();
            if (productManagerComp == null)
            {
                return;
            }
            productManagerComp.SetProductFavourited(id,doFavorite);
        }
        
        
        // ProcessResize was ai generated because 2 brain cells
        private static bool _isResizing;
        private static Vector2 _initialMousePosition;
        private static Rect _initialWindowRect;
        private static Vector2 _minWindowSize = new Vector2(375, 400);
        static void ProcessResize(Rect handleRect)
        {
            Event currentEvent = Event.current;
            Vector2 mousePos = currentEvent.mousePosition;
            Vector2 screenMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    if (handleRect.Contains(mousePos))
                    {
                        _isResizing = true;
                        _initialMousePosition = screenMousePos;
                        _initialWindowRect = _recipeResultPageRect;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isResizing)
                    {
                        Vector2 currentMousePos = screenMousePos;
                        float newWidth = Mathf.Max(_minWindowSize.x, _initialWindowRect.width + (currentMousePos.x - _initialMousePosition.x));
                        float newHeight = Mathf.Max(_minWindowSize.y, _initialWindowRect.height + (currentMousePos.y - _initialMousePosition.y));

                        _recipeResultPageRect.width = newWidth;
                        _recipeResultPageRect.height = newHeight;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_isResizing)
                    {
                        _isResizing = false;
                        currentEvent.Use();
                    }
                    break;
            }
        }
    }
}
