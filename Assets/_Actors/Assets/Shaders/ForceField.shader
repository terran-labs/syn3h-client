Shader "ForceField"
{
	Properties
	{
		_Color("_Color", Color) = (0,1,0,1)
		_CutStrength("_CutStrength", Range(0.1,100) ) = 0.1
		_CutRange("_CutRange", Range(0,2) ) = 0
		_RimPower("_RimPower", Range(1,5) ) = 1
		_Inside("_Inside", Range(0,0.2) ) = 0
		_Rim("_Rim", Range(1,2) ) = 1.2
		_Texture("_Texture", 2D) = "white" {}
		_Speed("_Speed", Range(0.5,5) ) = 0.5
		_Tile("_Tile", Range(1,10) ) = 5
		_Strength("_Strength", Range(0,5) ) = 1.5
		_Cube ("_Cube", Cube) = "" {}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Opaque"
		}
		LOD 500

		Cull Off
		ZWrite On
		ZTest Always

		CGPROGRAM
			#pragma surface surf Lambert alpha addshadow
			#pragma target 3.0

			struct Input {
				float4 screenPos;
				float3 viewDir;
				float2 uv_Texture;
				float3 worldRefl;
				INTERNAL_DATA
			};

			float4 _Color;
			float _CutStrength;
			float _CutRange;
			sampler2D _CameraDepthTexture;
			float _RimPower;
			float _Inside;
			float _Rim;
			sampler2D _Texture;
			float _Speed;
			float _Tile;
			float _Strength;
			samplerCUBE _Cube;

			//void vert (inout appdata_full v, out Input o) {
			//	v.vertex.xyz += v.normal * abs(fmod(v.vertex.x,1)) * fmod(1, _Time.y) * 1;
			//}

			void surf (Input IN, inout SurfaceOutput o) {

				//Base forcefield
				float ScreenDepthDiff0= LinearEyeDepth (tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r) - IN.screenPos.z;
				float4 Step1=step(ScreenDepthDiff0, 0.0 );
				float4 Multiply10=float4(-2,-2,-2,-2) * Step1;
				float4 Add1=float4(1.0,1.0,1.0,1.0) + Multiply10;
				float4 Multiply9=ScreenDepthDiff0 * Add1;
				float4 Subtract0=_CutRange.xxxx - Multiply9;
				float4 Saturate1=saturate(Subtract0);
				float4 Multiply6=_CutStrength.xxxx * Saturate1;
				float4 Step2=step(0.0 ,ScreenDepthDiff0);
				float4 Multiply4=Step2 * float4(1.0,1.0,1.0,1.0);
				float4 Clamp1=clamp(Multiply6,Multiply4,float4(100,100,100,100));
				float4 Step3=step(Clamp1,float4(0.0,0.0,0.0,0.0));
				float4 Multiply8=Step3 * _RimPower.xxxx;
				float Fresnelf= 1.0 - dot( normalize( float4(IN.viewDir, 1.0).xyz), normalize( float3(0,0,1) ) );
				float4 Fresnel0=float4(Fresnelf,Fresnelf,Fresnelf,Fresnelf);
				float4 Add0=Multiply8 + Fresnel0;
				float4 Step0=step(Add0,float4(1.0,1.0,1.0,1.0));
				float4 Clamp0=clamp(Step0,_Inside.xxxx,float4(1.0,1.0,1.0,1.0));
				float4 Pow0=pow(Add0,_Rim.xxxx);
				float4 Multiply5=_Time * _Speed.xxxx;
				float4 UV_Pan0=float4((IN.uv_Texture.xyxy).x,(IN.uv_Texture.xyxy).y + Multiply5.x,(IN.uv_Texture.xyxy).z,(IN.uv_Texture.xyxy).w);
				float4 Multiply1=UV_Pan0 * _Tile.xxxx;
				float4 Tex2D0=tex2D(_Texture,Multiply1.xy);
				float4 Multiply2=Tex2D0 * _Strength.xxxx;
				float4 Multiply0=Pow0 * Multiply2;
				float4 Multiply3=Clamp0 * Multiply0;
				float4 Multiply7=Clamp1 * Multiply3;
				float4 Master0_0_NoInput = float4(0,0,0,0);
				float4 Master0_1_NoInput = float4(0,0,1,1);
				float4 Master0_3_NoInput = float4(0,0,0,0);
				float4 Master0_4_NoInput = float4(0,0,0,0);
				float4 Master0_6_NoInput = float4(1,1,1,1);


				//Rim Lighting
				float viewDot = dot (normalize(IN.viewDir), o.Normal);
				half rim = 1.0 - saturate(viewDot);
				o.Alpha = Multiply7;
				o.Albedo = (1 - min(1,ScreenDepthDiff0)) * .3;

				o.Gloss = 1;
				o.Specular = 1;
				o.Normal = float3(0,0,1); //UnpackNormal (float4(0,0,0,1));

				//Reflection
				//o.Emission = lerp(_Color.rgb, texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb, .7);
				o.Emission = _Color.rgb / 2 + texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb;
				//o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          		//o.Emission = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb;

				//o.Emission = _Color.rgb;

			}
		ENDCG


	}
	Fallback "Transparent/VertexLit"
}