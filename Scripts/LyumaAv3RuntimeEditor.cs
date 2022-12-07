using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Avatars.Components;

#if UNITY_EDITOR
[InitializeOnLoad]
static class Av3RuntimeEditorState
{
    // Try to remember these states most of the time
    public static bool drawDefaultInspector = false;

    public static bool foldoutAvatar = true;
    public static bool foldoutOSC = false;
    public static bool foldoutNetworkClonesSync = true;
    public static bool foldoutPlayerLocalMirrorReflection = true;
    
    public static bool foldoutBuiltins = true;
    public static bool foldoutViseme = true;
    public static bool foldoutGesture = true;
    public static bool foldoutLocomotion = true;
    public static bool foldoutTrackingOther = true;

    public static bool foldoutUserValues = false;
    public static bool foldoutUserValuesFloat = true;
    public static bool floatViewCompact = true;
    public static bool foldoutUserValuesBool = true;
    public static bool foldoutUserValuesInt = true;
}

[CustomEditor(typeof(LyumaAv3Runtime))]
public class LyumaAv3RuntimeEditor : UnityEditor.Editor
{
    LyumaAv3Runtime runtime;

    void OnEnable()
    {
        runtime = (LyumaAv3Runtime) target;
    }

    // Force constant updates
    public override bool RequiresConstantRepaint() => true;

    public override void OnInspectorGUI()
    {
        Av3RuntimeEditorState.drawDefaultInspector = GUILayout.Toggle(Av3RuntimeEditorState.drawDefaultInspector, new GUIContent("Use Default Inspector"));
        
        GUILine();
        EditorGUILayout.Separator();

        if(Av3RuntimeEditorState.drawDefaultInspector)
        {
            DrawDefaultInspector();
            return;
        }

        EditorGUIUtility.labelWidth = 0.0f;
        float defaultWidth = EditorGUIUtility.labelWidth;

        // Avatar State management
        Av3RuntimeEditorState.foldoutAvatar = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutAvatar, "Avatar", EditorStyles.foldoutHeader);
        if(Av3RuntimeEditorState.foldoutAvatar)
        {
            EditorGUI.indentLevel++;
            runtime.ResetAvatar = GUILayout.Toggle(runtime.ResetAvatar, new GUIContent("Reset Avatar", "Resets avatar state machine instantly"), "Button");
            runtime.ResetAndHold = GUILayout.Toggle(runtime.ResetAndHold, new GUIContent("Reset and Hold", "Resets avatar state machine and waits until you uncheck this to start"), "Button");
            runtime.RefreshExpressionParams = GUILayout.Toggle(runtime.RefreshExpressionParams, new GUIContent("Refresh Expression Params", "Click if you modified your menu or parameter list"), "Button");
            runtime.KeepSavedParametersOnReset = EditorGUILayout.Toggle(new GUIContent(" Keep Saved Parameters On Reset", "Simulates saving and reloading the avatar"), runtime.KeepSavedParametersOnReset);
            
            EditorGUILayout.Separator();
            runtime.DebugDuplicateAnimator = (VRCAvatarDescriptor.AnimLayerType) EditorGUILayout.EnumPopup("Debug Duplicate Animator", runtime.DebugDuplicateAnimator);
            runtime.ViewAnimatorOnlyNoParams = (VRCAvatarDescriptor.AnimLayerType) EditorGUILayout.EnumPopup("View-only Debug Animator", runtime.ViewAnimatorOnlyNoParams);
        
            // OSC
            GUILine();
            EditorGUILayout.Separator();
            Av3RuntimeEditorState.foldoutOSC = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutOSC, "OSC (double click OSC Controller for debug and port settings)", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutOSC)
            {
                runtime.EnableAvatarOSC = EditorGUILayout.Toggle(new GUIContent("Enable OSC"), runtime.EnableAvatarOSC);
                runtime.LogOSCWarnings = EditorGUILayout.Toggle(new GUIContent("Log OSC Warnings"), runtime.LogOSCWarnings);
                runtime.OSCController = (LyumaAv3Osc) EditorGUILayout.ObjectField(new GUIContent("OSC Controller"), runtime.OSCController, typeof(LyumaAv3Osc), allowSceneObjects: true);
                // TODO : figure out how to show struct (A3EOSCConfiguration) OSCConfigurationFile
            }
            
            // Network Clones/Sync
            GUILine();
            EditorGUILayout.Separator();
            Av3RuntimeEditorState.foldoutNetworkClonesSync = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutNetworkClonesSync, "Network Clones/Sync", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutNetworkClonesSync)
            {
                runtime.CreateNonLocalClone = GUILayout.Toggle(runtime.CreateNonLocalClone, new GUIContent("Create Nonlocal Clone"), "Button");
                runtime.locally8bitQuantizedFloats = EditorGUILayout.Toggle(new GUIContent("Locally Quantize 8-bit Floats", "In VRChat, 8-bit float quantization only happens remotely. Check this to test your robustness to quantization locally, too. (example: 0.5 -> 0.503"), runtime.locally8bitQuantizedFloats);
                runtime.IKSyncRadialMenu = EditorGUILayout.Toggle(new GUIContent("IK Sync Radial Menu", "Parameters being actively edited in the radial menu will IK sync (lerp between values)"), runtime.IKSyncRadialMenu);
                runtime.NonLocalSyncInterval = EditorGUILayout.Slider(new GUIContent("Non-Local Sync Interval"), runtime.NonLocalSyncInterval, 0f, 2f);
            }
            
            // PlayerLocal/MirrorReflection
            GUILine();
            EditorGUILayout.Separator();
            Av3RuntimeEditorState.foldoutPlayerLocalMirrorReflection = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutPlayerLocalMirrorReflection, "PlayerLocal/MirrorReflection", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutPlayerLocalMirrorReflection)
            {
                runtime.EnableHeadScaling = EditorGUILayout.Toggle(new GUIContent(" Enable Head Scaling"), runtime.EnableHeadScaling);
                runtime.DisableMirrorAndShadowClones = EditorGUILayout.Toggle(new GUIContent(" Disable Mirror & Shadow Clones"), runtime.DisableMirrorAndShadowClones);
                runtime.DebugOffsetMirrorClone = EditorGUILayout.Toggle(new GUIContent(" Debug Offset Mirror Clone", "To Debug both copies at once"), runtime.DebugOffsetMirrorClone);
                runtime.ViewMirrorReflection = EditorGUILayout.Toggle(new GUIContent(" View Mirror Reflection"), runtime.ViewMirrorReflection);
                runtime.ViewBothRealAndMirror = EditorGUILayout.Toggle(new GUIContent(" View Real & Mirror"), runtime.ViewBothRealAndMirror);
            }
            EditorGUI.indentLevel--;
        }

        // Built-in inputs
        GUILine();
        EditorGUILayout.Separator();
        Av3RuntimeEditorState.foldoutBuiltins = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutBuiltins, "Built-in Parameters", EditorStyles.foldoutHeader);
        if(Av3RuntimeEditorState.foldoutBuiltins)
        {
            EditorGUI.indentLevel++;

            // Viseme
            Av3RuntimeEditorState.foldoutViseme = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutViseme, "Visemes/Voice", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutViseme)
            {
                runtime.Viseme = (LyumaAv3Runtime.VisemeIndex) EditorGUILayout.EnumPopup("Viseme", runtime.Viseme);
                runtime.VisemeIdx = EditorGUILayout.IntSlider(new GUIContent("Viseme ID"), runtime.VisemeIdx, 0, 15);
                runtime.Voice = EditorGUILayout.Slider(new GUIContent("Voice", "Volume of the user's microphone input"), runtime.Voice, 0f, 1f);
            }
            GUILine(1);

            // Hand Gestures
            Av3RuntimeEditorState.foldoutGesture = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutGesture, "Hand Gestures", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutGesture)
            {
                // Hack to make sliders not dispapear
                EditorGUIUtility.labelWidth = defaultWidth * 0.65f;
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Left
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Left", EditorStyles.boldLabel);
                        runtime.GestureLeft = (LyumaAv3Runtime.GestureIndex) EditorGUILayout.EnumPopup("Gesture", runtime.GestureLeft);
                        runtime.GestureLeftIdx = EditorGUILayout.IntSlider(new GUIContent("Gesture ID"), runtime.GestureLeftIdx, 0, 9);
                        runtime.GestureLeftWeight = EditorGUILayout.Slider(new GUIContent("Weight"), runtime.GestureLeftWeight, 0f, 1f);
                    }

                    // Right
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Right", EditorStyles.boldLabel);
                        runtime.GestureRight = (LyumaAv3Runtime.GestureIndex) EditorGUILayout.EnumPopup("Gesture", runtime.GestureRight);
                        runtime.GestureRightIdx = EditorGUILayout.IntSlider(new GUIContent("Gesture ID"), runtime.GestureRightIdx, 0, 9);
                        runtime.GestureRightWeight = EditorGUILayout.Slider(new GUIContent("Weight"), runtime.GestureRightWeight, 0f, 1f);
                    }
                }
                EditorGUIUtility.labelWidth = defaultWidth;
            }
            GUILine(1);

            // Locomotion
            Av3RuntimeEditorState.foldoutLocomotion = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutLocomotion, "Locomotion", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutLocomotion)
            {
                runtime.Velocity = EditorGUILayout.Vector3Field(new GUIContent("Velocity"), runtime.Velocity);
                runtime.AngularY = EditorGUILayout.Slider(new GUIContent("Angular Y Velocity"), runtime.AngularY, -400f, 400f);
                runtime.Upright = EditorGUILayout.Slider(new GUIContent("Upright", "Headset location between floor (0) and User Real Height (1)"), runtime.Upright, 0f, 1f);
                
                EditorGUILayout.Separator();
                
                runtime.Grounded = EditorGUILayout.Toggle(new GUIContent("Grounded"), runtime.Grounded);
                runtime.Jump = EditorGUILayout.Toggle(new GUIContent("Jump"), runtime.Jump);
                runtime.JumpPower = EditorGUILayout.FloatField(new GUIContent("Jump Power"), runtime.JumpPower);
                runtime.RunSpeed = EditorGUILayout.FloatField(new GUIContent("Run Speed"), runtime.RunSpeed);

                EditorGUILayout.Separator();

                runtime.Seated = EditorGUILayout.Toggle(new GUIContent("Seated"), runtime.Seated);
                runtime.AFK = EditorGUILayout.Toggle(new GUIContent("AFK"), runtime.AFK);
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Calibration Pose");
                    runtime.TPoseCalibration = GUILayout.Toggle(runtime.TPoseCalibration, new GUIContent(" IK Pose"));
                    runtime.IKPoseCalibration = GUILayout.Toggle(runtime.IKPoseCalibration, new GUIContent(" T Pose"));
                }
            }
            GUILine(1);

            // Tracking/Other
            Av3RuntimeEditorState.foldoutTrackingOther = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutTrackingOther, "Tracking Setup and Other", EditorStyles.foldoutHeader);
            if(Av3RuntimeEditorState.foldoutTrackingOther)
            {
                runtime.TrackingType = (LyumaAv3Runtime.TrackingTypeIndex) EditorGUILayout.EnumPopup("Tracking Type", runtime.TrackingType);
                runtime.TrackingTypeIdx = EditorGUILayout.IntSlider(new GUIContent("Tracking Type ID"), runtime.TrackingTypeIdx, 0, 6);
                
                runtime.VRMode = EditorGUILayout.Toggle(new GUIContent("VR Mode"), runtime.VRMode);
                runtime.MuteSelf = EditorGUILayout.Toggle(new GUIContent("Muted"), runtime.MuteSelf);
                runtime.InStation = EditorGUILayout.Toggle(new GUIContent("In Station"), runtime.InStation);
            }
            GUILine(1);

            EditorGUILayout.LabelField("Output State (read-only)", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(true))
            {
                runtime.IsLocal = EditorGUILayout.Toggle(new GUIContent("IsLocal"), runtime.IsLocal);
                runtime.LocomotionIsDisabled = EditorGUILayout.Toggle(new GUIContent("Locomotion Disabled"), runtime.LocomotionIsDisabled);
            }

            EditorGUI.indentLevel--;
        }

        // User-generated Values
        GUILine();
        Av3RuntimeEditorState.foldoutUserValues = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutUserValues, "User Parameters", EditorStyles.foldoutHeader);
        if(Av3RuntimeEditorState.foldoutUserValues)
        {
            EditorGUIUtility.labelWidth = defaultWidth * 0.75f;
            EditorGUI.indentLevel++;
            using (new EditorGUILayout.HorizontalScope())
            {
                Av3RuntimeEditorState.foldoutUserValuesFloat = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutUserValuesFloat, "Floats", EditorStyles.foldoutHeader);
                Av3RuntimeEditorState.floatViewCompact = EditorGUILayout.Toggle("Compact View", Av3RuntimeEditorState.floatViewCompact);
            }
            EditorGUI.indentLevel--;
            if(Av3RuntimeEditorState.foldoutUserValuesFloat)
            {
                foreach(LyumaAv3Runtime.FloatParam param in runtime.Floats)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if(!param.synced)
                            EditorGUILayout.LabelField("        " + param.name + "  (local)");
                        else
                            EditorGUILayout.LabelField(new GUIContent("  " + param.name, EditorGUIUtility.IconContent("preAudioLoopOff").image));

                        if(Av3RuntimeEditorState.floatViewCompact)
                        {
                            param.expressionValue = EditorGUILayout.Slider(param.expressionValue, -1f, 1f, GUILayout.MinWidth(125));
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.TextField(param.value.ToString(), GUILayout.Width(45));
                            }
                        }
                    }
                    if(!Av3RuntimeEditorState.floatViewCompact)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            param.expressionValue = EditorGUILayout.Slider(" ", param.expressionValue, -1f, 1f, GUILayout.MinWidth(125));
                            using (new EditorGUI.DisabledGroupScope(true))
                            {
                                EditorGUILayout.TextField(param.value.ToString(), GUILayout.Width(45));
                            }
                        }
                    }
                }
            }
            GUILine(1);
            
            EditorGUI.indentLevel++;
            Av3RuntimeEditorState.foldoutUserValuesInt = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutUserValuesInt, "Ints", EditorStyles.foldoutHeader);
            EditorGUI.indentLevel--;
            if(Av3RuntimeEditorState.foldoutUserValuesInt)
            {
                foreach(LyumaAv3Runtime.IntParam param in runtime.Ints)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if(!param.synced)
                            EditorGUILayout.LabelField("        " + param.name + "  (local)");
                        else
                            EditorGUILayout.LabelField(new GUIContent("  " + param.name, EditorGUIUtility.IconContent("preAudioLoopOff").image));

                        param.value = EditorGUILayout.IntSlider(param.value, 0, 255);
                    }
                }
            }
            GUILine(1);

            EditorGUI.indentLevel++;
            Av3RuntimeEditorState.foldoutUserValuesBool = EditorGUILayout.Foldout(Av3RuntimeEditorState.foldoutUserValuesBool, "Bools", EditorStyles.foldoutHeader);
            EditorGUI.indentLevel--;
            if(Av3RuntimeEditorState.foldoutUserValuesBool)
            {
                foreach(LyumaAv3Runtime.BoolParam param in runtime.Bools)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if(!param.synced)
                            EditorGUILayout.LabelField("        " + param.name + "  (local)");
                        else
                            EditorGUILayout.LabelField(new GUIContent("  " + param.name, EditorGUIUtility.IconContent("preAudioLoopOff").image));

                        param.value = EditorGUILayout.Toggle(param.value);
                    }
                }
            }

            EditorGUIUtility.labelWidth = defaultWidth;
        }

        EditorGUILayout.Space();
    }

    void GUILine(Color color, int height=3)
    {
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, color);
    }

    void GUILine(int height=3) => GUILine(new Color(0.5f, 0.5f, 0.5f, 0.5f), height);
}
#endif

