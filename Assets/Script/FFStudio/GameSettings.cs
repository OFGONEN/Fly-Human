﻿/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields (Settings)
    // Info: You can use Title() attribute ONCE for every game-specific group of settings.
    [ Title( "Stickman" ) ]
		[ LabelText( "Pose Duration" ) ] public float stickman_pose_duration;
		[ LabelText( "Jump Power" ) ] public float stickman_jump_power;
		[ LabelText( "Jump Duration" ) ] public float stickman_jump_duration;
		[ LabelText( "Disable Duration" ) ] public float stickman_disableDuration;
		[ LabelText( "Target Jump Power" ) ] public Vector2 stickman_target_jump_power;
		[ LabelText( "Target Jump Duration" ) ] public Vector2 stickman_target_jump_duration;

    [ Title( "Vehicle Platform" ) ]
		[ LabelText( "Vehicle Ray Cast Height" ) ] public float vehicle_rayCast_height;
		[ LabelText( "Vehicle Ray Cast Direction" ) ] public Vector3 vehicle_rayCast_direction;
		[ LabelText( "Vehicle Ray Cast Distance" ) ] public float vehicle_rayCast_distance;
		[ LabelText( "Vehicle Movement Step Distance" ) ] public float vehicle_movement_step;
		[ LabelText( "Vehicle Movement Look Speed" ) ] public float vehicle_movement_look_speed;
		[ LabelText( "Vehicle Movement Look Axis" ) ] public Vector3 vehicle_movement_look_axis;

    [ Title( "Vehicle Air" ) ]
		[ LabelText( "Vehicle Air Movement Rotate Axis" ) ] public Vector3 vehicle_fly_rotate_axis;
		[ LabelText( "Vehicle Air Movement Rotate Clamp" ) ] public float vehicle_fly_rotate_clamp;
		[ LabelText( "Vehicle Air Movement Cofactor" ) ] public float vehicle_fly_cofactor = 1.5f;

	[ Title( "Vehicle Landing" ) ]
		[ LabelText( "Vehicle Landing Collide Angle" ) ] public float vehicle_landing_angle;
		[ LabelText( "Vehicle Landing Adjust Duration" ) ] public float vehicle_landing_adjust_duration;
	
	[ Title( "Target Vehicle" ) ]
		[ LabelText( "Target Vehicle Offset" ) ] public Vector3 vehicle_target_offset;
		[ LabelText( "Target Vehicle Spawn Offset" ) ] public Vector3 vehicle_target_spawn_offset;
		[ LabelText( "Target Vehicle Spawn Duration" ) ] public float vehicle_target_spawn_duration;

    [ Title( "Camera" ) ]
        [ LabelText( "Follow Speed" ), SuffixLabel( "units/seconds" ), Min( 0 ) ] public float camera_follow_speed;
        [ LabelText( "Follow Offset Position" ) ] public Vector3 camera_follow_offset_position;

        [ LabelText( "Follow Offset Rotation" ) ] public Vector3 camera_follow_offset_rotation;

    [ Title( "UI Vehicle Progress" ) ]
		[ LabelText( "UI Vehicle Progress Gap Distance" ) ] public float ui_vehicle_progress_distance;
		[ LabelText( "UI Progress Travel Duration" ) ] public float ui_vehicle_progress_travel_duration;
		[ LabelText( "UI Progress Travel Ease" ) ] public Ease ui_vehicle_progress_travel_ease;
    
    [ Title( "Project Setup", "These settings should not be edited by Level Designer(s).", TitleAlignments.Centered ) ]
        public Vector3 game_forward;
        public int maxLevelCount;
        
        // Info: 3 groups below (coming from template project) are foldout by design: They should remain hidden.
		[ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text relative float height"                ) ] public float ui_PopUp_height;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text float duration"                       ) ] public float ui_PopUp_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Random Spawn Area in Screen" ), SuffixLabel( "percentage" ) ] public float ui_particle_spawn_width; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Duration" ), SuffixLabel( "seconds" ) ] public float ui_particle_spawn_duration; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Ease" ) ] public Ease ui_particle_spawn_ease;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Wait Time Before Target" ) ] public float ui_particle_target_waitTime;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Time" ) ] public float ui_particle_target_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Ease" ) ] public Ease ui_particle_target_ease;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Safe Area Base Top Offset" ) ] public int ui_safeArea_offset_top = 88;

    [ Title( "UI Particle" ) ]
		[ LabelText( "Random Spawn Area in Screen Witdh Percentage" ) ] public float uiParticle_spawn_width_percentage = 10;
		[ LabelText( "Spawn Movement Duration" ) ] public float uiParticle_spawn_duration = 0.1f;
		[ LabelText( "Spanwn Movement Ease" ) ] public DG.Tweening.Ease uiParticle_spawn_ease = DG.Tweening.Ease.Linear;
		[ LabelText( "Target Travel Wait Time" ) ] public float uiParticle_target_waitDuration = 0.16f;
		[ LabelText( "Target Travel Duration" ) ] public float uiParticle_target_duration = 0.4f;
		[ LabelText( "Target Travel Duration (REWARD)" ) ] public float uiParticle_target_duration_reward = 0.85f;
		[ LabelText( "Target Travel Ease" ) ] public DG.Tweening.Ease uiParticle_target_ease = DG.Tweening.Ease.Linear;


        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_duration;
#endregion

#region Fields (Singleton Related)
        static GameSettings instance;

        delegate GameSettings ReturnGameSettings();
        static ReturnGameSettings returnInstance = LoadInstance;
#if UNITY_EDITOR
		public static GameSettings Instance
		{
			get
			{
				if( Application.isPlaying )
					return returnInstance();
				else
				{
					return UnityEditor.AssetDatabase.LoadAssetAtPath< GameSettings >( "Assets/Resources/game_settings.asset" );
				}
			}

		}
#else 
		public static GameSettings Instance => returnInstance();
#endif
#endregion

#region Implementation
        static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
