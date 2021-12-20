#Boilerplate Project Guide
___

#Project Organization

##Main Assets
1. **Configuration**
  `Assets/00_Config/MainGameConfig`

2. **Main Scene**
  Assets/01_Scenes/main.scene

3. **Level Scenes Directory**
   Assets/01_Scenes/Levels

##Folder Structure
- `00_Config` All configuration related project assets.
- `01_Scenes`  
- `02_Scripts`
- `03_Prefabs`
- `04_VisualAssets`
- `Plugins` 3rd party libraries


##Guidelines
1. Single main scene. `Assets/01_Scenes/main.scene` Never create/maintain more than one main scene.
   >If required, create additional scenes with `__` prefix temporarily.
2. Main art reference scene is named `art.scene`, any additions and variants must be named `art_additionalDescription`.
2. Any other reference and experimental scenes must be named with `__` prefix.
3. Level scenes must be under `Assets/01_Scenes/Levels` and should be numbered.
4. All assets must be located in their respective directories **strictly**. eg. Prefabs in `03_Prefabs`.
5. 3rd Party Libraries should be in either root `Assets/` or `Assets/Plugins`

##Naming

- **Folders & Assets:** `UpperCamelCase`
   > Use `_` for variations
   eg. `SkyBoxMat_InGame`
   
- **Images:** `kebab-case`, eg. `ingame-green-button`
   
- **Temporary Files/Folders:** `__UpperCamelCase`

---

# Coding Guidelines
**.Net Design Guidelines\
https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/**

##Naming
- .Net Naming Guidelines\
  https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines
- For serialized private or public fields, use `_UpperCamelCase` \
    **eg.** `[SerializeField] private Player _Player;`
- Use `_lowerCamelCase` for private fields.   

##Unity
1. Don't use public fields for serialization, use `[SerializeField]`. eg. 
   
   >eg. `[SerializeField] private Player _Player;`
    
2. Don't use public fields at all (Except `Config`).\
   If a field is required to be accessed publicly, create properties or auto properties with required accessors.
   
         // Serialized private field with public property 
         [SerializeField] private float _PublicSerializedField;

         public float PublicSerializedField
         {
            get => _PublicSerializedField;
            set => _PublicSerializedField = value;
         }
   
         // Auto property  
         public float PublicField{get; set;)

3. Minimize the usage of `Awake()` and `Start()` Unity events. 
   Try to limit usage of these to independent initialization. \
Instead create an initialisation method such as `Init()` with required parameters
   to initialize object from the class that manages it,
   
      >eg. `Game` should initialize `Player` with `Player.Init()`.
   
      > When necessary; use `Awake()` for preparing initial state, caching, listening events. Make sure none of these operations depend on other classes.\
      Use `Start()` for initial actions that depend on other classes.
   
4. Never use `FindObjectOfType`, if required restructure your code to initialize your objects properly.
3. Don't initialize private fields at `Awake()` with `GetComponent<>`. And ensure their availability with `[RequiredComponent(typeof(Class))]`.
   Use following pattern instead to ensure accessibility at all times.
   
        private ClassName _className;

        // Always use this propery to access _className 
        public ClassName ClassName
        {
            get
            {
                if (_className == null) _className = GetComponent<ClassName>();
   
                // Use following if the ClassName is inherited from Unity Object, eg. Component, Monobehaviour.
                // if (!_className) _className = GetComponent<ClassName>();
   
                return _className;
            }
        }
   
5. Don't use `Component.isEnabled` or `GameObject.SetActive(bool)` to enable or disable behaviours. Create additional fields to control activity.
6. Try to restrict usage of `GameObject.SetActive(bool)` for visual purposes and separate view and controller classes when necessary.
   
   >eg. `Player` and `PlayerController`. Disabling Player GameObject would not affect the behaviour of its controller.
   
7. Create design parameters in `Config` class publicly and use these fields in runtime. Don't cache these unless product is shipping or necessary.
8. Utilize `OnValidate()` Unity event to prevent Edit Time problems.\
   Use `[RequiredComponent(typeof(Class))]` if a class strictly depends on existence of another one.
9. Always use namespaces, every game related class should be under the the namespace of the game
   
   >eg. `ThisGame.Management`.
    
8. Consider disabling **Domain Reloading** for Faster PlayMode initialisation. This might be not possible when using 3rd party libraries.\
   Don't use static fields in your classes or de-initialize these fields properly.
   More Info:\
   https://docs.unity3d.com/Manual/ConfigurableEnterPlayMode.html \
   https://blogs.unity3d.com/2019/11/05/enter-play-mode-faster-in-unity-2019-3/

---

#Included Packaged & Assets
- URP (Universal Render Pipeline)
- TextMeshPro
- Cinemachine
- QuickSearch
- GamePack
    - LeanTween (with modifications)

- Odin Inspector
- Nice Vibrations