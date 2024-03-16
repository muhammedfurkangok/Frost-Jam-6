__________________________________________________________________________________________

Package "Animation Designer"
Version 1.0.2.1.1

Made by FImpossible Creations - Filip Moeglich
https://www.FilipMoeglich.pl
FImpossibleCreations@Gmail.com or FImpossibleGames@Gmail.com or Filip.Moeglich@Gmail.com

__________________________________________________________________________________________

Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Facebook: https://www.facebook.com/FImpossibleCreations
Twitter (@FimpossibleC): https://twitter.com/FImpossibleC

___________________________________________________


Package Contests:

User Manual: Check the .pdf file

DEMO - Animation Designer.unitypackage
Package contains scene with few example designed animations with Animation Designer and comparisons.

Animation Designer - Assembly Definitions.unitypackage (Supported since Unity 2017)
Assembly Definition files to speed up compilation time of your project (Fimpossible Directories will have no influence on compilation time of whole project)


__________________________________________________________________________________________
Description:

Innovative Animation Tweaking Tool For Unity Engine 

Upgrade Animation Clips with additional procedural motion and bake it!
Tweak Animation Clip pose and save it in new AnimationClip file.
Set additional motion to some bones and save new AnimationClip for use as animation variant.

NO TOYING WITH KEYFRAMES OR TIMELINE!
Meet new workflow for tweaking animation!
All you need to do is using few sliders and curve window.

You don't need to be an Animation Artist to work with this tool.
If you're an Animation Artist, this tool will give you new possibilities!

Keep original animation intact!
Save modified animation as separated AnimationClip file.
Store designer animation settings for multiple animation clips, in single preset file, so you can go back for tweaking animations design, in any time!

Prepare base animation without full polishing, 
let do it the procedural motion algorithm. Add some flappyness to small limbs and bake it.
Adjust Unity Humanoid animation retargeting errors inside editor.


MAIN FEATURES:
• Editor Animation Tweaking Tool
• Use already animated clips and save modified versions in new AnimationClip files
• Store Animation Modifiers Settings inside Presets
• Intuitive GUI
• Automatic Limbs Setup for Humanoid Rigs
• Supporting Generic Rigs
• Single Bone Modificators
• Limbs Modificators - Procedural Elasticness
• Limbs IK - For Hands, Bone Chains and Legs (Humanoid and Generic Rigs Supported)
• Legs IK Analyzer for adjusting Legs Motion Animation
• Supporting Root Motion Animations

__________________________________________________________________________________________

version 1.0.2.1.1
- Added experimental "Reverse Clip Time" button on the right of "Clip Duration Multiplier" field
- Added experimental "Clip Time Modify" curve
- Fixed export of not needed parameters for humanoid clips

version 1.0.2.1
- Added possibility to export Generic/Legacy animation clips out of Humanoid Rig
(Under "Additional Export Settings" when you work on humanoid rig)
- Additional options for helping out automatic grounding IK curve generating
- Added possibility to export grounding curve as animator parameter
version 1.0.2.0
-Original Clip Root motion disable options in the Setup tab under "Additional Settings For" foldout
-Experimental feature to enable multiple cycles, of the original animation motion
version 1.0.1.9
-Corrected effect of "Hips and legs precision boost" slider for humanoid rigs which will provide better non-sliding foots for idle animations
version 1.0.1.8
-Added foot glue tool which can be useful for fixing mocap sliding foots issue
-Using alt + left mouse button on some curve fields will result in keyframe added in current playback position
version 1.0.1.7
-Fixed root motion issue with humanoids with big scale offsets
-Grounding Foot IK "swings" and related parameters now should work correctly with the root motion
version 1.0.1.6
-Improved Looping algorithms
-Loops were lacking one last frame in the animation after exporting : fixed
-Fixed cleaning references after closing Designer Window (previously it resulted in bug of moving other object than edited one)
version 1.0.1.5
-Added support for exporting root motion of original clips (needs testing)
-Reworked humanoid root motion export
-Fixed issue with one-framed animation export
-Added "Additional Loop Frames" 3 algorithms for different loop purposes
-Few minor fixes

Version 1.0.1:
-Added support for baking root motion of generic rigs and improved humanoid root motion baking
-Fixed export legs quality boost slider effect
-Added experimental "stretching" slider for movement based elasticness
-After finishing export, character should behave much more calm
-Overwriting already existing animation clip files should work better now
-Modified looping algorithm to work more universally
-Improved "Motion Influence" parameter for elasticness to help out controll root motion
-Fixed "Calibrate" option freezing animated bones
-Few minor fixes

