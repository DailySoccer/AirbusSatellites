using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Orbitation))]
public class OrbitationEditor : Editor {

	enum setupPlanet {
		Setup_Orbit, 
		Setup_Rotation, 
		Setup_Seasons
	}

 	

	setupPlanet SetupPlanet;
	bool OrbitTexture;

	public override void OnInspectorGUI () {
		Orbitation myTarget = (Orbitation)target;

		EditorGUILayout.Space ();
		GUILayout.Label("ForceX Tools: Planet & Orbit Editor");
		GUILayout.Label("Version 1.0.1");
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		
		myTarget.TimeMultiplier = EditorGUILayout.IntField ( "Time Multiplier: ", myTarget.TimeMultiplier);
		myTarget.Name = EditorGUILayout.TextField ( "Name: ", myTarget.Name);
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		
		EditorGUILayout.Space ();
		//EditorGUIUtility.LookLikeInspector();
		EditorGUIUtility.labelWidth = 0;
		EditorGUIUtility.fieldWidth = 0;
		myTarget.Parent = EditorGUILayout.ObjectField ("Parent Mass: ", myTarget.Parent, typeof(Transform), true) as Transform;
		//EditorGUIUtility.LookLikeControls();
		EditorGUIUtility.labelWidth = 0;
		EditorGUIUtility.fieldWidth = 0;
		EditorGUILayout.Space ();
		myTarget.LockOrbit = EditorGUILayout.Toggle("Lock Orbit:", myTarget.LockOrbit);
		myTarget.TidalLock = EditorGUILayout.Toggle("Tidal Lock:", myTarget.TidalLock);
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		GUILayout.Label("Orbit Display 	------------------------------", EditorStyles.boldLabel);
		myTarget._DrawOrbit = EditorGUILayout.Toggle("Draw Orbit:", myTarget._DrawOrbit);
		
		if(myTarget._DrawOrbit == true){
			myTarget.Segments = EditorGUILayout.IntField ( "Display Segments: ", myTarget.Segments);
			myTarget.DisplaySize = EditorGUILayout.FloatField ( "Display Size: ", myTarget.DisplaySize);
			myTarget.DisplayColor = EditorGUILayout.ColorField ( "Display Color: ", myTarget.DisplayColor);
			
			EditorGUILayout.Space ();
			myTarget.UseTexture = EditorGUILayout.Toggle("Use Texture:", myTarget.UseTexture);
			if(myTarget.UseTexture == true){
				//EditorGUIUtility.LookLikeInspector();
				EditorGUIUtility.labelWidth = 0;
				EditorGUIUtility.fieldWidth = 0;
				myTarget.DisplayTexture = EditorGUILayout.ObjectField ("Display Texture: ", myTarget.DisplayTexture, typeof(Texture2D), true) as Texture2D;
				//EditorGUIUtility.LookLikeControls(); 
				EditorGUIUtility.labelWidth = 0;
				EditorGUIUtility.fieldWidth = 0;
				myTarget.DisplayTiling = EditorGUILayout.IntField ( "Texture Tiling: ", myTarget.DisplayTiling);
			}
		}
		EditorGUILayout.Space (); 
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		//SetupPlanet = EditorGUILayout.EnumPopup("Planet Options:",SetupPlanet);
		
		GUILayout.Label("Planet Orbit 	------------------------------", EditorStyles.boldLabel);
		EditorGUILayout.Space ();
		
		//if(SetupPlanet == 0){
		myTarget.AxialTilt = EditorGUILayout.Slider("Axial Tilt: ",myTarget.AxialTilt,0,360);
		myTarget.OrbitalDistance = EditorGUILayout.FloatField ( "Orbital Distance: ", myTarget.OrbitalDistance);
		myTarget.OrbitAngle = EditorGUILayout.FloatField ( "Orbit Angle: ", myTarget.OrbitAngle);
		EditorGUILayout.Space ();
		myTarget.OrbitOffset = EditorGUILayout.Vector3Field ( "Orbit Center Offset: ", myTarget.OrbitOffset);
		EditorGUILayout.Space ();
		
		EditorGUILayout.Space ();
		myTarget.OrbitPosOffset = EditorGUILayout.Slider("Start Orbital Offset: ",myTarget.OrbitPosOffset,0,360);
		EditorGUILayout.Space ();
		if(!myTarget.LockOrbit){
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			myTarget.SetOrbit = (Orbitation.Orbit)EditorGUILayout.EnumPopup("Set Orbit:", myTarget.SetOrbit);
			EditorGUILayout.Space ();
			if(myTarget.SetOrbit == 0){
				GUILayout.Label("Orbital Period x1 Earth Years");
				myTarget.OrbitalPeriod = EditorGUILayout.FloatField ( "Orbital Period: ", myTarget.OrbitalPeriod);
			}else{
				GUILayout.Label("Orbital Period In Earth Time");
				myTarget.OrbitYears = EditorGUILayout.IntField ( "Orbit Years: ", myTarget.OrbitYears);
				myTarget.OrbitDays = EditorGUILayout.IntField ( "Orbit Days: ", myTarget.OrbitDays);
				myTarget.OrbitHours = EditorGUILayout.IntField ( "Orbit Hours: ", myTarget.OrbitHours);
				myTarget.OrbitMinutes = EditorGUILayout.IntField ( "Orbit Minutes: ", myTarget.OrbitMinutes);
				myTarget.OrbitSeconds = EditorGUILayout.FloatField ( "Orbit Seconds: ", myTarget.OrbitSeconds);
			}
		}
		//}else
		
		//if(SetupPlanet == 1){	
		if(!myTarget.TidalLock){
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			GUILayout.Label("Planet Rotation 	------------------------------", EditorStyles.boldLabel);
			EditorGUILayout.Space ();
			myTarget.SetRotation = (Orbitation.Rotation)EditorGUILayout.EnumPopup("Set Rotation:",myTarget.SetRotation);
			EditorGUILayout.Space ();
			if(myTarget.SetRotation == 0){
				GUILayout.Label("Rotation Period x1 Earth Days");
				myTarget.RotationPeriod = EditorGUILayout.FloatField ( "Orbital Period: ", myTarget.OrbitalPeriod);
			}else{
				GUILayout.Label("Rotation Period In Earth Time");
				myTarget.RotationYears = EditorGUILayout.IntField ( "Rotation Years: ", myTarget.RotationYears);
				myTarget.RotationDays = EditorGUILayout.IntField ( "Rotation Days: ", myTarget.RotationDays);
				myTarget.RotationHours = EditorGUILayout.IntField ( "Rotation Hours: ", myTarget.RotationHours);
				myTarget.RotationMinutes = EditorGUILayout.IntField ( "Rotation Minutes: ", myTarget.RotationMinutes);
				myTarget.RotationSeconds = EditorGUILayout.FloatField ( "Rotation Seconds: ", myTarget.RotationSeconds);
			}
			
		}
		//}
		
		EditorGUILayout.Space ();	
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if(!myTarget.LockOrbit){
			GUILayout.Label("Local Planetary Statistics 	--------------------",  EditorStyles.boldLabel);
			
			EditorGUILayout.Space ();
			//GUILayout.Label("Allow Start Orbital Offset To Effect Year");
			//myTarget.OrbitOffSetYear = EditorGUILayout.Toggle("Enabled:", myTarget.OrbitOffSetYear);
			myTarget.CurrentOrbitPos = EditorGUILayout.Slider("Orbital Position: ",myTarget.CurrentOrbitPos,0,360);
			
			EditorGUILayout.Space ();
			myTarget.KeepTime = EditorGUILayout.Toggle("Keep Local Time:", myTarget.KeepTime);
			
			if(myTarget.KeepTime == true){
				//EditorGUIUtility.LookLikeInspector();
				EditorGUIUtility.labelWidth = 0;
				EditorGUIUtility.fieldWidth = 0;
				myTarget.CounterYear = EditorGUILayout.IntField ( "	Orbits: ", myTarget.CounterYear);
				if(!myTarget.TidalLock){
					myTarget.CounterDay = EditorGUILayout.IntField ( "	Rotations: ", myTarget.CounterDay);
					
					EditorGUILayout.Space ();	
					
					myTarget.RotInOrbit = EditorGUILayout.IntField ( "	Rotations Per Orbit: ", myTarget.RotInOrbit);
					myTarget.HoursInDay = EditorGUILayout.IntField ( "	Hours Per Rotation: ", myTarget.HoursInDay);
					
					EditorGUILayout.Space ();	
					GUILayout.Label("Current Local Time");
					myTarget.CounterHour = EditorGUILayout.IntField ( "	Hours: ", myTarget.CounterHour);
					myTarget.CounterMinute = EditorGUILayout.IntField ( "	Minutes: ", myTarget.CounterMinute);
					myTarget.CounterSecond = EditorGUILayout.FloatField ( "	Seconds: ", myTarget.CounterSecond);
				}
			}
		}
	}
}
