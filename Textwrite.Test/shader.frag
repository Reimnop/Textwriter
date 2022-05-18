#version 430 core

layout(location = 0) out vec4 fragColor;

layout(location = 0) in vec2 UV;
layout(location = 1) in vec4 Color;
layout(location = 2) flat in int Index;

layout(location = 1) uniform sampler2D atlas[16];

void main() {
    vec4 outCol;
    if (Index == 0) {
        outCol = vec4(1.0);
    } else if (Index < 0) {
        outCol = texture(atlas[-Index - 1], UV).rgba;
    } else {
        outCol = vec4(1.0, 1.0, 1.0, texture(atlas[Index - 1], UV).r);
    }
    
    fragColor = outCol * Color;
}
