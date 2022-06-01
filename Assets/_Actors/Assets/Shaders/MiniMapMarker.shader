Shader "FX/MiniMapMarker" {
	Properties {
   		_Color ("Main Color", Color) = (1,1,1,0)
	}
	SubShader {
		ZTest Always
		Alphatest Greater 0
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Tags {"Queue"="Overlay" "RenderType"="Transparent"}
		Pass { Color [_Color]}
	} 
}