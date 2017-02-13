---
title: Alchemy Tutorial
layout: default
---

# Alchemy

##### Author: Treewyrm

*WIP: Still being converted to online format*

This guide explains the how Freelancer particle system (Alchemy) works and how to edit and create your own particle effects.

In Freelancer particle system is used extensively for dynamic visual effects such as weapon projectiles, engine exhausts, explosions, jumpholes and jump effects, environment elements such as rain at Leeds planetscape background or sand winds at Malta, even tiny dust particles in space. They’re also extensively used in real-time cutscenes and prominently visible in main menu backgrounds: animated star and ash clouds of a volcanic planet.

The particle system in Freelancer is quite similar to particle systems in other games and engines, although not at powerful or expressive as some are. Feature-wise it’s quite basic but for most of the part it works well enough.


## Tools
To create your own effects or edit existing ones you'll need several tools to work with Alchemy files (.ale).

As you'll work with plain text XML and INI files I would suggest using [Notepad++](http://notepad-plus-plus.org/) (free) or [Sublime Text](http://www.sublimetext.com/) (commercial), although my personal choice in anything related to XML is [Oxygen XML Developer](http://www.oxygenxml.com/xml_developer.html) (commercial).

To convert .ale files into readable and editable XML document and back you’ll need [Freelancer XML project](http://adoxa.110mb.com/freelancer/tools.html) utilities. UTFXML converts files from their original format and XMLUTF rebuilds them from your source XML files.

[CRCTool](http://adoxa.110mb.com/freelancer/tools.html) utility is necessary to generate CRC number for effect names when referencing them in Freelancer .ini files.

[UTFEditor](http://svn.the-starport.net/utfeditor) will come very handy to open and edit .txm files that contain bitmap images used for particles. You may need it if you’re going to make your own .txm files.

If you’d like to create your own images to use in particles any graphics editor that can work with alpha channel and save into .tga files (or better yet into DirectDraw Surface format .dds files) will do. In my case I’ll be using Adobe Photoshop CS6 with [Nvidia DDS plug-in](https://developer.nvidia.com/nvidia-texture-tools-adobe-photoshop).

Optionally BINI tools to convert compressed INI files into editable plain-text.

## Alchemy Structure

* Effects
* Emitters:
    - Cube emitter
    - Sphere emitter
    - Cone emitter
* Particles:
	- Basic particle
	- Direction particle
	- Perpendicular particle
	- Beam particle
	- Mesh particle
	- Effect particle (composite)
* Fields:
	- Radial field
	- Air field
	- Turbulence field
	- Gravity field
	- Collision field
	- Dust field

## Effects

A single effect groups together emitters, particles, fields and typically a single dummy node. These parts are collectively called ‘nodes’. Alchemy files (.ale) can contain multiple independent effects which can share same elements. This is useful to create a single file that would contain multiple effects related to same entity, for example a weapon alchemy file typically has three effects: projectile, hit explosion and muzzle flash effect for gun.

Effects are listed at the beginning of XML file in header section `<ALEffectLib/>.`

	<effect name="my_test_engine">
	    <unused>0, 0, 0; 0</unused>
        <fx>
            1, 32768, 1, 0xEE223B51
            2, 1, 0, my_test_engine_fire.emt
            3, 1, 0, my_test_engine_fire.app
        </fx>
        <pairs>
            2, 3 
        </pairs>
	</effect>

`<fx/>` element is a table of contents for effect. First name is identification number for node, each must be unique with one effect. Second number is ID number of parent node, used to create hierarchy structure. Third number is a type of node - this value is always 0 for regular nodes and 1 for dummy node. The last is node name, and like ID number it needs to be unique too.

`<unused/>` is just a sequence of four values that apparently do nothing, so leave them at 0
Through ID and parent ID numbers all nodes are in hierarchy within effect:

> Unless made dependent emitters should be children of a dummy node in effect. Without setting parent ID correctly emitters will not be attached to object in scene that has the effect, instead they’ll appear at the center of the system or the scene.

In example above the effect contains three nodes: dummy *(0xEE223B51)*, emitter *(my_test_engine_fire.emt)* and particle *(my_test_engine_fire.app)*.

For effect to now which emitter uses what particle to generate it must be defined in `<pairs/>` element. In this case a single pair which tells emitter that node with ID = 2 uses node with ID = 3: emitter *(my_test_engine_fire.emt)* uses particle *(my_test_engine_fire.app)*.

Hierarchy allows one element to be dependent on another. For example if emitter has child emitters then by moving this emitter all children elements will move as well.

### Emitters

Basic effect consists of at least one emitter and one particle it emits. Emitters spawn particles at random location within their shape volumes. Alchemy system provides three types: cube, sphere and cone.

One emitter can spawn only one particle type.

### Appearance

Appearance are the actual visual elements in particle system. Emitters and fields by themselves are invisible objects that manifest themselves only through particles. Typically a single appearance is a simple 2D bitmap that has coordinates in 3D space. It may have a vector to move in environment, as well as number of other properties such as color tint, opacity, size of image. Basic appearance has no geometry since it’s not a 3D model but a simple dot.

### Fields

Particles can be affected by additional forces called fields.

A single particle prototype can be affected only by one field, unfortunately, so pick wisely as you’ll not be able apply both gravity and radial fields.

## Property Types

Emitters, particles and fields have many properties to define their characteristics and their behavior.

Colors, transformation arrays, pressure forces, generation frequencies, particle lifespans and many others can be animated. Animation is keyframe-based by defining keyframe stops in object timeline to tell which property will have what value and when. Animation sequence can be played either once or indefinitely, that is until the object using this effect is deleted.

### Color
Used for particle property `BasicApp_Color`.

    <rgb_header type="4">
        0:
        <rgb type="4">
            0.0:  10,   0, 117
            0.5: 255, 0, 0
            1.0: 255, 252,   0
        </rgb>
    </rgb_header>

![Color diagram](../assets/images/alchemy-tutorial/particle-color.png)

### Number
Simple number property type is used for particle opacity, size, emitter frequency, pressure, etc. Animation is linear between keyframes.

    <float_header type="4">
        0:
        <float type="4">
            0.00: 1
            1.00: 0
        </float>
    </float_header>

For example property value for `BasicApp_Alpha` in particle (while also using example above for `BasicApp_Color`) would have this result:

![Alpha diagram](../assets/images/alchemy-tutorial/particle-alpha.png)

### Curve
Similar to type above except this property supports smoothing transition between keyframes called easing, this transition is defined through two additional numbers:

    <single type="4">
        0:
        <loop>
            0.0:   0,  0,  0
            4.0: 100,  0,  0
            8.0:  50,  0,  0
        </loop>
    </single>

Each keyframe in sequence contain four numbers: time, key value, ending velocity for previous keyframe, starting velocity between this and next keyframe. By default these values are typically left at 0, meaning that value transition speed increases from 0 at the beginning and slows down to 0 as it reaches resulting value defined in next keyframe. However sometimes you may want to make keyframe value transition in a linear fashion, in this case you need to take value difference between two keyframes and divide it by duration between them, and the result goes into corresponding velocity values. For example let’s say you’d like to make emitter do a complete rotation at a constant speed within eight seconds: 360°/8 =45°, so we set first keyframe velocity start and next keyframe velocity end to 45, like this:

    <loop type="16">
        0: 0, 0, 45
        8: 360,45, 0 
    </loop>

Emitter and field keyframe indexes are measured in seconds. Particles however depend on `Emitter_InitLifeSpan` property of the emitter they are generated from, so range from 0 to 1 is from each particle birth till when it expires.

### Transformation array

Used by `Node_Transform` to position node in scene, set rotation angles and scaling modifiers. This property type consists of nine curve type values: Y offset, X offset, Z offset, Y rotation, X rotation, Z rotation, Y scale, X scale and Z scale. Each property can be animated and can be made dependent on external variable.

    <effect type="0x105" name="Node_Transform">0x435 
        <single type="4" count="1">
            0: 0   <!-- Y offset -->
        </single>
        <single type="4" count="1"> 
            0: 0   <!-- X offset -->
        </single>
        <single type="4" count="1">
            0: 0   <!-- Z offset -->
        </single>
        <single type="4" count="1"> 
            0: 90   <!-- Y rotation -->
        </single>
        <single type="4" count="1">
            0: 0   <!-- X rotation -->
        </single>
        <single type="4" count="1"> 
            0: 0   <!-- Z rotation -->
        </single>
        <single type="4" count="1">
            0: 1   <!-- Y scale -->
        </single>
        <single type="4" count="1"> 
            0: 1   <!-- X scale -->
        </single>
        <single type="4" count="1">
           0: 1   <!-- Z scale -->
        </single>
    </effect>

### Shortening properties

If you’re not providing any animation to property you can use a shorter version.

Short notation is available only for number (`<float_header/>`) and curve (`<single/>`) types. If you use both static value and animation block then animation block values will override static value.

### External variable

Values in animations can also be influenced by external parameter.

    <float_header type="4" count="3">
        0.0: 2
        0.9: 8
        1.0: 16
    </float_header>

For example if effect is used for ship engine then variable range from 0 to 0.9 is mapped to idle (or engine kill) are maximum engine use, and range from 0.9 to 1 is mapped to cruise mode charging up at 0% to 100% respectively. So you can make some emitter to generate more particles when the ship at full speed, or even make additional emitters to work only when the ship is in cruise mode.
Be aware that using animated values in variable ranges of a single property may lead to unpredictable results as whenever external variable is changed it forces animation sequence to reset.

In THN scripts this parameter can be controlled and animated by `START_PSYS_PROP_ANIM` events.

### Easing types

#### _type attribute for transitions_
`<float type="4">`

|Value|Type|Graph|Notes|
|----|-----|-----|-----------|
|1|Linear|![Linear Graph](../assets/images/alchemy-tutorial/easing-linear.png)|
|2|Ease in|![Ease in Graph](../assets/images/alchemy-tutorial/easing-easein.png)|
|3|Ease out|![Ease out Graph](../assets/images/alchemy-tutorial/easing-easeout.png)|
|4|Ease in-out|![Ease in-out Graph](../assets/images/alchemy-tutorial/easing-easeinout.png)|
|5|Step|![Step Graph](../assets/images/alchemy-tutorial/easing-step.png)|Animation jumps immediately to value of keyframe and stays in that position until the next keyframe.|

These property types can’t be animated:

### Blending modes

A special type used only in `BasicApp_BlendInfo`. It consists of two values: blending mode for alpha channel of the used texture and mode for RGB channels. This affects appearance of particle texture and how its alpha channel and colors mix with each other and the environment.

### Text & others

Properties that expect text value (such as `Node_Name` and `BasicApp_TexName`). `Emitter_InitialParticles` accepts number and obviously can’t be animated nor affected by the external variable.

Several properties are Boolean types, such as `BasicApp_FlipTexU`, they accept either `true` or `false`. For complete list of node properties, what type they are and whether they can be animated refer to table below:

### Node types and properties reference

| Node type | Property | Description | Value | ♽ |
|-----------|----------|-------------|-------|---|
|**All nodes**|`Node_Name`|Unique node name|String|✔︎|
||`Node_Lifespan`|Lifespan of a whole node|Number (sec)|✘|
||`Node_Transform`|Transformation array|Array|✔︎|
|**All emitters**|`Emitter_LODCurve`|Level of detail optimization|Number|✔︎|
||`Emitter_InitialParticles`|Amount of particles that spawn at the beginning|Number|✘|
||`Emitter_Frequency`|Particle generation frequency (amount/sec)|Number (spawn/sec)|✔︎|
||`Emitter_MaxParticles`|Limit amount of simultaneous particles|Number|✔︎|
||`Emitter_EmitCount`|Unused|Number|✔︎|
||`Emitter_InitLifeSpan`|Lifespan of generated particles|Number (sec)|✔︎|
||`Emitter_Pressure`|Initial velocity provided to each particle generated|Number|✔︎|
||`Emitter_VelocityApproach`|Object movement vector multiplier applied to spawned particle|Number|✔︎|
|**FxCubeEmitter**|`CubeEmitter_Width`|Cube width|Number|✔︎|
||`CubeEmitter_Height`|Cube height|Number|✔︎|
||`CubeEmitter_Depth`|Cube depth|Number|✔︎|
||`CubeEmitter_MinSpread`|Spray cone minimum inner angle (hollow)|Number (degrees)|✔︎|
||`CubeEmitter_MaxSpread`|Spray cone outer angle|Number (degrees)|✔︎|
|**FxSphereEmitter**|`SphereEmiter_MinRadius`|Inner radius (hollow)|Number|✔︎|
||`SphereEmitter_MaxRadius`|Outer radius|Number|✔︎|
|**FxConeEmitter**|`ConeEmitter_MinRadius`|Cone sphere inner radius (hollow)|Number|✔︎|
||`ConeEmitter_MaxRadius`|Cone sphere outer radius (cone distance)|Number|✔︎|
||`ConeEmitter_MinSpread`|Cone minimum inner angle (hollow)|Number (degrees)|✔︎|
||`ConeEmitter_MaxSpread`|Cone maximum outer angle|Number (degrees)|✔︎|
|**All particles**|`Appearance_LODCurve`|Level of detail optimization|Number|✔︎|
||`BasicApp_TriTexture`|Particle shape is upside down triangle|Boolean|✘|
||`BasicApp_QuadTexture`|Particle shape is square|Boolean|✘|
||`BasicApp_MotionBlur`| *?* |Boolean|✘|
|**FxBasicAppearance**|`BasicApp_Color`|Particle color tint|Color|✔︎|
||`BasicApp_Alpha`|Particle opacity (0 transparent, 1 fully opaque)|Number|✔︎|
||`BasicApp_Size`|Sprite size|Number|✔︎|
||`BasicApp_HtoVAspect`|Horizontal to vertical aspect ratio|Number|✔︎|
||`BasicApp_Rotate`|Rotation amount|Number (radians)|✔︎|
||`BasicApp_TexName`|Texture name|String|✘|
||`BasicApp_BlendInfo`|Alpha channel and RGB channels blending mode|Special|✘|
||`BasicApp_UseCommonTexFrame`| *?* |Boolean|✘|
||`BasicApp_TexFrame`|Animation|Number|✔︎|
||`BasicApp_CommonTexFrame`|*Animation?*|Number|✔︎|
||`BasicApp_FlipTexU`|Flip texture horizontal|Boolean|✘|
||`BasicApp_FlipTexV`|Flip texture vertical|Boolean|✘|
|**FxRectAppearance**|`RectApp_CenterOnPos`|Centers texture|Boolean|✘|
||`RectApp_ViewingAngleFade`|Makes particle fade if it's too close to screen center|Boolean|✘|
||`RectApp_Scale`|Surface size|Number|✔︎|
||`RectApp_Length`|Surface length|Number|✔︎|
||`RectApp_Width`|Surface width|Number|✔︎|
|**FxParticleAppearance**|`ParticleApp_LifeName`|Effect displayed during particle lifespan|String|✘|
||`ParticleApp_DeathName`|Effect displayed when particle dies|String|✘|
||`ParticleApp_UseDynamicRotation`| *?* |Boolean|✘|
||`ParticleApp_SmoothRotation`| *?* |Boolean|✘|
|**FLBeamAppearance**|`BeamApp_DisablePlaceHolder`| *?* |Boolean|✘|
||`BeamApp_DupeFirstParticle`| *?* |Boolean|✘|
||`BeamApp_LineAppearance`| Disables texture |Boolean|✘|
|**FxRadialField**|`RadialField_Radius`|Radial field radius|Number|✔︎|
||`RadialField_Attenuation`| *?* |Number|✔︎|
||`RadialField_Magnitude`|Amount of force that is applied to particles. Positive value draws particle inwards and negative outwards.|Number|✔︎|
||`RadialField_Approach`|Force frequency|Number|✔︎|
|**FxGravityField**|`GravityField_Gravity`|Gravity force|Number|✔︎|
|**FxCollideField**|`CollideField_Reflectivity`|Collision reflectivity|Number|✔︎|
||`CollideField_Width`|Collision plane width|Number|✔︎|
||`CollideField_Height`|Collision plane height|Number|✔︎|
|**FxTurbulenceField**|`TurbulenceField_Magnitude`|Turbulence force strength|Number|✔︎|
||`TurbulenceField_Approach`|Force frequency|Number|✔︎|
|**FxAirField**|`AirField_Magniture`|Air field strength|Number|✔︎|
||`AirField_Approach`|Force frequency|Number|✔︎|


## Nodes

### Dummy

Dummy node has no properties and does nothing on its own, it is however a very important node – it links other nodes, such as emitters and fields, to actual object or hardpoint.

Typically all nodes are attached to dummy node; however it is not always the case. For example particles attached to its emitter will also move if the emitter moves, but if attached to dummy node they will move only with the object itself and not with emitter. Particles can be attached to another emitter or field too. Particles not attached to anything (and have their parent ID set to 32768, just as dummy node itself) will be unaffected by object movement. Emitters and fields can be attached to other emitters and fields. Emitters and particles detached from anything else will appear at the origin of the scene (center of system).

Dummy node is defined only in effect node list and never in `<AlchemyNodeLibrary/>.`

### Cube Emitter

#### _(FxCubeEmitter)_

Cube emitter is a simple box shape defined by width, length and depth that has a direction. Pressure applied to particles will throw them at a direction limited between `CubeEmitter_MinSpread` and `CubeEmitter_MaxSpread` angles.

| Name | Description | Type |
|------|-------------|------|
|**CubeEmitter_Width**|Cube Width|Number|
|**CubeEmitter_Height**|Cube Height|Number|
|**CubeEmitter_Depth**|Cube Depth|Number|
|**CubeEmitter_MinSpread**|Particle spread minimum angle|Number (degrees)|
|**CubeEmitter_MaxSpread**|Particle spread maximum angle|Number (degrees)|

### Sphere Emitter

#### _(FxSphereEmitter)_

Sphere emitter generates particles within a sphere shape defined by `SphereEmitter_MaxRadius`. If you wish it to be hollow set `SphereEmitter_MinRadius` to a value above 0 and below or equal to `SphereEmitter_MaxRadius`.

A positive pressure value applied to particles will throw them outwards while negative inwards.

![Sphere Emitter](../assets/images/alchemy-tutorial/sphere-emitter.png)

| Name | Description | Type |
|------|-------------|------|
|**SphereEmitter_MinRadius**|Inner sphere (hollow) radius|Number (degrees)|
|**SphereEmitter_MaxRadius**|Outer sphere radius|Number (degrees)|

### Cone Emitter

#### _(FxConeEmitter)_

Sphere emitter subset. Cone emitter is defined by sphere radius and cone inner and outer angles. Pressure vectors are the same as with sphere emitter.

![Cone Emitter](../assets/images/alchemy-tutorial/cone-emitter.png)

Here `ConeEmitter_MinSpread` is 11.25°, `ConeEmitter_MaxSpread` is 45° and `ConeEmitter_MinRadius` is half `ConeEmitter_MaxRadius`. Marked purple area is where particles will be generated.

You can create particle rings by having `ConeEmitter_MinSpread` and `ConeEmitter_MaxSpread` set to 90°. Ring width then will be a distance between `ConeEmitter_MaxRadius` and `ConeEmitter_MinRadius`.

| Name | Description | Type |
|------|-------------|------|
|**ConeEmitter_MinRadius**|Inner sphere (hollow) radius|Number (degrees)|
|**ConeEmitter_MaxRadius**|Outer sphere radius|Number (degrees)|
|**ConeEmitter_MinSpread**|Inner cone (hollow) angle|Number (degrees)|
|**ConeEmitter_MaxSpread**|Outer cone angle|Number (degrees)|

A common example would be volcanic eruption effect at one of the main menu backgrounds:

### Basic Appearance

#### _(FxBasicAppearance)_

A flat sprite that is always directed towards you.

### Directional Appearance

#### _(FxRectAppearance)_

Similar to basic particle but its normal however is pointing towards the center of the screen.

This type of particles is often used to create various ray effects

### Perpendicular Appearance

#### _(FxPerpAppearance)_

Similar as basic particle this one however is entirely flat.

Common examples are circles created by Liberty Dreadnaught engine.

Use node transformation scaling to adjust square length and width.

### Beam Appearance

#### _(FLBeamAppearance)_

As the name implies it is typically used for beam effects; however what this type does is connecting particles with two cross planes. Beam particle is used for various beam effects, contrails, and in lightning effect at Tekagi’s Arch 

![Beam Particle](../assets/images/alchemy-tutorial/beam-particle.png)

### Mesh Appearance

#### _(FxMeshAppearance)_

*Unimplemented in Freelancer and Librelancer*


### Effect Appearance

#### _(FxParticleAppearance)_

Composite effect particle provides no visual effect on its own, instead it uses other effects: one during particle life defined by `ParticleApp_LifeName` and `ParticleApp_DeathName` is shown when it dies. This particle becomes the invisible object itself “carrying” effect assigned to `ParticleApp_LifeName`.

Composite particles can create complex cascading effects, but be careful not to create infinite recursion.

### Radial Field

#### _(FxRadialField)_

Spherical field defined by `RadialField_Radius` it either attracts or detracts particles caught within. 

### Air Field

#### _(FxAirField)_

It has no dimension properties and simply affects all particles paired to it. Air field does not accelerate particles over time as with gravity field, instead they move at a constant speed.

### Turbulence Field

#### _(FxTurbulenceField)_

It has no dimension properties and simply affects all particles paired to it. Turbulence applies randomized force vector to particles defined by its `TurbulenceField_Magnitude` and frequency (hits/sec) of `TurbulenceField_Approach` properties. This creates shaking effect.

### Gravity Field

#### _(FxGravityField)_

Like turbulence field it has no dimensions and similarly applied to all particles paired to it. Gravity applies constant vector to particles defined by its. Vector direction can be controlled by rotating node, gravity value can also be a negative number.
Emitters provide initial push through `Emitter_Pressure` property and particles continue at a constant speed but gravity field applies force constantly and accelerates particles.

### Collision Field

#### _(FxCollideField)_

Acting as invisible flat wall defined by its `CollideField_Width` and `CollideField_Height` properties collision field prevents particles from passing through. Additionally it may deflect particles by providing positive value to `CollideField_Reflectivity`.

### Dust Field

#### _(FLDustField)_

*TODO: Needs test to see what it actually does*



