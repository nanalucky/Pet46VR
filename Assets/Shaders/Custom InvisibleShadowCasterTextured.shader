Shader "Custom/InvisibleShadowCasterTextured" 
 {
     Subshader
     {
         UsePass "VertexLit/SHADOWCOLLECTOR"    
         UsePass "VertexLit/SHADOWCASTER"
     }
 
     Fallback off
 }