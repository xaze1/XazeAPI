## How to install
Download the latest release's dll file and place it into `SCP Secret Laboratory > LabAPI > plugins > global`

To use it in your Plugin, simply add it as a Reference

## How to use
Even though this goes into the plugins folder, this is just a Framework for other Plugins
To use it in your Plugin, simply add it as a Reference to your project

I would advise you use `XazeAPI.API.Logging` over `LabAPI.Features.Console.Logger` for displaying messages to the console, 
but that is fully up to you

## Useful Systems
### CustomSSSSync
Allows syncing localized Server-Specific Settings
```typescript
CustomSSSSync.AddGlobalSettings([
    new SSGroupHeader("Example Settings"), 
    new SSTextSetting("This is a example!")
])
// Adds a global setting, which every player gets when joining
// You can make only specific players get it, by setting CustomSSSSync.SendOnJoinFilter

// To send a specific player a setting, you can use this:
CustomSSSSync.AddLocalSettings(Player.Host, [
    new SSGroupHeader("Example Local Setting"), 
    new SSTextSetting("Hello, " + Player.Host.DisplayName)
])
// You can also simply add/set the value
CustomSSSSync.DefinedSettings[Player.Host.RefenceHub] = [new SSGroupHeader("Pretty much nothing")]
```

### EffectStackManager
Allows the stacking of the same effect<br>
`Player.AddEffect<StatusEffectBase>(intensity, duration, max intensity)`
```typescript
Player.AddEffect<MovementBoost>(25, 5);
Player.AddEffect<MovementBoost>(5, 10);
// Player will have 30 Movement boost for 5s and then 5 Movement boost for another 5s

Player.AddEffect<DamageReduction>(() => Player.ReadyList.Count * 2, 30);
// Player gets 1% damage resistance for each player, for the next 30s
// Intensity gets recalculated, i.e don't worry about updating the Intensity manually

// To make it so a effect isn't removed from a player, unless you explicitly want it to, use 'CanBeRemoved'
var stack = new EffectStack() { Intensity = 5, CanBeRemoved = false };
Player.AddEffect<FogControl>(stack);
```

### LightSystem
Dynamic LightSourceToy system<br>
Updates the Light's Color, depending on the Config

```typescript
var primitive = PrimitiveObjectToy.Create(Vector3.zero);
var light = new LightSystem.LightConfig(primitive.Transform, [Color.red, Color.blue]);

Timing.CallDelayed(5f, light.Destroy);
// Creates a LightSourceToy, which follows the PrimitiveObjectToy and cycles it's light between red and blue (hue-change)
// Destroys the Light after 5s

var plrLight = new LightSystem.LightConfig(Player.Host, [Color.magenta, Color.yellow])
Timing.CallDelayed(30f, plrLight.Destroy);
// Creates a LightSourceToy that follows the Player
// Light becomes invisible, if player becomes invisible (globally)
// Light destroys itself, if the player disconnects
```